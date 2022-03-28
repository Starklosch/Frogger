using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : Environment
{
    [SerializeField] SpriteMaskInteraction interaction;

    public SpriteMaskInteraction Interaction { get => interaction; set => interaction = value; }

    protected override void OnStart()
    {
        var renderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var ren in renderers)
        {
            ren.maskInteraction = interaction;
        }
    }
}
