using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogSprite : MonoBehaviour
{
    Frog frog;

    // Start is called before the first frame update
    void Start()
    {
        frog = GetComponentInParent<Frog>();
    }

    void MoveStarted()
    {
        frog.MoveStarted();
    }

    void MoveFinished()
    {
        frog.MoveFinished();
    }

    void DieEnd()
    {
        frog.DieEnd();
    }
}
