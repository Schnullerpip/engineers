using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionReceiver : MonoBehaviour
{
    public abstract void receive(bool isToggle);
}
