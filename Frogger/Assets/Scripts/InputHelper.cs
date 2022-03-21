using System.Collections.Generic;
using UnityEngine;

public class InputHelper : MonoBehaviour
{
    Dictionary<KeyCode, KeyInfo> savedKeys = new Dictionary<KeyCode, KeyInfo>();
    public float minKeyTimeDown = .2f;
    public float maxKeyTimeDown = .5f;

    List<KeyCode> keysToCheck = new List<KeyCode>();

    static InputHelper instance;
    public static InputHelper Instance { get => instance; }

    public delegate void KeyEvent(KeyCode key, KeyInfo info);

    public event KeyEvent KeyDown;
    public event KeyEvent KeyPressed;
    public event KeyEvent KeyUp;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        InputCheck();
    }

    public void CheckKey(KeyCode key)
    {
        keysToCheck.Add(key);
        savedKeys[key] = new KeyInfo();
    }

    void InputCheck()
    {
        foreach (var key in keysToCheck)
        {
            SaveKey(key);
        }
    }

    void SaveKey(KeyCode key)
    {
        KeyInfo keyInfo = savedKeys[key];


        if (Input.GetKeyDown(key))
        {
            keyInfo.state = KeyInfo.State.Down;
            keyInfo.timeDown = Time.time;
            keyInfo.holded = false;

            KeyDown?.Invoke(key, keyInfo);

            //DebugUI.Instance[key.ToString()] = "Down";
        }
        else if (Input.GetKey(key) && keyInfo.state != KeyInfo.State.Up)
        {
            //if (keyInfo.timeDown + minKeyTimeDown > Time.time)
            //    return;

            keyInfo.state = KeyInfo.State.Pressed;
            keyInfo.holded = true;

            KeyPressed?.Invoke(key, keyInfo);

            //DebugUI.Instance[key.ToString()] = "Pressed";
        }
        else
        {
            if (keyInfo.state != KeyInfo.State.Up)
            {
                keyInfo.timeUp = Time.time;
                keyInfo.state = KeyInfo.State.Up;
                KeyUp?.Invoke(key, keyInfo);
            }
            
            //DebugUI.Instance[key.ToString()] = "Up";
        }
    }

    public KeyInfo this[KeyCode code]
    {
        get
        {
            if (savedKeys.ContainsKey(code))
                return (KeyInfo)savedKeys[code].Clone();

            return null;
        }
    }
}

public class KeyInfo : System.ICloneable
{
    public enum State
    {
        Pressed,
        Down,
        Up
    }

    public State state;
    public float timeDown;
    public float timeUp;
    public bool holded = false;

    public KeyInfo()
    {
        state = State.Up;
        timeDown = -1;
        timeUp = -1;
    }

    public KeyInfo(State state, float timeDown, float timeUp, bool holded)
    {
        this.state = state;
        this.timeDown = timeDown;
        this.timeUp = timeUp;
        this.holded = holded;
    }

    public object Clone()
    {
        return new KeyInfo(state, timeDown, timeUp, holded);
    }

}