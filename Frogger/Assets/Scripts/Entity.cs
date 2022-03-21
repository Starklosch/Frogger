using System;
using UnityEngine;


public class Entity : MonoBehaviour
{
    protected Animator anim;
    protected BoxCollider2D col;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
    }

    protected virtual void Start()
    {
        GameManager.Instance.Pause += Pause;
        GameManager.Instance.Resume += Resume;

        OnStart();
    }

    protected virtual void Update()
    {
        OnUpdate();
    }

    protected virtual void OnDestroy()
    {
        GameManager.Instance.Pause -= Pause;
        GameManager.Instance.Resume -= Resume;
    }

    protected void Pause()
    {
        enabled = false;
        if (anim != null)
            anim.enabled = false;
    }

    protected void Resume()
    {
        enabled = true;
        if (anim != null)
            anim.enabled = true;
    }

    protected virtual void OnStart()
    {
    }
    protected virtual void OnUpdate()
    {
    }
}

