using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongCrocodile : Environment
{
    SpriteRenderer head;
    int headSpriteIndex = 0;
    float headNextTime = 0;

    public int length = 1;
    public Sprite[] spritesHead;
    public Sprite spriteBody;
    public Sprite spriteTail;

    public float headAnimationTime = 1;
    public int enemyLayer;

    bool CanAnimateHead { get => headNextTime < Time.time; }

    // Start is called before the first frame update
    protected override void OnStart()
    {
        if (length < 2)
            Debug.LogError("Too short");

        GenerateLong(length, spritesHead, spriteTail.MakeList(), spriteBody.MakeList(), out head);
        head.gameObject.layer = enemyLayer;
        head.gameObject.AddComponent<BoxCollider2D>();
    }


    protected override void OnUpdate()
    {
        // Change head
        if (CanAnimateHead)
        {
            head.sprite = spritesHead[headSpriteIndex++];
            headSpriteIndex %= spritesHead.Length;

            headNextTime = Time.time + headAnimationTime;
        }
    }
}
