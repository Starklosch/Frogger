using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneCoordinator : MonoBehaviour
{
    Collider2D col;
    float lastSpawnTime = -10;

    [SerializeField] GameObject prefab;
    [SerializeField] float laneWidth = 10;
    [SerializeField] float maxEntities = 4;
    [SerializeField] float speed;
    [SerializeField] float spawnTime;
    [SerializeField] bool right = true;
    [SerializeField] LayerMask mask;

    public float Speed { get => speed; }
    public bool Right { get => Right; }

    int EntityCount
    {
        get
        {
            var pos = new Vector3(0, transform.position.y);
            var size = new Vector3(laneWidth, 1);
            var hits = Physics2D.BoxCastAll(pos, size, 0, Vector2.zero, 0, mask);
            return hits.Length;
        }
    }

    bool CanSpawn {
        get {
            var hit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.zero, 0, mask);
            if (hit)
            {
                lastSpawnTime = Time.time;
                return false;
            }
            if (EntityCount >= maxEntities)
                return false;

            return Time.time - lastSpawnTime > spawnTime;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        var pos = new Vector3(0, transform.position.y);
        var size = new Vector3(laneWidth, 1);
        Gizmos.DrawWireCube(pos, size);
    }

}
