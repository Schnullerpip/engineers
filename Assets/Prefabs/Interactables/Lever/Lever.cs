using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Interactable
{
    private bool leverPulledDown = false;

    /**
     * Triggers the animator to transition between pulled down state and pulled up state.
     * If the lever is a toggle it will wait for a short duration then directly un-toggle the lever again.
     */
    protected override void interaction()
    {
        if(this.untoggler == null)
        {
            animator.SetBool("LeverToggle", leverPulledDown = !leverPulledDown);

            if(this.isToggle)
            {
               this.untoggler = StartCoroutine("WaitThenUntoggle"); 
            }
        }

    }

    /**
     * Keeps track if a coroutine is currently active and pending to untoggle a lever.
     */
    private Coroutine untoggler = null;

    private IEnumerator WaitThenUntoggle()
    {
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("LeverToggle", leverPulledDown = !leverPulledDown);
        this.untoggler = null;
    }
}
