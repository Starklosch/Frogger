using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : Entity
{
    const int defaultLayer = 0;
    const int groundLayer = 8;

    int diveAnim = Animator.StringToHash("Dive");
    int animSpeed = Animator.StringToHash("Speed");

    public bool canDive;
    public TurtleGroup group;
    public float animSpeedValue;

    // Start is called before the first frame update
    protected override void OnStart()
    {
        anim.SetFloat(animSpeed, animSpeedValue);

        if (canDive)
            anim.Play(diveAnim);
    }

    void Ground()
    {
        gameObject.layer = groundLayer;
    }
    void Unground()
    {
        gameObject.layer = defaultLayer;
    }
}
