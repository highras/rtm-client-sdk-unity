using System;
using System.Collections.Generic;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        internal static readonly byte MessageMType_Chat = 30;
        internal static readonly byte MessageMType_Audio = 31;
        internal static readonly byte MessageMType_Cmd = 32;

        //===========================[ Sending Chat ]=========================//
        public bool SendChat(ActTimeDelegate callback, long uid, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(uid, MessageMType_Chat, message, attrs, callback, timeout);
        }

        public int SendChat(out long mtime, long uid, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(out mtime, uid, MessageMType_Chat, message, attrs, timeout);
        }


        public bool SendGroupChat(ActTimeDelegate callback, long groupId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(groupId, MessageMType_Chat, message, attrs, callback, timeout);
        }

        public int SendGroupChat(out long mtime, long groupId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(out mtime, groupId, MessageMType_Chat, message, attrs, timeout);
        }


        public bool SendRoomChat(ActTimeDelegate callback, long roomId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(roomId, MessageMType_Chat, message, attrs, callback, timeout);
        }

        public int SendRoomChat(out long mtime, long roomId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(out mtime, roomId, MessageMType_Chat, message, attrs, timeout);
        }

        //===========================[ Sending Cmd ]=========================//
        public bool SendCmd(ActTimeDelegate callback, long uid, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(uid, MessageMType_Cmd, message, attrs, callback, timeout);
        }

        public int SendCmd(out long mtime, long uid, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(out mtime, uid, MessageMType_Cmd, message, attrs, timeout);
        }


        public bool SendGroupCmd(ActTimeDelegate callback, long groupId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(groupId, MessageMType_Cmd, message, attrs, callback, timeout);
        }

        public int SendGroupCmd(out long mtime, long groupId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(out mtime, groupId, MessageMType_Cmd, message, attrs, timeout);
        }


        public bool SendRoomCmd(ActTimeDelegate callback, long roomId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(roomId, MessageMType_Cmd, message, attrs, callback, timeout);
        }

        public int SendRoomCmd(out long mtime, long roomId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(out mtime, roomId, MessageMType_Cmd, message, attrs, timeout);
        }

        //===========================[ Sending Audio ]=========================//
        public bool SendAudio(ActTimeDelegate callback, long uid, byte[] message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(uid, MessageMType_Audio, message, attrs, callback, timeout);
        }

        public int SendAudio(out long mtime, long uid, byte[] message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(out mtime, uid, MessageMType_Audio, message, attrs, timeout);
        }


        public bool SendGroupAudio(ActTimeDelegate callback, long groupId, byte[] message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(groupId, MessageMType_Audio, message, attrs, callback, timeout);
        }

        public int SendGroupAudio(out long mtime, long groupId, byte[] message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(out mtime, groupId, MessageMType_Audio, message, attrs, timeout);
        }


        public bool SendRoomAudio(ActTimeDelegate callback, long roomId, byte[] message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(roomId, MessageMType_Audio, message, attrs, callback, timeout);
        }

        public int SendRoomAudio(out long mtime, long roomId, byte[] message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(out mtime, roomId, MessageMType_Audio, message, attrs, timeout);
        }

        //===========================[ History Chat (Chat & Cmd & Audio) ]=========================//
        private static readonly List<byte> chatMTypes = new List<byte> { MessageMType_Chat, MessageMType_Audio, MessageMType_Cmd };

        public bool GetGroupChat(HistoryMessageDelegate callback, long groupId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetGroupMessage(callback, groupId, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }

        public int GetGroupChat(out HistoryMessageResult result, long groupId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetGroupMessage(out result, groupId, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }


        public bool GetRoomChat(HistoryMessageDelegate callback, long roomId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetRoomMessage(callback, roomId, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }

        public int GetRoomChat(out HistoryMessageResult result, long roomId, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetRoomMessage(out result, roomId, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }


        public bool GetBroadcastChat(HistoryMessageDelegate callback, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetBroadcastMessage(callback, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }

        public int GetBroadcastChat(out HistoryMessageResult result, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetBroadcastMessage(out result, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }


        public bool GetP2PChat(HistoryMessageDelegate callback, long peerUid, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetP2PMessage(callback, peerUid, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }

        public int GetP2PChat(out HistoryMessageResult result, long peerUid, bool desc, int count, long beginMsec = 0, long endMsec = 0, long lastId = 0, int timeout = 0)
        {
            return GetP2PMessage(out result, peerUid, desc, count, beginMsec, endMsec, lastId, chatMTypes, timeout);
        }

        //===========================[ Unread Chat ]=========================//
        //-- Action<List<p2p_uid>, List<groupId>, errorCode>
        public bool GetUnread(Action<List<long>, List<long>, int> callback, bool clear = false, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("getunread");
            quest.Param("clear", clear);

            return client.SendQuest(quest, (Answer answer, int errorCode) => {

                List<long> p2pList = null;
                List<long> groupList = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        p2pList = WantLongList(answer, "p2p");
                        groupList = WantLongList(answer, "group");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(p2pList, groupList, errorCode);
            }, timeout);
        }

        public int GetUnread(out List<long> p2pList, out List<long> groupList, bool clear = false, int timeout = 0)
        {
            p2pList = null;
            groupList = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getunread");
            quest.Param("clear", clear);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                p2pList = WantLongList(answer, "p2p");
                groupList = WantLongList(answer, "group");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Clear Unread ]=========================//
        public bool ClearUnread(DoneDelegate callback, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("cleanunread");
            return client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);
        }

        public int ClearUnread(int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("cleanunread");
            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Get Session ]=========================//
        //-- Action<List<p2p_uid>, List<groupId>, errorCode>
        public bool GetSession(Action<List<long>, List<long>, int> callback, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("getsession");
            return client.SendQuest(quest, (Answer answer, int errorCode) => {

                List<long> p2pList = null;
                List<long> groupList = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        p2pList = WantLongList(answer, "p2p");
                        groupList = WantLongList(answer, "group");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(p2pList, groupList, errorCode);
            }, timeout);
        }

        public int GetSession(out List<long> p2pList, out List<long> groupList, int timeout = 0)
        {
            p2pList = null;
            groupList = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("getsession");
            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                p2pList = WantLongList(answer, "p2p");
                groupList = WantLongList(answer, "group");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Delete Chat ]=========================//
        //-- xid: peer uid, or groupId, or roomId
        //-- type: 1: p2p, 2: group; 3: room

        public bool DeleteChat(DoneDelegate callback, long xid, long mid, int type, int timeout = 0)
        {
            return DeleteMessage(callback, xid, mid, type, timeout);
        }

        public int DeleteChat(long xid, long mid, int type, int timeout = 0)
        {
            return DeleteMessage(xid, mid, type, timeout);
        }

        //===========================[ Get Chat ]=========================//
        //-- xid: peer uid, or groupId, or roomId
        //-- type: 1: p2p, 2: group; 3: room

        public bool GetChat(Action<RetrievedMessage, int> callback, long xid, long mid, int type, int timeout = 0)
        {
            return GetMessage(callback, xid, mid, type, timeout);
        }

        public int GetChat(out RetrievedMessage retrievedMessage, long xid, long mid, int type, int timeout = 0)
        {
            return GetMessage(out retrievedMessage, xid, mid, type, timeout);
        }

        //===========================[ Set Translated Languag ]=========================//
        public bool SetTranslatedLanguage(DoneDelegate callback, TranslateLanguage targetLanguage, int timeout = 0)
        {
            return SetTranslatedLanguage(callback, GetTranslatedLanguage(targetLanguage), timeout);
        }
        private bool SetTranslatedLanguage(DoneDelegate callback, string targetLanguage, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("setlang");
            quest.Param("lang", targetLanguage);

            return client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);
        }

        public int SetTranslatedLanguage(TranslateLanguage targetLanguage, int timeout = 0)
        {
            return SetTranslatedLanguage(GetTranslatedLanguage(targetLanguage), timeout);
        }
        private int SetTranslatedLanguage(string targetLanguage, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("setlang");
            quest.Param("lang", targetLanguage);
            Answer answer = client.SendQuest(quest, timeout);
            return answer.ErrorCode();
        }

        //===========================[ Translate ]=========================//
        public enum TranslateType
        {
            Chat,
            Mail
        }

        public enum ProfanityType
        {
            Off,
            Stop,
            Censor
        }

        //-- Action<TranslatedMessage, errorCode>
        public bool Translate(Action<TranslatedMessage, int> callback, string text,
            TranslateLanguage destinationLanguage, TranslateLanguage sourceLanguage = TranslateLanguage.None,
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            return Translate(callback, text, GetTranslatedLanguage(destinationLanguage),
                GetTranslatedLanguage(sourceLanguage), type, profanity, timeout);
        }

        //-- Action<TranslatedMessage, errorCode>
        private bool Translate(Action<TranslatedMessage, int> callback, string text,
            string destinationLanguage, string sourceLanguage = "",
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("translate");
            quest.Param("text", text);
            quest.Param("dst", destinationLanguage);

            if (sourceLanguage.Length > 0)
                quest.Param("src", sourceLanguage);

            if (type == TranslateType.Mail)
                quest.Param("type", "mail");
            else
                quest.Param("type", "chat");

            switch (profanity)
            {
                case ProfanityType.Stop: quest.Param("profanity", "stop"); break;
                case ProfanityType.Censor: quest.Param("profanity", "censor"); break;
                case ProfanityType.Off: quest.Param("profanity", "off"); break;
            }

            return client.SendQuest(quest, (Answer answer, int errorCode) => {

                TranslatedMessage tm = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        tm = new TranslatedMessage();
                        tm.source = answer.Want<string>("source");
                        tm.target = answer.Want<string>("target");
                        tm.sourceText = answer.Want<string>("sourceText");
                        tm.targetText = answer.Want<string>("targetText");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(tm, errorCode);
            }, timeout);
        }

        public int Translate(out TranslatedMessage translatedMessage, string text,
            TranslateLanguage destinationLanguage, TranslateLanguage sourceLanguage = TranslateLanguage.None,
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            return Translate(out translatedMessage, text, GetTranslatedLanguage(destinationLanguage),
                GetTranslatedLanguage(sourceLanguage), type, profanity, timeout);
        }

        private int Translate(out TranslatedMessage translatedMessage, string text,
            string destinationLanguage, string sourceLanguage = "",
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            translatedMessage = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("translate");
            quest.Param("text", text);
            quest.Param("dst", destinationLanguage);

            if (sourceLanguage.Length > 0)
                quest.Param("src", sourceLanguage);

            if (type == TranslateType.Mail)
                quest.Param("type", "mail");
            else
                quest.Param("type", "chat");

            switch (profanity)
            {
                case ProfanityType.Stop: quest.Param("profanity", "stop"); break;
                case ProfanityType.Censor: quest.Param("profanity", "censor"); break;
                case ProfanityType.Off: quest.Param("profanity", "off"); break;
            }

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                translatedMessage = new TranslatedMessage
                {
                    source = answer.Want<string>("source"),
                    target = answer.Want<string>("target"),
                    sourceText = answer.Want<string>("sourceText"),
                    targetText = answer.Want<string>("targetText")
                };

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Profanity ]=========================//
        //-- Action<string text, List<string> classification, errorCode>
        public bool Profanity(Action<string, List<string>, int> callback, string text, bool classify = false, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("profanity");
            quest.Param("text", text);
            quest.Param("classify", classify);

            return client.SendQuest(quest, (Answer answer, int errorCode) => {

                string resultText = "";
                List<string> classification = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        resultText = answer.Want<string>("text");
                        classification = GetStringList(answer, "classification");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(resultText, classification, errorCode);
            }, timeout);
        }

        public int Profanity(out string resultText, out List<string> classification, string text, bool classify = false, int timeout = 0)
        {
            resultText = "";
            classification = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("profanity");
            quest.Param("text", text);
            quest.Param("classify", classify);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                resultText = answer.Want<string>("text");
                classification = GetStringList(answer, "classification");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ Transcribe ]=========================//
        //-- Action<string text, string language, errorCode>
        public bool Transcribe(Action<string, string, int> callback, byte[] audio, int timeout = 120)
        {
            return TranscribeInternal(callback, audio, null, timeout);
        }

        public bool Transcribe(Action<string, string, int> callback, byte[] audio, bool filterProfanity, int timeout = 120)
        {
            return TranscribeInternal(callback, audio, filterProfanity, timeout);
        }

        private bool TranscribeInternal(Action<string, string, int> callback, byte[] audio, bool? filterProfanity, int timeout = 120)
        {
#if UNITY_2017_1_OR_NEWER
            RTMAudioData audioData = new RTMAudioData(audio);
            string cacheText = audioData.RecognitionText;
            string cacheLanguage = audioData.RecognitionLang;
            if (cacheText != "" && cacheLanguage != "")
            {
                ClientEngine.RunTask(() => {
                    callback(cacheText, cacheLanguage, fpnn.ErrorCode.FPNN_EC_OK);
                });
                return true;
            }
#endif

            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("transcribe");
            quest.Param("audio", audio);
            if (filterProfanity.HasValue)
                quest.Param("profanityFilter", filterProfanity.Value);

            return client.SendQuest(quest, (Answer answer, int errorCode) => {

                string resultText = "";
                string resultLanguage = "";

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        resultText = answer.Want<string>("text");
                        resultLanguage = answer.Want<string>("lang");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(resultText, resultLanguage, errorCode);
            }, timeout);
        }

        public int Transcribe(out string resultText, out string resultLanguage, byte[] audio, int timeout = 120)
        {
            return TranscribeInternal(out resultText, out resultLanguage, audio, null, timeout);
        }

        public int Transcribe(out string resultText, out string resultLanguage, byte[] audio, bool filterProfanity, int timeout = 120)
        {
            return TranscribeInternal(out resultText, out resultLanguage, audio, filterProfanity, timeout);
        }

        private int TranscribeInternal(out string resultText, out string resultLanguage, byte[] audio, bool? filterProfanity, int timeout = 120)
        {
#if UNITY_2017_1_OR_NEWER
            RTMAudioData audioData = new RTMAudioData(audio);
            string cacheText = audioData.RecognitionText;
            string cacheLanguage = audioData.RecognitionLang;
            if (cacheText != "" && cacheLanguage != "")
            {
                resultText = cacheText;
                resultLanguage = cacheLanguage;
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
#endif

            resultText = "";
            resultLanguage = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("transcribe");
            quest.Param("audio", audio);
            if (filterProfanity.HasValue)
                quest.Param("profanityFilter", filterProfanity.Value);

            Answer answer = client.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                resultText = answer.Want<string>("text");
                resultLanguage = answer.Want<string>("lang");

                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }
    }
}
