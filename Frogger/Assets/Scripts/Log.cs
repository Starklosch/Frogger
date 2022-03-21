using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Log : Environment
{
    int headSpriteIndex = 0;

    public int length = 1;
    public Sprite spritesEnd;
    public Sprite[] spriteBody;
    public Sprite spriteBegin;

    // Start is called before the first frame update
    protected override void Start()
    {
        if (length < 2)
            Debug.LogError("Too short");

        GenerateLong(length, spritesEnd.MakeList(), spriteBegin.MakeList(), spriteBody, out _);

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {

        base.Update();
    }
}
