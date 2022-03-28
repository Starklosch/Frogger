using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleGroup : SimpleEnemy
{
    [SerializeField] GameObject prefab;
    [SerializeField] float animSpeed;
    public int count;

    public bool canDive;

    protected override void OnStart()
    {
        base.OnStart();

        bool canDive = spawner.SpawnedCount % spawner.MaxEntities == spawner.RandomInt;

        var turtles = GetComponentsInChildren<Turtle>();
        foreach (var turtle in turtles)
        {
            turtle.group = this;
            turtle.animSpeedValue = animSpeed;
            turtle.canDive = canDive;
        }
    }
}
