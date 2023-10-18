using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateTimerTest : MonoBehaviour
{
    [SerializeField] private ActionOnTimer actionOnTimer;
    private bool hasTimerElapse;

    void Start()
    {
        actionOnTimer.SetTimer(1f, ()=> { Debug.Log("Timer Complete"); });
    }

}
