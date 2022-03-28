using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour
{
    public static DebugUI Instance;

    Text textComponent;
    Dictionary<string, string> text = new Dictionary<string, string>();

    // Start is called before the first frame update
    void Awake()
    {
        textComponent = GetComponent<Text>();
        Instance = this;
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    void UpdateText()
    {
        textComponent.text = "";
        foreach (var pair in text)
        {
            textComponent.text += pair.Key + ": " + pair.Value + "\n";
        }
    }

    public string this[string key]
    {
        set
        {
            text[key] = value;
            UpdateText();
        }
    }
}
