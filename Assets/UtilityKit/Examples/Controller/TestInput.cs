using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MCFramework;

public class TestInput : MonoBehaviour
{
    private Movement m_Movement;

    // Start is called before the first frame update
    void Start()
    {
        m_Movement = GetComponent<Movement>();

        GameInputManager.ObserveAxis("Horizontal");
        GameInputManager.ObserveAxis("Vertical");

        GameInputManager.Register(OnInputEvent);
    }

    void OnDisable()
    {
        GameInputManager.Unregister(OnInputEvent);
    }

    void OnInputEvent(EventData data)
    {
        if (data.used) 
            return;

        if (data.axis == "Horizontal")
        {
            m_Movement.Horizontal = data.value;
            data.used = true;
        }
        else if (data.axis == "Vertical")
        {
            m_Movement.Vertical = data.value;
            data.used = true;
        }
    }
}
