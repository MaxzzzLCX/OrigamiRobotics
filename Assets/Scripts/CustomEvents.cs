using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class CustomEvents : MonoBehaviour
{
    public UnityEvent Event=new UnityEvent();

    public void CallEvent()
    {
        Event?.Invoke();
    }
}
