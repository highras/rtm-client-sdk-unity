using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatItem : MonoBehaviour
{
    TMP_Text Name_Text;
    TMP_Text Time_Text;
    TMP_Text Message_Text;
    // Start is called before the first frame update
    DateTime startTime;
    void Start()
    {
    }

    public void Init(string name, long time, string message)
    {
        Name_Text = transform.Find("Name_Text").GetComponent<TMP_Text>();
        Time_Text = transform.Find("Time_Text").GetComponent<TMP_Text>();
        Message_Text = transform.Find("Message_Text").GetComponent<TMP_Text>();
        startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

        Name_Text.text = name;
        Time_Text.text = startTime.AddMilliseconds(time).ToString();
        Message_Text.text = message;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
