using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : Interactable
{

    private bool isPressed = false;

    protected override void interaction()
    {
        if (!this.isPressed)
        {
            this.isPressed = true;
            this.animator.SetTrigger("PressedButton");
        }
    }

    void resetButtonState()
    {
        this.isPressed = false;
    }
}
