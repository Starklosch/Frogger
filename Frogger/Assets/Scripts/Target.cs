using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    const int groundLayer = 8;
    const int enemyLayer = 9;

    Animator anim;
    int animCrocodile = Animator.StringToHash("Crocodile");
    int animButterfly = Animator.StringToHash("Butterfly");

    float lastTryTime = -10;
    float lastSpawnTime = -10;

    static int summonCount = 0;

    public float cooldown = 5;
    public float spawnTryTime = .5f;
    public bool canSummonCrocodile = false;

    bool CanTry { get => lastTryTime + spawnTryTime < Time.time; }
    bool CanSummon { get => summonCount < 2 && CanTry && lastSpawnTime + cooldown < Time.time; }

    // Start is called before the first frame update
    void Start()
    {
        summonCount = 0;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Summon();
    }

    void Summon()
    {
        //Debug.Log(summonCount);
        if (!CanSummon)
            return;

        var choice = Random.Range(0, 100);
        lastTryTime = Time.time;
        if (canSummonCrocodile)
        {
            if (choice < 55)
                return;

            if (choice > 80)
                anim.Play(animButterfly);
            else
                anim.Play(animCrocodile);

        }
        else if (choice > 70)
        {
            anim.Play(animButterfly);
        }
        lastSpawnTime = Time.time;
        summonCount++;
    }

    void CrocoArrive()
    {
        gameObject.layer = enemyLayer;
    }

    void CrocoLeft()
    {
        gameObject.layer = groundLayer;
        summonCount--;
    }

    void ButteflyLeft()
    {
        gameObject.layer = groundLayer;
        summonCount--;
        //animationRunning = false;
        //lastTime = Time.time;
    }

    //void AnimationEnd()
    //{
    //    animationRunning = false;
    //    lastTime = Time.time;
    //}


}
