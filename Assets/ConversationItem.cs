using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConversationItem : MonoBehaviour
{
    Button Conversation_Button;
    TMP_Text Name_Text;
    TMP_Text Time_Text;
    TMP_Text Message_Text;
    DateTime startTime;
    ConversationType type;
    long conversationId;
    Action<ConversationType, long> callback;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init(Action<ConversationType, long> callback, ConversationType type, long conversationId, string name, long time, string message)
    { 
        Conversation_Button = transform.Find("Conversation_Button").GetComponent<Button>();
        Conversation_Button.onClick.AddListener(Enter);
        Name_Text = transform.Find("Name_Text").GetComponent<TMP_Text>();
        Time_Text = transform.Find("Time_Text").GetComponent<TMP_Text>();
        Message_Text = transform.Find("Message_Text").GetComponent<TMP_Text>();
        startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

        if (type == ConversationType.P2P)
            Name_Text.text = "P2P:"+name;
        else if (type == ConversationType.GROUP)
            Name_Text.text = "GROUP:"+name;
        else if (type == ConversationType.ROOM)
            Name_Text.text = "ROOM:"+name;
        if (time != 0)
            Time_Text.text = startTime.AddMilliseconds(time).ToString();
        else
            Time_Text.text = "";
        Message_Text.text = message;
        this.type = type;
        this.conversationId = conversationId;
        this.callback = callback;
    }

    void Enter()
    {
        Debug.Log("Enter");
        callback?.Invoke(type, conversationId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
