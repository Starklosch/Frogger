using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private const float correction = -.1f;
    BoxCollider2D col;
    Killer killer;
    float lastSpawnTime = -10;
    float spawnTime;
    int spawnedCount = 0;
    public int SpawnedCount => spawnedCount;

    int randomNumber;
    public int RandomInt => randomNumber;

    bool right = true;
    [SerializeField] GameObject prefab;
    [SerializeField] float laneWidth = 10;
    [SerializeField] float laneHeight = .8f;
    [SerializeField] float maxEntities = 4;
    [SerializeField] float baseSpeed = 1, maxSpeedMultiplier = 2;
    [SerializeField] float spawnTimeVariation = .2f;
    [SerializeField] LayerMask mask;

    public float MaxEntities => maxEntities;

    float SpeedMultiplier => Random.Range(1, maxSpeedMultiplier);

    float Speed => baseSpeed * SpeedMultiplier;
    public int entityCount = 0;

    //int EntityCount
    //{
    //    get
    //    {
    //        var pos = new Vector3(0, transform.position.y);
    //        var size = new Vector3(laneWidth, laneHeight);
    //        var hits = Physics2D.BoxCastAll(pos, size, 0, Vector2.zero, 0, mask);
    //        return hits.Length;
    //    }
    //}

    bool CanSpawn {
        get {
            var hit = Physics2D.BoxCast(col.bounds.center, col.size, 0, Vector2.zero, 0, mask);
            if (hit)
            {
                lastSpawnTime = Time.time - spawnTime / 5;
                return false;
            }
            if (entityCount > maxEntities)
                return false;

            return Time.time - lastSpawnTime > spawnTime;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        entityCount = 0;

        killer = GetComponentInChildren<Killer>();
        killer.mask = mask;
        killer.spawner = this;
        killer.transform.position = new Vector3(-transform.position.x, transform.position.y);
        
        if (transform.localScale.x < 0)
            right = false;

        col.size = new Vector2(col.size.x, laneHeight);
        var kilCol = killer.GetComponent<BoxCollider2D>();
        kilCol.size = new Vector2(kilCol.size.x, laneHeight);

        randomNumber = Random.Range(0, (int)maxEntities);

        GameManager.Instance.LevelCompleted += LevelCompleted;
    }

    private void LevelCompleted()
    {
        spawnedCount = 0;
        randomNumber = Random.Range(0, (int)maxEntities);
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSpawn)
        {
            Spawn();
        }

        if (transform.childCount > 1)
            killer.Kill();
    }

    void Spawn()
    {
        var ent = Instantiate(prefab, transform.position, Quaternion.identity, transform).GetComponent<Environment>();
        ent.speed = Speed;
        ent.right = right;
        ent.spawner = this;
        ent.transform.SetParent(transform);

        float time = laneWidth / baseSpeed / maxEntities;
        spawnTime = time + Random.Range(-spawnTimeVariation, spawnTimeVariation);
        lastSpawnTime = Time.time;
        entityCount++;
        spawnedCount++;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var pos = new Vector3(0, transform.position.y);
        var size = new Vector3(laneWidth, laneHeight);
        Gizmos.DrawWireCube(pos, size);
    }

}
