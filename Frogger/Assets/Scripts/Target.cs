using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : Entity
{
    int animCrocodile = Animator.StringToHash("Crocodile");
    int animButterfly = Animator.StringToHash("Butterfly");
    int animFrog = Animator.StringToHash("Frog");

    float lastTryTime = -10;
    float lastSpawnTime = -10;

    static int summonCount = 0;

    [SerializeField] bool hasFrog = false;

    public float cooldown = 5;
    public float spawnTryTime = .5f;
    private bool canSummonCrocodile = true;

    bool hasFly = false;
    bool CanTry { get => lastTryTime + spawnTryTime < Time.time; }
    bool CanSummon { get => !hasFrog /*&& summonCount < 3*/ && lastSpawnTime + cooldown < Time.time && CanTry; }
    public bool HasFrog { get => hasFrog; set => hasFrog = value; }
    public bool CanSummonCrocodile { get => canSummonCrocodile; set => canSummonCrocodile = value; }

    public delegate void TargetEvent(bool hasFly);
    public event TargetEvent FrogArrive;

    // Start is called before the first frame update
    protected override void OnStart()
    {
        summonCount = 0;

        SetupFrog();
    }

    // Update is called once per frame
    protected override void OnUpdate()
    {
        Summon();
    }

    public bool SetFrog()
    {
        if (hasFrog)
            return false;

        if (gameObject.layer != Constants.LAYER_GROUND)
            return false;

        hasFrog = true;
        SetupFrog();
        return true;
    }

    void SetupFrog()
    {
        if (!hasFrog)
            return;

        gameObject.layer = Constants.LAYER_WALL;
        anim.Play(animFrog);
        FrogArrive?.Invoke(hasFly);
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
            {
                hasFly = true;
                anim.Play(animButterfly);
            }
            else
                anim.Play(animCrocodile);

        }
        else if (choice > 70)
        {
            hasFly = true;
            anim.Play(animButterfly);
        }
        lastSpawnTime = Time.time;
        summonCount++;
    }

    void CrocoArrive()
    {
        gameObject.layer = Constants.LAYER_ENEMY;
    }

    void CrocoLeft()
    {
        gameObject.layer = Constants.LAYER_GROUND;
        summonCount--;
    }

    void ButteflyLeft()
    {
        gameObject.layer = Constants.LAYER_GROUND;
        summonCount--;
        hasFly = false;
        //animationRunning = false;
        //lastTime = Time.time;
    }

    //void AnimationEnd()
    //{
    //    animationRunning = false;
    //    lastTime = Time.time;
    //}


}
