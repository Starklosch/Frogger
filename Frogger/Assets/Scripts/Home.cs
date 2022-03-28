using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : Entity
{
    public enum Status
    {
        Empty,
        Crocodile,
        Butterfly,
        Frog
    }

    int animCrocodile = Animator.StringToHash("Crocodile");
    int animButterfly = Animator.StringToHash("Butterfly");
    int animFrog = Animator.StringToHash("Frog");
    int animNone = Animator.StringToHash("None");

    float lastTryTime = -10;
    float lastSpawnTime = -10;

    public float cooldown = 5;
    public float spawnTryTime = .5f;
    bool canSummonCrocodile = true;

    Status status;

    public static Home[] Homes => FindObjectsOfType<Home>();

    public static int SummonCount
    {
        get
        {
            int count = 0;
            foreach (var home in Homes)
            {
                if (home.HasSomething)
                    count++;
            }
            return count;
        }
    }

    public static int ButterflyCount
    {
        get
        {
            int count = 0;
            foreach (var home in Homes)
            {
                if (home.HasButterfly)
                    count++;
            }
            return count;
        }
    }

    public static int CrocodileCount
    {
        get
        {
            int count = 0;
            foreach (var home in Homes)
            {
                if (home.HasCrocodile)
                    count++;
            }
            return count;
        }
    }

    public static int FrogCount
    {
        get
        {
            int count = 0;
            foreach (var home in Homes)
            {
                if (home.HasFrog)
                    count++;
            }
            return count;
        }
    }

    #region Checks
    public bool HasSomething => status != Status.Empty && status != Status.Frog;
    public bool HasFrog => status == Status.Frog;
    public bool HasButterfly => status == Status.Butterfly;
    public bool HasCrocodile => status == Status.Crocodile;
    public bool IsGround => gameObject.layer == Constants.LAYER_GROUND;

    bool CanTry { get => lastTryTime + spawnTryTime < Time.time; }
    bool CanSummon { get => !HasFrog && SummonCount < 3 && lastSpawnTime + cooldown < Time.time && CanTry; }
    public bool CanSummonCrocodile { get => canSummonCrocodile; set => canSummonCrocodile = value; }
    #endregion

    public delegate void TargetEvent(bool hasFly);
    public event TargetEvent FrogArrive;

    protected override void OnStart()
    {
        status = Status.Empty;
        GameManager.Instance.SubscribeHome(this);

        SetupFrog();
    }

    protected override void OnUpdate()
    {
        Summon();
    }

    void Summon()
    {
        if (!CanSummon)
            return;

        var choice = Random.Range(0, 100);
        lastTryTime = Time.time;
        if (CanSummonCrocodile)
        {
            if (choice < 55)
                return;

            if (choice > 80)
            {
                if (ButterflyCount == 0)
                    SetButterfly();
            }
            else
                if (CrocodileCount < 2)
                    SetCrocodile();
        }
        else
        {
            if (choice < 70)
                return;

            SetButterfly();
        }
    }

    void SetupFrog()
    {
        if (!HasFrog)
            return;

        gameObject.layer = Constants.LAYER_WALL;
        anim.Play(animFrog);
    }

    #region Setters
    public bool SetFrog()
    {
        if (HasFrog || !IsGround)
            return false;

        status = Status.Frog;
        SetupFrog();
        FrogArrive?.Invoke(HasButterfly);
        return true;
    }

    bool SetButterfly()
    {
        if (HasFrog)
            return false;

        lastSpawnTime = Time.time;
        status = Status.Butterfly;
        anim.Play(animButterfly);
        return true;
    }

    bool SetCrocodile()
    {
        if (HasFrog)
            return false;

        lastSpawnTime = Time.time;
        status = Status.Crocodile;
        anim.Play(animCrocodile);
        return true;
    }

    public void SetEmpty(bool now = false)
    {

        if (now)
            anim.Play(animNone);

        status = Status.Empty;
        gameObject.layer = Constants.LAYER_GROUND;
    }
    #endregion

    void CrocoArrive()
    {
        gameObject.layer = Constants.LAYER_ENEMY;
    }

    void CrocoLeft()
    {
        SetEmpty();
    }

    void ButteflyLeft()
    {
        SetEmpty();
    }

}
