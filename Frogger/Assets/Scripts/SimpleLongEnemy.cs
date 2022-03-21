using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLongEnemy : Environment
{
    [SerializeField] int length = 1;

    [SerializeField] Sprite[] lengthOneSprites;
    [SerializeField] Sprite spriteLong_Begin;
    [SerializeField] Sprite spriteLong_Middle;
    [SerializeField] Sprite spriteLong_End;

    protected override void OnStart()
    {
        if (length == 1)
        {
            var renderer = CreateSprite(lengthOneSprites.PickRandom());
            renderer.transform.position = transform.position;
            renderer.transform.SetParent(transform);
        }
        else
        {
            GenerateLong(length, spriteLong_Begin.MakeList(), spriteLong_End.MakeList(), spriteLong_Middle.MakeList(), out _);
        }
    }


    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawCube(transform.position, new Vector3(length - 0.2f, 0.8f));
    //}
    
}
