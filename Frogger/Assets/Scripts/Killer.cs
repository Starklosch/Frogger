using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killer : MonoBehaviour
{
    Collider2D col;

    public Spawner spawner;
    public LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    public void Kill()
    {
        var hits = Physics2D.BoxCastAll(col.bounds.center, col.bounds.size, 0, Vector2.zero, 0, mask);
        foreach (var item in hits)
        {
            Destroy(item.transform.gameObject);
            spawner.entityCount--;
        }
    }
}
