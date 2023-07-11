using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.fpnn;
using com.fpnn.rtm;
using TMPro;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

public class ChatDemoQuestProcessor : RTMQuestProcessor
{
    public override void SessionClosed(int ClosedByErrorCode)
    {
        Debug.Log("SessionClosed errorCode = " + ClosedByErrorCode);
    }

    public override bool ReloginWillStart(int lastErrorCode, int retriedCount)
    {
        Debug.Log("ReloginWillStart lastErrorCode = " + lastErrorCode + ", retriedCount = " + retriedCount);
        return true;
    }

    public override void Kickout()
    {
        Debug.Log("Kickout");
    }

    public override void KickoutRoom(long roomId)
    {
        Debug.Log("KickoutRoom roomId = " + roomId);
    }
}

[Serializable]
public class ChatDemoMessage : IComparable<ChatDemoMessage>
{
    public ChatDemoMessage(long messageId, long fromUid, string message, long mtime, long cursorId = 0)
    {
        this.messageId = messageId;
        this.fromUid = fromUid;
        this.message = message;
        this.mtime = mtime;
        this.cursorId = cursorId;
    }
    public static ChatDemoMessage ConvertoFromRTMMessage(RTMMessage message)
    {
        ChatDemoMessage chatDemoMessage = new ChatDemoMessage(message.messageId, message.fromUid, message.stringMessage, message.modifiedTime);
        return chatDemoMessage;
    }

    public static ChatDemoMessage ConvertoFromHistoryMessage(HistoryMessage message)
    {
        ChatDemoMessage chatDemoMessage = new ChatDemoMessage(message.messageId, message.fromUid, message.stringMessage, message.modifiedTime, message.cursorId);
        return chatDemoMessage;
    }

    public int CompareTo(ChatDemoMessage message)
    {
        if (mtime > message.mtime)
            return 1;
        else if (mtime < message.mtime)
            return -1;
        else
        {
            if (messageId >= message.messageId)
                return 1;
            else
                return -1;
        }
    }
    //public MessageCategory type;
    public long messageId;
    public long fromUid;
    public string message;
    public long mtime;
    public long cursorId;
}

public class ChatDemoMessageCompare: IEqualityComparer<ChatDemoMessage>
{ 
    public bool Equals(ChatDemoMessage msg1, ChatDemoMessage msg2)
    {
        if (msg1.mtime == msg2.mtime && msg1.messageId == msg2.messageId)
        {
            if (msg1.cursorId == msg2.cursorId)
                return true;
            else if ((msg1.cursorId == 0 && msg2.cursorId != 0) || (msg1.cursorId != 0 && msg2.cursorId == 0))
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public int GetHashCode(ChatDemoMessage msg)
    {
        if (msg == null)
            return 0;
        else
            return string.Format("{0}_{1}_{2}", msg.mtime, msg.messageId, msg.message).GetHashCode();
    }
}

[Serializable]
public class ChatDemoConversation
{
    public ConversationType type;
    public Int64 id;
    public Int32 unreadCount;
    public ChatDemoMessage lastMessage;
    public void AddMessage(ChatDemoMessage message)
    {
        if (lastMessage == null)
        {
            lastMessage = message;
            return;
        }
        if (message.mtime > lastMessage.mtime)
        {
            lastMessage = message;
            return;
        }
        else if (message.mtime == lastMessage.mtime && lastMessage.messageId > message.messageId)
        {
            lastMessage = message;
            return;
        }
    }
}
[Serializable]
public class ChatDemoConversationList
{
    public List<ChatDemoConversation> list;
    public Int64 timestamp;
    public Int64 lastMessageTime;
}

public class ChatDemoChat
{
    public ConversationType type;
    public Int64 id;
    public Int64 lastId;
    public List<ChatDemoMessage> messageList;
}

public class ChatDemo : MonoBehaviour
{
    private string rtmServerEndpoint = "rtm-nx-front.ilivedata.com:13321";
    private long pid = 11000002;
    private long worldRoomId = 1000;
 
    static RTMClient client;
    ChatDemoConversationList conversationList = new ChatDemoConversationList();
    List<ChatDemoChat> chatList = new List<ChatDemoChat>();
    List<GameObject> conversationGameObjectList = new List<GameObject>();
    List<GameObject> chatGameObjectList = new List<GameObject>();


    TMP_InputField UID_InputField;
    Button Login_Button;
    TMP_InputField Message_InputField;
    Button Send_Button;
    Button Delete_Button;
    Button NextPage_Button;
    GameObject Conversation_Content;
    GameObject Chat_Content;
    GameObject conversationItemPrefab;
    GameObject chatItemPrefab;

    // Start is called before the first frame update
    void Start()
    {
        ClientEngine.Init();
        RTMControlCenter.Init();

        UID_InputField = GameObject.Find("UID_InputField").GetComponent<TMP_InputField>();
        Login_Button = GameObject.Find("Login_Button").GetComponent<Button>();
        Login_Button.onClick.AddListener(Login);
        Message_InputField = GameObject.Find("Message_InputField").GetComponent<TMP_InputField>();
        Send_Button = GameObject.Find("Send_Button").GetComponent<Button>();
        Send_Button.onClick.AddListener(Send);
        Delete_Button = GameObject.Find("Delete_Button").GetComponent<Button>();
        Delete_Button.onClick.AddListener(Delete);
        NextPage_Button = GameObject.Find("NextPage_Button").GetComponent<Button>();
        NextPage_Button.onClick.AddListener(NextPage);
        Conversation_Content = GameObject.Find("Conversation_ScrollView/Viewport/Content");
        Chat_Content = GameObject.Find("Chat_ScrollView/Viewport/Content");
        conversationItemPrefab = Resources.Load("Conversation_Panel") as GameObject;
        chatItemPrefab = Resources.Load("Chat_Panel") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            Save();
    }

    private void OnApplicationQuit()
    {
        Save();
        RTMControlCenter.Close();
        ClientEngine.Close();
    }

    void OnPushP2PChat(RTMMessage message)
    {
        AddMessage(ConversationType.P2P, message.fromUid, ChatDemoMessage.ConvertoFromRTMMessage(message));
    }

    void OnPushGroupChat(RTMMessage message)
    { 
        AddMessage(ConversationType.GROUP, message.toId, ChatDemoMessage.ConvertoFromRTMMessage(message));
    }

    void OnPushRoomChat(RTMMessage message)
    { 
        AddMessage(ConversationType.ROOM, message.toId, ChatDemoMessage.ConvertoFromRTMMessage(message));
    }

    void OnRelogin(bool successful, bool retryAgain, int errorCode, int retriedCount)
    {
        Debug.Log("OnRelogin successful = " + successful + " retryAgain = " + retryAgain + ", errorCode = " + errorCode + ", retriedCount = " + retriedCount);
        if (successful)
        { 
            client.GetUnreadConversationList((List<Conversation> groupConversationList, List<Conversation> p2pConversationList, int errorCode) => 
            { 
                if (errorCode != 0)
                {
                    InitConversationList(groupConversationList);
                    InitConversationList(p2pConversationList);
                }
            }, startTime:conversationList.lastMessageTime);
        }
    }

    void Save()
    {
        if (client == null)
            return;
        if (conversationList.list == null || conversationList.list.Count <= 0)
            return;
        long uid = client.Uid;
        conversationList.timestamp = ClientEngine.GetCurrentMicroseconds();
        string json = JsonUtility.ToJson(conversationList);
        Debug.Log("json " + json);
        PlayerPrefs.SetString("conversation_"+uid.ToString(), json);
        PlayerPrefs.Save();
    }

    void Delete()
    {
        long uid = Convert.ToInt64(UID_InputField.text);
        PlayerPrefs.DeleteKey("conversation_"+uid.ToString());
        PlayerPrefs.Save();
    }

    void UpdateConversationList()
    {
        if (conversationList.list == null)
            return;
        foreach (GameObject conversation in conversationGameObjectList)
            Destroy(conversation);
        conversationGameObjectList.Clear();
        foreach (ChatDemoConversation conversation in conversationList.list)
        {
            GameObject item = Instantiate(conversationItemPrefab, Conversation_Content.transform, false) as GameObject;
            item.name = conversationItemPrefab.name;
            ConversationItem conversationItem = item.GetComponent<ConversationItem>();
            if (conversation.lastMessage != null)
                conversationItem.Init(EnterChat, conversation.type, conversation.id, conversation.id.ToString(), conversation.lastMessage.mtime, conversation.lastMessage.message);
            else
                conversationItem.Init(EnterChat, conversation.type, conversation.id, conversation.id.ToString(), 0, null);
            conversationGameObjectList.Add(item);
        }
    }

    void EnterChat(ConversationType type, long conversationId)
    {
        ChatDemoConversation conversation = GetConversation(type, conversationId);
        if (conversation == null)
            return;

        activeConversationType = type;
        activeConversationId = conversationId;
        ChatDemoChat chat = GetChat(type, conversationId);
        if (chat != null)
            chat.messageList.Clear();

        if (conversation.type == ConversationType.P2P)
        {
            client?.GetP2PChat((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
            {
                if (errorCode == 0)
                {
                    InitChat(conversation.type, conversation.id, lastCursorId, messages);
                }
            }, conversation.id, true, 10);
        }
        else if (conversation.type == ConversationType.GROUP)
        {
            client?.GetGroupChat((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
            {
                if (errorCode == 0)
                {
                    InitChat(conversation.type, conversation.id, lastCursorId, messages);
                }
            }, conversation.id, true, 10);
        }
        else if (conversation.type == ConversationType.ROOM)
        {
            client?.GetRoomChat((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
            {
                if (errorCode == 0)
                {
                    InitChat(conversation.type, conversation.id, lastCursorId, messages);
                }
            }, conversation.id, true, 10);
        }
    }

    void InitChat(ConversationType type, long id, long lastId, List<HistoryMessage> messages)
    {
        RTMControlCenter.callbackQueue.PostAction(() =>
        {
            ChatDemoChat chat = GetChat(type, id);
            if (chat == null)
            {
                chat = new ChatDemoChat();
                chat.type = type;
                chat.id = id;
                chat.messageList = new List<ChatDemoMessage>();
                chatList.Add(chat);
            }
            if (messages != null)
            {
                if ((chat.lastId == 0 || lastId < chat.lastId) && lastId != 0)
                    chat.lastId = lastId;
                foreach (HistoryMessage message in messages)
                {
                    ChatDemoMessage chatDemoMessage = new ChatDemoMessage(message.messageId, message.fromUid, message.stringMessage, message.modifiedTime, message.cursorId);
                    chat.messageList.Add(chatDemoMessage);
                }
                chat.messageList = chat.messageList.Distinct(new ChatDemoMessageCompare()).ToList();
                chat.messageList.Sort();
            }
            UpdateChat(chat);
        });
    }

    void UpdateChat(ChatDemoChat chat)
    {
        foreach (GameObject message in chatGameObjectList)
            Destroy(message);
        chatGameObjectList.Clear();

        if (chat.messageList == null)
            return;

        foreach (ChatDemoMessage message in chat.messageList)
        {
            GameObject item = Instantiate(chatItemPrefab, Chat_Content.transform, false) as GameObject;
            item.name = chatItemPrefab.name;
            ChatItem chatItem = item.GetComponent<ChatItem>();
            chatItem.Init(message.fromUid.ToString(), message.mtime, message.message);
            chatGameObjectList.Add(item);
        }
    }

    ChatDemoChat GetChat(ConversationType type, long id)
    { 
        foreach (ChatDemoChat chat in chatList)
        {
            if (chat.type == type && chat.id == id)
                return chat;
        }
        return null;
    }

    void UpdateMtime(long mtime)
    {
        conversationList.lastMessageTime = mtime;
    }

    void InitConversationList(List<Conversation> list)
    {
        RTMControlCenter.callbackQueue.PostAction(() =>
        {
            foreach (Conversation conversation in list)
            {
                ChatDemoConversation chatDemoConversation = GetConversation(conversation.conversationType, conversation.id);
                if (chatDemoConversation == null)
                {
                    chatDemoConversation = new ChatDemoConversation();
                    chatDemoConversation.type = conversation.conversationType;
                    chatDemoConversation.id = conversation.id;
                    if (conversationList.list == null)
                        conversationList.list = new List<ChatDemoConversation>();
                    conversationList.list.Add(chatDemoConversation);
                }
                chatDemoConversation.unreadCount += conversation.unreadCount;
                if (conversation.lastMessage != null && conversation.lastMessage.messageId != 0)
                    chatDemoConversation.AddMessage(ChatDemoMessage.ConvertoFromHistoryMessage(conversation.lastMessage));
            }
            UpdateConversationList();
        });
    }

    ChatDemoConversation GetConversation(ConversationType type, long conversationId)
    {
        if (conversationList.list == null)
            return null;
        foreach (ChatDemoConversation conversation in conversationList.list)
        {
            if (conversation.type == type && conversation.id == conversationId)
                return conversation;
        }
        return null;
    }

    void GetConversationList()
    {
        long uid = client.Uid;
        if (PlayerPrefs.HasKey("conversation_"+uid.ToString()))
        {
            string val = PlayerPrefs.GetString("conversation_"+uid.ToString());
            conversationList = JsonUtility.FromJson<ChatDemoConversationList>(val);
            UpdateConversationList();
            client.GetUnreadConversationList((List<Conversation> groupConversationList, List<Conversation> p2pConversationList, int errorCode) => 
            { 
                if (errorCode == 0)
                {
                    InitConversationList(groupConversationList);
                    InitConversationList(p2pConversationList);
                }
            }, startTime:conversationList.lastMessageTime);
        }
        else
        {
            client.GetGroupConversationList((List<Conversation> groupConversationList, int errorCode) => 
            { 
                if (errorCode == 0)
                {
                    InitConversationList(groupConversationList);
                }
            });
            client.GetP2PConversationList((List<Conversation> p2pConversationList, int errorCode) => 
            { 
                if (errorCode == 0)
                {
                    InitConversationList(p2pConversationList);
                }
            });
        }

        //client.EnterRoom((int errorCode) =>
        //{
        //    client.GetUserRoomLastMessage((Dictionary<long, HistoryMessage> messages, int errorCode) => {
        //        RTMControlCenter.callbackQueue.PostAction(() => {
        //            if (messages == null)
        //                return;
        //            List<Conversation> conversationList =  new List<Conversation>();
        //            foreach(var kv in messages)
        //            {
        //                Conversation conversation = new Conversation();
        //                conversation.conversationType = ConversationType.ROOM;
        //                conversation.id = kv.Key;
        //                conversation.unreadCount = 0;
        //                conversation.lastMessage = kv.Value;
        //                conversationList.Add(conversation);
        //            }
        //            InitConversationList(conversationList);
        //        });
        //    });
        //}, worldRoomId);


    }

    void AddMessage(ConversationType type, long conversationId, ChatDemoMessage message)
    {
        ChatDemoConversation conversation = GetConversation(type, conversationId);
        if (conversation == null)
        {
            conversation = new ChatDemoConversation();
            conversation.type = type;
            conversation.id = conversationId;
            conversation.unreadCount = 1;
            conversation.lastMessage = message;
            if (conversationList.list == null)
                conversationList.list = new List<ChatDemoConversation>();
            conversationList.list.Add(conversation);
        }
        else
        {
            conversation.AddMessage(message);
        }
        UpdateConversationList();

        if (type == activeConversationType && conversationId == activeConversationId)
        {
            ChatDemoChat chat = GetChat(type, conversationId);
            if (chat == null)
                return;
            chat.messageList.Add(message);
            chat.messageList = chat.messageList.Distinct(new ChatDemoMessageCompare()).ToList();
            chat.messageList.Sort();
            UpdateChat(chat);
        }
    }

    void InitProcessor(ChatDemoQuestProcessor processor)
    {
        processor.PushChatCallback = OnPushP2PChat;
        processor.PushGroupChatCallback = OnPushGroupChat;
        processor.PushRoomChatCallback = OnPushRoomChat;
        processor.ReloginCompletedCallback = OnRelogin;
    }

    void Login()
    {
        long uid = Convert.ToInt64(UID_InputField.text);
        string token = TokenHelper.GetTokenV2(uid, pid, out long ts);
        //string token = TokenHelper.GetToken(uid, pid);
        ChatDemoQuestProcessor processor = new ChatDemoQuestProcessor();
        InitProcessor(processor);
        client = RTMClient.getInstance(rtmServerEndpoint, pid, uid, processor);
        client.Login((long pid, long uid, bool successful, int errorCode) => {
            if (successful)
            {
                RTMControlCenter.callbackQueue.PostAction(() => {
                    GetConversationList();
                });
                Debug.Log("Login succeed");
            }
            else
                Debug.Log("Login failed, errorCode = " + errorCode);
        }, token, ts);
    }

    long activeConversationId = 0;
    ConversationType activeConversationType = ConversationType.INVALID;

    void Send()
    {
        if (activeConversationType == ConversationType.INVALID || activeConversationId == 0)
            return;
        string message = Message_InputField.text;
        ConversationType conversationType = activeConversationType;
        long conversationId = activeConversationId;
        if (conversationType == ConversationType.GROUP)
        {
            client?.SendGroupChat((long messageId, long mtime, int errorCode) => {
                if (errorCode != 0)
                    return;
                RTMControlCenter.callbackQueue.PostAction(() => {
                    AddMessage(conversationType, conversationId, new ChatDemoMessage(messageId, client.Uid, message, mtime));
                    UpdateMtime(mtime);
                });
            }, conversationId, message);
        }
        else if (conversationType == ConversationType.P2P)
        { 
            client?.SendChat((long messageId, long mtime, int errorCode) => { 
                if (errorCode != 0)
                    return;
                RTMControlCenter.callbackQueue.PostAction(() => {
                    AddMessage(conversationType, conversationId, new ChatDemoMessage(messageId, client.Uid, message, mtime));
                    UpdateMtime(mtime);
                });
            }, conversationId, message);
 
        }
        else if (conversationType == ConversationType.ROOM)
        { 
            client?.SendRoomChat((long messageId, long mtime, int errorCode) => {
                if (errorCode != 0)
                    return;
                RTMControlCenter.callbackQueue.PostAction(() => {
                    AddMessage(conversationType, conversationId, new ChatDemoMessage(messageId, client.Uid, message, mtime));
                    UpdateMtime(mtime);
                });
            }, conversationId, message);
        }
    }

    void NextPage()
    {
        if (activeConversationType == ConversationType.INVALID || activeConversationId == 0)
            return;
        ConversationType conversationType = activeConversationType;
        long conversationId = activeConversationId;
        ChatDemoChat chat = GetChat(conversationType, conversationId);
        if (chat == null)
            return;
        if (conversationType == ConversationType.P2P)
        {
            client?.GetP2PChat((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
            {
                if (errorCode == 0)
                {
                    InitChat(conversationType, conversationId, lastCursorId, messages);
                }
            }, conversationId, true, 10, lastId:chat.lastId);
        }
        else if (conversationType == ConversationType.GROUP)
        {
            client?.GetGroupChat((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
            {
                if (errorCode == 0)
                {
                    InitChat(conversationType, conversationId, lastCursorId, messages);
                }
            }, conversationId, true, 10, lastId:chat.lastId);
        }
        else if (conversationType == ConversationType.ROOM)
        {
            client?.GetRoomChat((int count, long lastCursorId, long beginMsec, long endMsec, List<HistoryMessage> messages, int errorCode) =>
            {
                if (errorCode == 0)
                {
                    InitChat(conversationType, conversationId, lastCursorId, messages);
                }
            }, conversationId, true, 10, lastId:chat.lastId);
        }
    }
}
