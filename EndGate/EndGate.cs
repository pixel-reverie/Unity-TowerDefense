using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGate : MonoBehaviour
{
    public void OnTriggerEnter(Collider trigger)
    {
        Enemy enemy = trigger.GetComponent<Enemy>();

        if (!enemy) { return; }

        enemy.ReachGoal();
    }
}
