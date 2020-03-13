using System.Text;
using UnityEngine;

namespace example.common
{
    public class RTMExampleQuestProcessor : com.fpnn.rtm.IRTMQuestProcessor
    {
        public RTMExampleQuestProcessor()
        {
        }

        public void SessionClosed(int ClosedByErrorCode)
        {
            lock (this)
                Debug.Log("Session closed by error code: " + ClosedByErrorCode);
        }

        public void Kickout()
        {
            lock (this)
                Debug.Log("Received kickout.");
        }

        public void KickoutRoom(long roomId)
        {
            lock (this)
                Debug.Log("Kickout from room " + roomId);
        }

        //-- message for string format
        public void PushMessage(long fromUid, long toUid, byte mtype, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push message: from {0}, type: {1}, mid: {2}, attrs: {3}, message: {4}",
                fromUid, mtype, mid, attrs, message);

            lock (this)
                Debug.Log(sb.ToString());
        }

        public void PushGroupMessage(long fromUid, long groupId, byte mtype, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push group message: from {0}, in group {5}, type: {1}, mid: {2}, attrs: {3}, message: {4}",
                    fromUid, mtype, mid, attrs, message, groupId);

            lock (this)
                Debug.Log(sb.ToString());
        }

        public void PushRoomMessage(long fromUid, long roomId, byte mtype, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push room message: from {0}, in room {5}, type: {1}, mid: {2}, attrs: {3}, message: {4}",
                    fromUid, mtype, mid, attrs, message, roomId);

            lock (this)
                Debug.Log(sb.ToString());
        }

        public void PushBroadcastMessage(long fromUid, byte mtype, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push broadcast message: from {0}, type: {1}, mid: {2}, attrs: {3}, message: {4}",
                    fromUid, mtype, mid, attrs, message);

            lock (this)
                Debug.Log(sb.ToString());
        }


        //-- message for binary format
        public void PushMessage(long fromUid, long toUid, byte mtype, long mid, byte[] message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push binary message: from {0}, type: {1}, mid: {2}, attrs: {3}, message length: {4}",
                    fromUid, mtype, mid, attrs, message.Length);

            lock (this)
                Debug.Log(sb.ToString());
        }

        public void PushGroupMessage(long fromUid, long groupId, byte mtype, long mid, byte[] message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push binary group message: from {0}, in group {5}, type: {1}, mid: {2}, attrs: {3}, message length: {4}",
                    fromUid, mtype, mid, attrs, message.Length, groupId);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushRoomMessage(long fromUid, long roomId, byte mtype, long mid, byte[] message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push binary room message: from {0}, in room {5}, type: {1}, mid: {2}, attrs: {3}, message length: {4}",
                    fromUid, mtype, mid, attrs, message.Length, roomId);

            lock (this)
                Debug.Log(sb.ToString());
        }

        public void PushBroadcastMessage(long fromUid, byte mtype, long mid, byte[] message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push binary broadcast message: from {0}, type: {1}, mid: {2}, attrs: {3}, message length: {4}",
                    fromUid, mtype, mid, attrs, message.Length);

            lock (this)
                Debug.Log(sb.ToString());
        }

        public void PushChat(long fromUid, long toUid, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push chat: from {0}, mid: {1}, attrs: {2}, message: {3}",
                    fromUid, mid, attrs, message);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushGroupChat(long fromUid, long groupId, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push group chat: from {0},in group {4}, mid: {1}, attrs: {2}, message: {3}",
                    fromUid, mid, attrs, message, groupId);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushRoomChat(long fromUid, long roomId, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push room chat: from {0},in room {4}, mid: {1}, attrs: {2}, message: {3}",
                    fromUid, mid, attrs, message, roomId);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushBroadcastChat(long fromUid, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push broadcast chat: from {0}, mid: {1}, attrs: {2}, message: {3}",
                    fromUid, mid, attrs, message);

            lock (this)
                Debug.Log(sb.ToString());
        }

        public void PushChat(long fromUid, long toUid, long mid, com.fpnn.rtm.TranslatedMessage message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push translated chat: from {0}, mid: {1}, attrs: {2}, from {3} {4} to {5} {6}",
                    fromUid, mid, attrs, message.source, message.sourceText, message.target, message.targetText);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushGroupChat(long fromUid, long groupId, long mid, com.fpnn.rtm.TranslatedMessage message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push translated group chat: from {0}, in group {7}, mid: {1}, attrs: {2}, from {3} {4} to {5} {6}",
                    fromUid, mid, attrs, message.source, message.sourceText, message.target, message.targetText, groupId);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushRoomChat(long fromUid, long roomId, long mid, com.fpnn.rtm.TranslatedMessage message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push translated room chat: from {0}, in room {7}, mid: {1}, attrs: {2}, from {3} {4} to {5} {6}",
                    fromUid, mid, attrs, message.source, message.sourceText, message.target, message.targetText, roomId);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushBroadcastChat(long fromUid, long mid, com.fpnn.rtm.TranslatedMessage message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push translated broadcast chat: from {0}, mid: {1}, attrs: {2}, from {3} {4} to {5} {6}",
                    fromUid, mid, attrs, message.source, message.sourceText, message.target, message.targetText);

            lock (this)
                Debug.Log(sb.ToString());
        }

        public void PushAudio(long fromUid, long toUid, long mid, byte[] message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push audio: from {0}, mid: {1}, attrs: {2}, audio length: {3}",
                    fromUid, mid, attrs, message.Length);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushGroupAudio(long fromUid, long groupId, long mid, byte[] message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push binary group audio: from {0}, in group {4}, mid: {1}, attrs: {2}, audio length: {3}",
                    fromUid, mid, attrs, message.Length, groupId);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushRoomAudio(long fromUid, long roomId, long mid, byte[] message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push binary room audio: from {0}, in room {4}, mid: {1}, attrs: {2}, audio length: {3}",
                    fromUid, mid, attrs, message.Length, roomId);

            lock (this)
                Debug.Log(sb.ToString());
        }

        public void PushBroadcastAudio(long fromUid, long mid, byte[] message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push binary broadcast audio: from {0}, mid: {1}, attrs: {2}, audio length: {3}",
                    fromUid, mid, attrs, message.Length);

            lock (this)
                Debug.Log(sb.ToString());
        }

        public void PushCmd(long fromUid, long toUid, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push cmd: from {0}, mid: {1}, attrs: {2}, message: {3}",
                    fromUid, mid, attrs, message);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushGroupCmd(long fromUid, long groupId, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push group cmd: from {0},in group {4}, mid: {1}, attrs: {2}, message: {3}",
                    fromUid, mid, attrs, message, groupId);

            lock (this)
                Debug.Log(sb.ToString());
        }

        public void PushRoomCmd(long fromUid, long roomId, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push room cmd: from {0},in room {4}, mid: {1}, attrs: {2}, message: {3}",
                    fromUid, mid, attrs, message, roomId);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushBroadcastCmd(long fromUid, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push broadcast cmd: from {0}, mid: {1}, attrs: {2}, message: {3}",
                    fromUid, mid, attrs, message);

            lock (this)
                Debug.Log(sb.ToString());
        }

        public void PushFile(long fromUid, long toUid, byte mtype, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push file: from {0}, mid: {1}, attrs: {2}, url: {3}",
                    fromUid, mid, attrs, message);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushGroupFile(long fromUid, long groupId, byte mtype, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push group file: from {0},in group {4}, mid: {1}, attrs: {2}, url: {3}",
                    fromUid, mid, attrs, message, groupId);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushRoomFile(long fromUid, long roomId, byte mtype, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push room file: from {0},in room {4}, mid: {1}, attrs: {2}, url: {3}",
                    fromUid, mid, attrs, message, roomId);

            lock (this)
                Debug.Log(sb.ToString());
        }
        public void PushBroadcastFile(long fromUid, byte mtype, long mid, string message, string attrs, long mtime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Receive push broadcast file: from {0}, mid: {1}, attrs: {2}, url: {3}",
                    fromUid, mid, attrs, message);

            lock (this)
                Debug.Log(sb.ToString());
        }
    }
}
