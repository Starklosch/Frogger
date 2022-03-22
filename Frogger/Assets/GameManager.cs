using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Frog frog;
    [SerializeField] float maxTime = 60;
    [SerializeField] float waitTime = 3;
    [SerializeField] int startLives = 3;
    [SerializeField] GameObject gameOverScreen;

    int level = 1;
    int Level
    {
        get => level;
        set
        {
            level = value;
            LevelChanged();
        }
    }

    int lives;
    int score = 0;
    int targetCount = 0;
    int targetsReached = 0;
    int TargetsReached
    {
        get => targetsReached;
        set
        {
            targetsReached = value;
            if (targetsReached >= targetCount)
            {
                Win();
            }
        }
    }

    int Score
    {
        get => score;
        set
        {
            score = value;
            GameUI.Instance.Score = FormatNumber(Score);
        }
    }

    Target[] targets;

    float startTime = 0;
    float ElapsedTime { get => Mathf.Clamp(Time.time - startTime, 0, maxTime); }
    bool HasTime { get => ElapsedTime < maxTime; }

    bool started = false;
    bool paused = false;
    bool Paused
    {
        get => paused;
        set
        {
            paused = value;
            if (paused)
                Pause?.Invoke();
            else
                Resume?.Invoke();
        }
    }

    public bool HasLives { get => lives > 0; }

    static GameManager instance;
    public static GameManager Instance { get => instance; }

    public event Action Pause;
    public event Action Resume;
    public event Action LevelCompleted;

    // Start is called before the first frame update
    void Start()
    {
        if (frog == null)
            frog = FindObjectOfType<Frog>();

        instance = this;
        lives = startLives;

        Score = 0;
        GameUI.Instance.MaxTime = maxTime;
        GameUI.Instance.Lives = lives.ToString();

        frog.Death += Frog_Death;
        frog.Move += Frog_Move;

        targets = FindObjectsOfType<Target>();
        foreach (var target in targets)
        {
            target.FrogArrive += Target_FrogArrive;
        }
        targetCount = targets.Length;

        gameOverScreen.SetActive(false);

        startTime = Time.time;
        Time.timeScale = waitTime * 2;
    }

    private void Frog_Move()
    {
        Score += 10;
    }

    private void Target_FrogArrive(bool fly)
    {
        TargetsReached++;
        Score += 50;
        Score += (int)(maxTime - ElapsedTime) * 10;
        if (fly)
        {
            Debug.Log("FLy");
            Score += 200;
        }

        ShowTime();
        Restart();
    }

    private void Frog_Death()
    {
        lives--;
        GameUI.Instance.Lives = lives.ToString();
    }

    void FirstStart()
    {
        started = true;
        Time.timeScale = 1;
        frog.CanMove = true;
        Restart();
    }

    void Restart(bool lifes = false, bool score = false, bool targets = false)
    {
        startTime = Time.time;
        if (lifes)
            lives = startLives;
        if (score)
            this.Score = 0;
        if (targets)
        {
            foreach (var target in this.targets)
            {
                target.HasFrog = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!started)
        {
            if (Time.time > startTime + waitTime)
                FirstStart();
            return;
        }

        if (!HasLives)
        {
            Lose();
        }

        if (Paused)
            return;

        if (!HasTime)
        {
            GameUI.Instance.TimeText = "TIME OVER";
            Paused = true;
            return;
        }
        GameUI.Instance.Time = ElapsedTime;
    }

    public void ShowTime()
    {
        GameUI.Instance.TimeText = "TIME: " + (int)(maxTime - ElapsedTime);
    }

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

    public void Replay()
    {
        SceneManager.LoadScene(0);
    }

    string FormatNumber(int n)
    {
        var text = n.ToString();
        if (text.Length == 0)
        {
            var zeros = new string('0', 8);
            text = "<color=#E7002F>" + zeros;
        }
        else if (text.Length < 8)
        {
            var zeros = new string('0', 8 - text.Length);
            text = "<color=#E7002F>" + zeros + "<color=white>" + text;
        }
        return text;
    }

    private void LevelChanged()
    {
        if (level == 1)
        {
            foreach (var item in targets)
            {
                item.CanSummonCrocodile = false;
            }
        }
        else if (level > 1)
        {
            foreach (var item in targets)
            {
                item.CanSummonCrocodile = true;
            }
        }

    }
}
