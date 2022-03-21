using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleGroup : Environment
{
    [SerializeField] GameObject prefab;
    [SerializeField] float animSpeed;
    public int count;

    // Start is called before the first frame update
    protected override void Start()
    {
        GetComponent<BoxCollider2D>().size = new Vector2(count, 1);

        Vector2 position = transform.position;
        position += new Vector2(-count / 2.0f + 0.5f, 0);

        for (int i = 0; i < count; i++)
        {
            var turtle = Instantiate(prefab, position, Quaternion.identity, transform).GetComponent<Turtle>();
            turtle.group = this;
            turtle.animSpeedValue = animSpeed;

            position += Vector2.right;
        }

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
