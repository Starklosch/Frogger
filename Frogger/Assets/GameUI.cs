using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI t_score;
    [SerializeField] TextMeshProUGUI t_upScore;
    [SerializeField] TextMeshProUGUI t_lives;
    [SerializeField] TextMeshProUGUI t_time;
    [SerializeField] GameObject timeBackground;
    [SerializeField] Slider slider_time;
    [SerializeField] float timeShowTime;

    static GameUI instance;
    public static GameUI Instance { get => instance; }

    float lastShowTime;

    bool Hide { get => timeBackground.activeSelf && lastShowTime + timeShowTime < UnityEngine.Time.time; }

    public string Score { set => t_score.text = value; get => t_score.text; }
    public string UpScore { set => t_upScore.text = value; get => t_upScore.text; }
    public string Lives { set => t_lives.text = value; get => t_lives.text; }
    public string TimeText
    { 
        set
        {
            t_time.text = value;
            ShowTime();
        }
        get => t_time.text;
    }

    public float Time { set => slider_time.value = value; get => slider_time.value; }
    public float MaxTime { set => slider_time.maxValue = value; get => slider_time.maxValue; }

    void ShowTime()
    {
        timeBackground.SetActive(true);
        t_time.gameObject.SetActive(true);
        lastShowTime = UnityEngine.Time.time;
    }

    void HideTime()
    {
        timeBackground.SetActive(false);
        t_time.gameObject.SetActive(false);
    }

    void Start()
    {
        instance = this;
        HideTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (Hide)
            HideTime();
    }
}
