using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{

    [SerializeField]
    private InteractionReceiver[] interactionReceivers;

    [SerializeField]
    protected bool isToggle = false;

    [SerializeField]
    private Color outlineColor = Color.yellow;

    protected Animator animator;

    private Outline outline;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        this.outline = this.gameObject.AddComponent<Outline>();
        this.outline.OutlineMode = Outline.Mode.OutlineVisible;
        this.outline.OutlineColor = this.outlineColor;
        this.outline.OutlineWidth = 10f;
    }

    // Start is called before the first frame update
    private void Start()
    {
        this.StartCoroutine("disableOutline");
    }

    /**
     * The Interactable's public interface that will be called from a user, automatically notifying the InteractionReceivers 
     */
    public void interact()
    {
        this.interaction();
        this.notify();
    }

    /**
     * Setting outline to true gives the component an outline
     */
    public void setOutline(bool enable)
    {
        this.outline.enabled = enable;
    }

    /**
     * The Interactables individual action that must be implemented
     */
    protected abstract void interaction();

    /**
     * Triggers the InteractionReceivers observing this Interactable
     */
    private void notify()
    {
        foreach(var receiver in interactionReceivers)
        {
            receiver.receive(this.isToggle);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        if(player)
        {
            player.registerInteractable(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<Player>();
        if(player)
        {
            player.unregisterInteractable(this);
            this.setOutline(false);
        }
    }

    /**
     * Due to a bug we cant create the outline component AND disable it in the same frame.
     * So at first we create the outline component then on the immediate next frame we initially disable it.
     */
    private IEnumerator disableOutline()
    {
        yield return new WaitForSeconds(0);
        this.outline.enabled = false;
    }

}
