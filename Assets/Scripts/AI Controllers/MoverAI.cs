using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

sealed public class MoverAI : AgentAI
{
    public void MoveTo (Transform t)
    {
        if (t == null)
            return;

        MoveTo(t.position);
    }

    public void MoveTo (Vector3 destination)
    {
        SetDestination(destination);
    }
}
