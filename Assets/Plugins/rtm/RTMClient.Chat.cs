using System;
using System.Collections.Generic;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        //===========================[ Sending Chat ]=========================//
        public bool SendChat(ActTimeDelegate callback, long uid, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(uid, (byte)MessageType.Chat, message, attrs, callback, timeout);
        }

        public int SendChat(out long mtime, long uid, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(out mtime, uid, (byte)MessageType.Chat, message, attrs, timeout);
        }


        public bool SendGroupChat(ActTimeDelegate callback, long groupId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(groupId, (byte)MessageType.Chat, message, attrs, callback, timeout);
        }

        public int SendGroupChat(out long mtime, long groupId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(out mtime, groupId, (byte)MessageType.Chat, message, attrs, timeout);
        }


        public bool SendRoomChat(ActTimeDelegate callback, long roomId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(roomId, (byte)MessageType.Chat, message, attrs, callback, timeout);
        }

        public int SendRoomChat(out long mtime, long roomId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(out mtime, roomId, (byte)MessageType.Chat, message, attrs, timeout);
        }

        //===========================[ Sending Cmd ]=========================//
        public bool SendCmd(ActTimeDelegate callback, long uid, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(uid, (byte)MessageType.Cmd, message, attrs, callback, timeout);
        }

        public int SendCmd(out long mtime, long uid, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(out mtime, uid, (byte)MessageType.Cmd, message, attrs, timeout);
        }


        public bool SendGroupCmd(ActTimeDelegate callback, long groupId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(groupId, (byte)MessageType.Cmd, message, attrs, callback, timeout);
        }

        public int SendGroupCmd(out long mtime, long groupId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(out mtime, groupId, (byte)MessageType.Cmd, message, attrs, timeout);
        }


        public bool SendRoomCmd(ActTimeDelegate callback, long roomId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(roomId, (byte)MessageType.Cmd, message, attrs, callback, timeout);
        }

        public int SendRoomCmd(out long mtime, long roomId, string message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(out mtime, roomId, (byte)MessageType.Cmd, message, attrs, timeout);
        }

        //===========================[ Sending Audio ]=========================//
        public bool SendAudio(ActTimeDelegate callback, long uid, byte[] message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(uid, (byte)MessageType.Audio, message, attrs, callback, timeout);
        }

        public int SendAudio(out long mtime, long uid, byte[] message, string attrs = "", int timeout = 0)
        {
            return InternalSendMessage(out mtime, uid, (byte)MessageType.Audio, message, attrs, timeout);
        }


        public bool SendGroupAudio(ActTimeDelegate callback, long groupId, byte[] message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(groupId, (byte)MessageType.Audio, message, attrs, callback, timeout);
        }

        public int SendGroupAudio(out long mtime, long groupId, byte[] message, string attrs = "", int timeout = 0)
        {
            return InternalSendGroupMessage(out mtime, groupId, (byte)MessageType.Audio, message, attrs, timeout);
        }


        public bool SendRoomAudio(ActTimeDelegate callback, long roomId, byte[] message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(roomId, (byte)MessageType.Audio, message, attrs, callback, timeout);
        }

        public int SendRoomAudio(out long mtime, long roomId, byte[] message, string attrs = "", int timeout = 0)
        {
            return InternalSendRoomMessage(out mtime, roomId, (byte)MessageType.Audio, message, attrs, timeout);
        }

        //===========================[ History Chat (Chat & Cmd & Audio) ]=========================//
        private static readonly List<byte> chatMTypes = new List<byte> { (byte)MessageType.Chat, (byte)MessageType.Audio, (byte)MessageType.Cmd };

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
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getunread");
            quest.Param("clear", clear);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

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

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
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
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("cleanunread");
            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
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
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("getsession");
            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

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

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
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
        //-- toId: peer uid, or groupId, or roomId
        //-- type: 1: p2p, 2: group; 3: room

        //-- Obsolete in v.2.2.0
        [System.Obsolete("TranslatedMessage is deprecated, please use RTMMessage instead.")]
        public bool DeleteChat(DoneDelegate callback, long fromUid, long toId, long messageId, int type, int timeout = 0)
        {
            return DeleteMessage(callback, fromUid, toId, messageId, type, timeout);
        }

        public bool DeleteChat(DoneDelegate callback, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0)
        {
            return DeleteMessage(callback, fromUid, toId, messageId, (byte)messageCategory, timeout);
        }

        //-- Obsolete in v.2.2.0
        [System.Obsolete("TranslatedMessage is deprecated, please use RTMMessage instead.")]
        public int DeleteChat(long fromUid, long toId, long messageId, int type, int timeout = 0)
        {
            return DeleteMessage(fromUid, toId, messageId, type, timeout);
        }

        public int DeleteChat(long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0)
        {
            return DeleteMessage(fromUid, toId, messageId, (byte)messageCategory, timeout);
        }

        //===========================[ Get Chat ]=========================//
        //-- toId: peer uid, or groupId, or roomId
        //-- type: 1: p2p, 2: group; 3: room

        //-- Obsolete in v.2.2.0
        [System.Obsolete("TranslatedMessage is deprecated, please use RTMMessage instead.")]
        public bool GetChat(Action<RetrievedMessage, int> callback, long fromUid, long toId, long messageId, int type, int timeout = 0)
        {
            return GetMessage(callback, fromUid, toId, messageId, type, timeout);
        }

        public bool GetChat(Action<RetrievedMessage, int> callback, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0)
        {
            return GetMessage(callback, fromUid, toId, messageId, (byte)messageCategory, timeout);
        }

        //-- Obsolete in v.2.2.0
        [System.Obsolete("TranslatedMessage is deprecated, please use RTMMessage instead.")]
        public int GetChat(out RetrievedMessage retrievedMessage, long fromUid, long toId, long messageId, int type, int timeout = 0)
        {
            return GetMessage(out retrievedMessage, fromUid, toId, messageId, type, timeout);
        }

        public int GetChat(out RetrievedMessage retrievedMessage, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 0)
        {
            return GetMessage(out retrievedMessage, fromUid, toId, messageId, (byte)messageCategory, timeout);
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
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("setlang");
            quest.Param("lang", targetLanguage);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => { callback(errorCode); }, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
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

        //-- Obsolete in v.2.2.0
        //-- Action<TranslatedMessage, errorCode>
        [Obsolete("Translate with TranslatedMessage is deprecated, please use Translate with TranslatedInfo instead.")]
        public bool Translate(Action<TranslatedMessage, int> callback, string text,
            TranslateLanguage destinationLanguage, TranslateLanguage sourceLanguage = TranslateLanguage.None,
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            return Translate((TranslatedInfo translatedinfo, int errorCode) => {
                TranslatedMessage translatedMessage = new TranslatedMessage
                {
                    source = translatedinfo.sourceLanguage,
                    target = translatedinfo.targetLanguage,
                    sourceText = translatedinfo.sourceText,
                    targetText = translatedinfo.targetText
                };

                callback(translatedMessage, errorCode);
            }, text, GetTranslatedLanguage(destinationLanguage),
                GetTranslatedLanguage(sourceLanguage), type, profanity, timeout);
        }

        //-- Action<TranslatedInfo, errorCode>
        public bool Translate(Action<TranslatedInfo, int> callback, string text,
            TranslateLanguage destinationLanguage, TranslateLanguage sourceLanguage = TranslateLanguage.None,
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            return Translate(callback, text, GetTranslatedLanguage(destinationLanguage),
                GetTranslatedLanguage(sourceLanguage), type, profanity, timeout);
        }

        //-- Action<TranslatedInfo, errorCode>
        private bool Translate(Action<TranslatedInfo, int> callback, string text,
            string destinationLanguage, string sourceLanguage = "",
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

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

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

                TranslatedInfo tm = null;

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        tm = new TranslatedInfo();
                        tm.sourceLanguage = answer.Want<string>("source");
                        tm.targetLanguage = answer.Want<string>("target");
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

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        //-- Obsolete in v.2.2.0
        [Obsolete("Translate with TranslatedMessage is deprecated, please use Translate with TranslatedInfo instead.")]
        public int Translate(out TranslatedMessage translatedMessage, string text,
            TranslateLanguage destinationLanguage, TranslateLanguage sourceLanguage = TranslateLanguage.None,
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            int errorCode = Translate(out TranslatedInfo translatedinfo, text, GetTranslatedLanguage(destinationLanguage),
                GetTranslatedLanguage(sourceLanguage), type, profanity, timeout);

            if (translatedinfo != null)
            {
                translatedMessage = new TranslatedMessage
                {
                    source = translatedinfo.sourceLanguage,
                    target = translatedinfo.targetLanguage,
                    sourceText = translatedinfo.sourceText,
                    targetText = translatedinfo.targetText
                };
            }
            else
                translatedMessage = null;

            return errorCode;
        }

        public int Translate(out TranslatedInfo translatedinfo, string text,
            TranslateLanguage destinationLanguage, TranslateLanguage sourceLanguage = TranslateLanguage.None,
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            return Translate(out translatedinfo, text, GetTranslatedLanguage(destinationLanguage),
                GetTranslatedLanguage(sourceLanguage), type, profanity, timeout);
        }

        private int Translate(out TranslatedInfo translatedinfo, string text,
            string destinationLanguage, string sourceLanguage = "",
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            translatedinfo = null;

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
                translatedinfo = new TranslatedInfo
                {
                    sourceLanguage = answer.Want<string>("source"),
                    targetLanguage = answer.Want<string>("target"),
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
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(string.Empty, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("profanity");
            quest.Param("text", text);
            quest.Param("classify", classify);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

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

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(string.Empty, null, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
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
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(string.Empty, string.Empty, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("transcribe");
            quest.Param("audio", audio);
            if (filterProfanity.HasValue)
                quest.Param("profanityFilter", filterProfanity.Value);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

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

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(string.Empty, string.Empty, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
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

        //===========================[ sTranscribe ]=========================//
        //-- Action<string text, string language, errorCode>
        public bool Transcribe(Action<string, string, int> callback, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 120)
        {
            return TranscribeInternal(callback, fromUid, toId, messageId, messageCategory, null, timeout);
        }

        public bool Transcribe(Action<string, string, int> callback, long fromUid, long toId, long messageId, MessageCategory messageCategory, bool filterProfanity, int timeout = 120)
        {
            return TranscribeInternal(callback, fromUid, toId, messageId, messageCategory, filterProfanity, timeout);
        }

        private bool TranscribeInternal(Action<string, string, int> callback, long fromUid, long toId, long messageId, MessageCategory messageCategory, bool? filterProfanity, int timeout = 120)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(string.Empty, string.Empty, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            Quest quest = new Quest("stranscribe");
            quest.Param("from", fromUid);
            quest.Param("xid", toId);
            quest.Param("mid", messageId);
            quest.Param("type", (byte)messageCategory);

            if (filterProfanity.HasValue)
                quest.Param("profanityFilter", filterProfanity.Value);

            bool asyncStarted = client.SendQuest(quest, (Answer answer, int errorCode) => {

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

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(string.Empty, string.Empty, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        public int Transcribe(out string resultText, out string resultLanguage, long fromUid, long toId, long messageId, MessageCategory messageCategory, int timeout = 120)
        {
            return TranscribeInternal(out resultText, out resultLanguage, fromUid, toId, messageId, messageCategory, null, timeout);
        }

        public int Transcribe(out string resultText, out string resultLanguage, long fromUid, long toId, long messageId, MessageCategory messageCategory, bool filterProfanity, int timeout = 120)
        {
            return TranscribeInternal(out resultText, out resultLanguage, fromUid, toId, messageId, messageCategory, filterProfanity, timeout);
        }

        private int TranscribeInternal(out string resultText, out string resultLanguage, long fromUid, long toId, long messageId, MessageCategory messageCategory, bool? filterProfanity, int timeout = 120)
        {
            resultText = "";
            resultLanguage = null;

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("stranscribe");
            quest.Param("from", fromUid);
            quest.Param("xid", toId);
            quest.Param("mid", messageId);
            quest.Param("type", (byte)messageCategory);

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
