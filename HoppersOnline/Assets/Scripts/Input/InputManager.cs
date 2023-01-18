using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private Controls _inputs;

    void Start()
    {
        _inputs = new Controls();
    }
}
