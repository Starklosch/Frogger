using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Frog frog;
    [SerializeField] int maxBeats = 60;
    [SerializeField] float waitTime = 3;
    [SerializeField] float beatTime = .6f;
    [SerializeField] int startLives = 3;
    [SerializeField] GameObject gameOverScreen;

    int level = 1;
    int lives = 0;
    int score = 0;
    int targetCount = 0;
    int targetsReached = 0;

    int beats = 0;
    float startTime = 0;
    float lastBeatTime = 0;

    bool started = false;
    bool paused = false;

    Coroutine beatingCoroutine;

    //Home[] Home.Homes;

    int Lives
    {
        get => lives;
        set
        {
            lives = value;
            GameUI.Instance.Lives = lives.ToString();
        }
    }

    int Level
    {
        get => level;
        set
        {
            level = value;
            LevelChanged();
        }
    }

    int Score
    {
        get => score;
        set
        {
            score = value;
            GameUI.Instance.Score = Utils.FormatNumber(Score);
        }
    }

    //float ElapsedTime { get => Mathf.Clamp(Time.time - startTime, 0, maxTime); }
    bool HasTime { get => beats < maxBeats; }

    int Beats
    {
        get => beats;
        set
        {
            beats = value;
            GameUI.Instance.Beats = beats;
        }
    }

    bool Paused
    {
        get => paused;
        set
        {
            if (paused == value)
                return;

            paused = value;
            PauseChanged();
        }
    }

    public bool HasLives { get => lives > 0; }

    static GameManager instance;
    public static GameManager Instance { get => instance; }

    public event Action Pause;
    public event Action Resume;
    public event Action LevelCompleted;

    #region Monobehaviour
    void Start()
    {
        if (frog == null)
            frog = FindObjectOfType<Frog>();

        instance = this;

        gameOverScreen.SetActive(false);

        // Initialize
        GameUI.Instance.MaxBeats = maxBeats;
        RestartBeats();
        RestartLifes();
        RestartScore();
        RestartTargets();

        // Register events
        frog.Death += Frog_Death;
        frog.Move += Frog_Move;

        targetCount = Home.Homes.Length;

        startTime = Time.time;
        Time.timeScale = waitTime;
    }

    void Update()
    {
        if (!started)
        {
            if (Time.time > startTime + waitTime)
                FirstStart();
            return;
        }

        if (Paused)
            return;

        if (!HasLives)
        {
            Lose();
        }

        if (!HasTime)
        {
            GameUI.Instance.TimeText = "TIME OVER";
            frog.Die();
            return;
        }
    }
    #endregion

    #region Functions
    void Win()
    {
        //Restart(targets: true);
        //Level++;
        //LevelCompleted?.Invoke();
        Score += 1000;
        Paused = true;
        gameOverScreen.SetActive(true);
    }

    void Lose()
    {
        Paused = true;
        gameOverScreen.SetActive(true);
    }

    public void ShowBeats()
    {
        GameUI.Instance.TimeText = "TIME: " + (maxBeats - beats);
    }

    public void Replay()
    {
        SceneManager.LoadScene(0);
    }
    #endregion

    #region Misc
    void FirstStart()
    {
        started = true;
        Time.timeScale = 1;
        frog.CanMove = true;
        StartBeating();
    }

    void RestartBeats()
    {
        StopBeating();
        Beats = 0;
    }

    void RestartLifes()
    {
        Lives = startLives;
    }

    void RestartScore()
    {
        Score = 0;
    }

    void RestartTargets()
    {
        var homes = Home.Homes;
        if (homes == null)
            return;

        foreach (var target in homes)
        {
            target.SetEmpty(true);
        }
    }

    void StartBeating(bool restart = true)
    {
        StopBeating();
        if (restart)
            Beats = 0;
        beatingCoroutine = StartCoroutine(Beating());
    }

    void StopBeating()
    {
        if (beatingCoroutine != null)
            StopCoroutine(beatingCoroutine);
    }

    public void SubscribeHome(Home home)
    {
        home.FrogArrive += Target_FrogArrive;
    }
    #endregion

    #region Event Handlers
    private void Frog_Move()
    {
        Score += 10;
    }

    private void Target_FrogArrive(bool fly)
    {
        Score += 50;
        Score += (maxBeats - beats) * 10;
        if (fly)
        {
            Debug.Log("Fly");
            Score += 200;
        }

        if (Home.FrogCount >= targetCount)
        {
            Win();
        }

        ShowBeats();
        StartBeating();
    }

    private void Frog_Death()
    {
        Lives--;
        StartBeating();
    }
    #endregion

    IEnumerator Beating()
    {
        while (beats < maxBeats)
        {
            yield return new WaitForSeconds(beatTime);
            Beats++;
        }
        beatingCoroutine = null;
    }

    private void LevelChanged()
    {
        if (level == 1)
        {
            foreach (var item in Home.Homes)
            {
                item.CanSummonCrocodile = false;
            }
        }
        else if (level > 1)
        {
            foreach (var item in Home.Homes)
            {
                item.CanSummonCrocodile = true;
            }
        }

    }

    private void PauseChanged()
    {
        if (paused)
        {
            StopBeating();
            Pause?.Invoke();
        }
        else
        {
            Resume?.Invoke();
        }
    }
}
