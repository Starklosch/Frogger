using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Frog : Entity
{
    FrogSprite sprite;

    // Animation
    int animMoveUp = Animator.StringToHash("Move_Up");
    int animMoveDown = Animator.StringToHash("Move_Down");
    int animMoveRight = Animator.StringToHash("Move_Right");
    int animMoveLeft = Animator.StringToHash("Move_Left");
    int animDie = Animator.StringToHash("Die");
    int animIdle = Animator.StringToHash("Idle");

    int animSpeed = Animator.StringToHash("Speed");

    // Movement
    bool moving = false;
    Vector2 polledMovement;
    Vector3 direction;
    float polledMovementNextTime = 0;

    bool canMove = false;
    bool onMovingSurface = false;
    bool shouldRespawn = true;
    bool death = false;

    public float minKeyHoldTimeToRepeat = 1;

    public float speed = 2;
    public float movementPoolingTime = .5f;

    [SerializeField] Vector3 spawnPoint;
    [SerializeField] Vector2 colliderSize = Vector2.one * .8f;

    public bool CanPoll { get => polledMovementNextTime > Time.time - movementPoolingTime; }
    public bool PressedEnoughTime { get => polledMovementNextTime > Time.time - movementPoolingTime; }
    public bool CanMove { get => canMove; set => canMove = value; }

    public event System.Action Death;
    public event System.Action Move;

    // Start is called before the first frame update
    protected override void OnStart()
    {
        anim = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<FrogSprite>();

        anim.SetFloat(animSpeed, speed);

        if (string.IsNullOrWhiteSpace(Constants.goalTag))
            Debug.LogWarning("goalTag unset");
        if (string.IsNullOrWhiteSpace(Constants.movingSurfaceTag))
            Debug.LogWarning("movingSurfaceTag unset");

        InputHelper.Instance.CheckKey(KeyCode.W);
        InputHelper.Instance.CheckKey(KeyCode.S);
        InputHelper.Instance.CheckKey(KeyCode.A);
        InputHelper.Instance.CheckKey(KeyCode.D);

        InputHelper.Instance.KeyUp += KeyUp;
    }

    private void KeyUp(KeyCode key, KeyInfo info)
    {
        // Clear polled movement
        if (info.holded && (info.timeDown + minKeyHoldTimeToRepeat < Time.time))
        {
            //Debug.Log("Cleared");
            polledMovement = Vector2.zero;
        }
    }

    // Update is called once per frame
    protected override void OnUpdate()
    {
        //if (!GameManager.Instance.HasLives)
        //    return;

        Respawn();

        if (death)
            return;

        Movement();

        if (!moving)
        {
            GroundChecking();
            EnemyChecking();
        }
    }

    void Respawn()
    {
        if (!shouldRespawn)
            return;

        transform.position = spawnPoint;
        anim.Play(animIdle);

        shouldRespawn = false;
        death = false;
    }

    bool KeyCheck(KeyCode code)
    {
        if (InputHelper.Instance == null)
            return false;

        var key = InputHelper.Instance[code];
        if (key == null)
            return false;

        if (key.state == KeyInfo.State.Down)
            return true;

        var pressedEnoughTime = key.timeDown + minKeyHoldTimeToRepeat < Time.time;
        if (key.state == KeyInfo.State.Pressed)
        {
            return pressedEnoughTime;
        }

        return false;
    }

    #region Checks
    bool WallChecking()
    {
        var wallHit = Physics2D.BoxCast(transform.position + direction, colliderSize, 0, Vector2.zero, 0, Constants.WallMask);
        if (wallHit)
            return false;

        return true;
    }

    void GroundCheckingPre()
    {
        var groundHit = Physics2D.BoxCast(transform.position + direction, colliderSize, 0, Vector2.zero, 0, Constants.GroundMask);

        if (!groundHit || groundHit.transform.tag != Constants.movingSurfaceTag)
        {
            transform.SetParent(null);
            onMovingSurface = false;
        }
    }

    // True if ground found
    bool GroundChecking()
    {
        var groundHit = Physics2D.BoxCast(transform.position, colliderSize, 0, Vector2.zero, 0, Constants.GroundMask);

        if (groundHit && groundHit.transform.tag == Constants.movingSurfaceTag)
        {
            transform.SetParent(groundHit.transform);
            onMovingSurface = true;
        }
        else
        {
            transform.SetParent(null);
            onMovingSurface = false;
        }

        if (groundHit && groundHit.transform.tag == Constants.goalTag)
        {
            var target = groundHit.transform.GetComponent<Target>();
            if (target.SetFrog())
            {
                shouldRespawn = true;
            }
        }

        if (!groundHit)
        {
            Die();
            return false;
        }

        return true;
    }

    // True if died
    bool EnemyChecking()
    {
        var enemyHit = Physics2D.BoxCast(transform.position, colliderSize, 0, Vector2.zero, 0, Constants.EnemyMask);
        if (enemyHit)
        {
            Die();
            return true;
        }
        return false;
    }
    #endregion

    void Die()
    {
        transform.SetParent(null);
        onMovingSurface = false;

        anim.Play(animDie);
        death = true;
    }

    void Movement()
    {
        if (!canMove)
            return;

        Vector2 registeredMovement = Vector2.zero;

        // Handle input
        if (KeyCheck(KeyCode.W))
            registeredMovement.y++;
        if (KeyCheck(KeyCode.S))
            registeredMovement.y--;
        if (KeyCheck(KeyCode.D))
            registeredMovement.x++;
        if (KeyCheck(KeyCode.A))
            registeredMovement.x--;

        Vector2 currentMovement = registeredMovement;
        var registeredIsZero = registeredMovement == Vector2.zero;

        // Don't do anything if moving
        if (moving)
        {
            // Pool movement
            if (!registeredIsZero)
            {
                polledMovement = registeredMovement;
                polledMovementNextTime = Time.time;
            }
            return;
        }

        // Make pooled movement and save actual one
        if (!registeredIsZero)
        {
            polledMovement = Vector2.zero;
            //Debug.Log("Using registered");
        }
        else if (polledMovement != Vector2.zero && CanPoll)
        {
            currentMovement = polledMovement;
            //Debug.Log("Using polled");

            polledMovement = registeredMovement;
            polledMovementNextTime = Time.time;
        }

        MakeMovement(currentMovement);
    }

    void MakeMovement(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;

        // Position correction
        //var newPos = transform.position;
        //newPos.x = Mathf.Round(newPos.x);
        //newPos.y = Mathf.Round(newPos.y);
        //transform.position = newPos;
        GroundCheckingPre();

        if (direction.y > 0)
        {
            this.direction = Vector3.up;
            if (!WallChecking())
                return;

            anim.Play(animMoveUp, -1, 0);
        }
        else if (direction.y < 0)
        {
            this.direction = Vector3.down;
            if (!WallChecking())
                return;

            anim.Play(animMoveDown, -1, 0);
        }
        else if (direction.x > 0)
        {
            this.direction = Vector3.right;
            if (!WallChecking())
                return;

            anim.Play(animMoveRight, -1, 0);
        }
        else if (direction.x < 0)
        {
            this.direction = Vector3.left;
            if (!WallChecking())
                return;

            anim.Play(animMoveLeft, -1, 0);
        }
    }

    #region Events
    public void MoveStarted()
    {
        moving = true;
    }

    public void MoveFinished()
    {
        moving = false;

        sprite.transform.SetParent(null);
        transform.position = sprite.transform.position;
        sprite.transform.SetParent(transform);


        var death = !GroundChecking();
        death |= EnemyChecking();
        if (!death)
            Move?.Invoke();
    }

    public void DieEnd()
    {
        shouldRespawn = true;
        Death?.Invoke();
    }
    #endregion

    private void OnDrawGizmos()
    {
        //var hit = Physics2D.BoxCast(transform.position, colliderSize, 0, Vector2.zero, 0, Constants.groundMask);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(spawnPoint, Vector3.one);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, colliderSize);
    }
}
