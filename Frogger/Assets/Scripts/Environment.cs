using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : Entity
{
    public Spawner spawner;
    public bool right = true;
    public float speed = 1;
    
    // Start is called before the first frame update
    //protected override void Start()
    //{
    //    base.Start();

    //    if (!right)
    //        transform.localScale = new Vector2(-1, 1);
    //}

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (right)
            transform.position += Vector3.right * speed * Time.deltaTime;
        else
            transform.position += Vector3.left * speed * Time.deltaTime;
    }

    // Utils
    static protected SpriteRenderer CreateSprite(Sprite sprite = null)
    {
        var child = new GameObject("Sprite");
        var renderer = child.AddComponent<SpriteRenderer>();
        if (sprite == null)
            return renderer;

        renderer.sprite = sprite;
        return renderer;
    }

    protected void GenerateLong(int length, IList<Sprite> beginSprites, IList<Sprite> endSprites, IList<Sprite> middleSprites, out SpriteRenderer beginRenderer)
    {
        col.size = new Vector2(length, 1);
        beginRenderer = null;

        Vector2 position;
        if (right)
            position = new Vector2(-length / 2.0f + 0.5f, 0);
        else
            position = new Vector2(length / 2.0f - 0.5f, 0);

        for (int i = 0; i < length; i++)
        {
            var renderer = CreateSprite();

            // First
            if (i == 0)
            {
                renderer.sprite = endSprites.PickRandom();
            }
            // Last
            else if (i == length - 1)
            {
                renderer.sprite = beginSprites.PickRandom();
                beginRenderer = renderer;
            }
            else
            {
                renderer.sprite = middleSprites.PickRandom();
            }

            renderer.transform.position = transform.position;
            renderer.transform.SetParent(transform);
            renderer.transform.localPosition = position;
            if (right)
                position += Vector2.right;
            else
                position -= Vector2.right;
        }
    }
}
