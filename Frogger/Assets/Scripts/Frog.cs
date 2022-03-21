using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Frog : MonoBehaviour
{

    Animator anim;

    bool alive = true;

    // Animation
    int animMoveUp = Animator.StringToHash("Move_Up");
    int animMoveDown = Animator.StringToHash("Move_Down");
    int animMoveRight = Animator.StringToHash("Move_Right");
    int animMoveLeft = Animator.StringToHash("Move_Left");
    int animDie = Animator.StringToHash("Die");

    int animSpeed = Animator.StringToHash("Speed");

    // Movement
    bool moving = false;
    Vector2 polledMovement;
    Vector3 direction;
    float polledMovementNextTime = 0;

    bool onMovingSurface = false;

    public float minKeyHoldTimeToRepeat = 1;

    public float speed = 2;
    public float movementPoolingTime = .5f;

    // Masks
    public LayerMask groundMask;
    public LayerMask enemyMask;
    public string movingSurfaceTag;

    public Vector2 colliderSize = Vector2.one * .8f;

    public bool CanPoll { get => polledMovementNextTime > Time.time - movementPoolingTime; }
    public bool PressedEnoughTime { get => polledMovementNextTime > Time.time - movementPoolingTime; }


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetFloat(animSpeed, speed);

        if (enemyMask.value == 0)
            Debug.LogWarning("enemyMask unset");
        if (groundMask.value == 0)
            Debug.LogWarning("groundMask unset");

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
    void Update()
    {
        if (!alive)
            return;

        Movement();

        if (onMovingSurface)
            GroundChecking();

        if (Input.GetKeyDown(KeyCode.E))
        {
            var newPos = transform.position;
            newPos.x = Mathf.Round(newPos.x);
            newPos.y = Mathf.Round(newPos.y);
            transform.position = newPos;
        }
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

    void GroundCheckingPre()
    {
        var groundHit = Physics2D.BoxCast(transform.position + direction, colliderSize, 0, Vector2.zero, 0, groundMask);

        if (!groundHit || groundHit.transform.tag != movingSurfaceTag)
        {
            transform.SetParent(null);
            onMovingSurface = false;
        }
    }

    void GroundChecking()
    {
        var groundHit = Physics2D.BoxCast(transform.position, colliderSize, 0, Vector2.zero, 0, groundMask);

        if (groundHit && groundHit.transform.tag == movingSurfaceTag)
        {
            transform.SetParent(groundHit.transform);
            onMovingSurface = true;
        }

        if (!groundHit)
        {
            alive = false;
            anim.Play(animDie);
        }
    }

    void EnemyChecking()
    {
        var enemyHit = Physics2D.BoxCast(transform.position, colliderSize, 0, Vector2.zero, 0, enemyMask);
        if (enemyHit)
        {
            alive = false;
            anim.Play(animDie);
        }
    }

    void Movement()
    {
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

        Move(currentMovement);
    }

    void Move(Vector2 direction)
    {
        if (direction == Vector2.zero)
            return;

        // Position correction
        //var newPos = transform.position;
        //newPos.x = Mathf.Round(newPos.x);
        //newPos.y = Mathf.Round(newPos.y);
        //transform.position = newPos;

        //var hit = Physics2D.BoxCast(transform.position + Vector3.up, groundSensivity, 0, Vector2.zero, 0, groundMask);
        if (direction.y > 0)
        {
            anim.Play(animMoveUp, -1, 0);
            this.direction = Vector3.up;
        }
        else if (direction.y < 0)
        {
            anim.Play(animMoveDown, -1, 0);
            this.direction = Vector3.down;
        }
        else if (direction.x > 0)
        {
            anim.Play(animMoveRight, -1, 0);
            this.direction = Vector3.right;
        }
        else if (direction.x < 0)
        {
            anim.Play(animMoveLeft, -1, 0);
            this.direction = Vector3.left;
        }
    }

    void MoveStarted()
    {
        moving = true;
        GroundCheckingPre();
    }

    void MoveFinished()
    {
        moving = false;
        GroundChecking();
        EnemyChecking();
    }

    private void OnDrawGizmos()
    {
        //var hit = Physics2D.BoxCast(transform.position, colliderSize, 0, Vector2.zero, 0, groundMask);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, colliderSize);
    }
}
