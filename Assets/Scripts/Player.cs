using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    //values adjustable by the user

    public float speed = 10;
    public float jumpStrength = 10;
    public float jumpMovementDampening = 0.5f;

    //cached instances grabbed on start

    private new Transform transform;
    private Rigidbody rigidBody;

    //internal state

    private bool airborn = false;
    private bool isTouchingLadder = false;
    private bool useGravity = true;

    private List<Interactable> interactablesInRange = new List<Interactable>();

    public bool touchingLadder
    {
        get
        {
            return this.isTouchingLadder;
        }

        set
        {
            this.isTouchingLadder = value;
            if(!value)
            {
                this.useGravity = true;
            }
        }
    }

    public void registerInteractable(Interactable i)
    {
        this.interactablesInRange.Add(i);
    }

    public void unregisterInteractable(Interactable i)
    {
        this.interactablesInRange.Remove(i);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.transform = this.gameObject.transform;
        this.rigidBody = GetComponent<Rigidbody>();

        if (!this.rigidBody)
        {
            //we could just add it here but i think this is a bad pattern -> viewing the object in the editor/scene should reveal what components it has.
            //this.rigidBody = this.gameObject.AddComponent<Rigidbody>();
            Debug.LogError("Could not retrieve a rigid body from the player mock object!?");
        }
    }

    private void FixedUpdate()
    {
        this.rigidBody.useGravity = this.useGravity;
    }

    // Update is called once per frame
    void Update()
    {
        this.processMovement();
        this.processInteraction();
    }

    private void processInteraction()
    {
        //1. calculate an interactable that can theoretically be interacted with
        var playerPosition = this.transform.position;
        float smallestDistance = float.MaxValue;
        Interactable closest = null;
        foreach(var i in this.interactablesInRange)
        {
            i.setOutline(false);
            var distanceSquared = (playerPosition - i.transform.position).sqrMagnitude;
            if(distanceSquared <= smallestDistance)
            {
                closest = i;
            }
        }

        if (!closest)
        {
            return;
        }

        //2. mark the interactable for the user to see it
        closest.setOutline(true);

        //3. detect user-input that should trigger an interaction if possible
        if (closest && Input.GetKeyDown("space"))
        {
            closest.interact();
        }
    }

    private void processMovement()
    {
        //fetch user-input
        var xAxisInput = Input.GetAxisRaw("Horizontal");
        var yAxisInput = Input.GetAxisRaw("Vertical");

        var tmp = this.transform.position;

        //jump
        if(!this.airborn && yAxisInput > 0)
        {
            var v = this.rigidBody.velocity;
            v.y = jumpStrength;
            this.rigidBody.velocity = v;
            return;
        }

        //move player left/right
        if(xAxisInput != 0) 
        {
            //apply the horizontal movement
            tmp.x += xAxisInput * speed * Time.deltaTime * (this.airborn ? this.jumpMovementDampening : 1);
        }

        //if player is in contact with a ladder s/he might also move up down directly
        if(this.isTouchingLadder)
        {
            if(yAxisInput > 0 )
            {
                //stop a possible jump velocity / grab onto the ladder and stop moving
                this.rigidBody.velocity = Vector3.zero;
                this.useGravity = false;

                //apply the vertical movement
                tmp.y += yAxisInput * speed / 2 * Time.deltaTime;
            }
            else if(yAxisInput < 0)
            {
                //if the player wants to go downwards just let them drop
                this.useGravity = true;
            }
        }

        //finally apply the new transform position
        this.transform.position = tmp;
    }

    private void OnCollisionExit(Collision collision)
    {
        if(!collision.collider.isTrigger)
        {
            //disable jumping
            this.airborn = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.isTrigger)
        {
            //enable jumping again
            this.airborn = false;
        }
    }
}
