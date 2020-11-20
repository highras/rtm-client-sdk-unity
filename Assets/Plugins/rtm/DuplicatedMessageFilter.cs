using System.Collections.Generic;

namespace com.fpnn.rtm
{
    internal class DuplicatedMessageFilter
    {
        private enum MessageCategories
        {
            P2PMessageType,
            GroupMessageType,
            RoomMessageType,
            BroadcastMessageType,
        }

        private struct MessageIdUnit
        {
            public MessageCategories messageType;
            public long bizId;
            public long uid;
            public long mid;
        }

        private const int expireSeconds = 20 * 60;
        private Dictionary<MessageIdUnit, long> midCache;
        private object interLocker;

        public DuplicatedMessageFilter()
        {
            midCache = new Dictionary<MessageIdUnit, long>();
            interLocker = new object();
        }

        public bool CheckP2PMessage(long uid, long mid)
        {
            MessageIdUnit unit;
            unit.messageType = MessageCategories.P2PMessageType;
            unit.bizId = 0;
            unit.uid = uid;
            unit.mid = mid;

            return CheckMessageIdUnit(unit);
        }

        public bool CheckGroupMessage(long groupId, long uid, long mid)
        {
            MessageIdUnit unit;
            unit.messageType = MessageCategories.GroupMessageType;
            unit.bizId = groupId;
            unit.uid = uid;
            unit.mid = mid;

            return CheckMessageIdUnit(unit);
        }

        public bool CheckRoomMessage(long roomId, long uid, long mid)
        {
            MessageIdUnit unit;
            unit.messageType = MessageCategories.RoomMessageType;
            unit.bizId = roomId;
            unit.uid = uid;
            unit.mid = mid;

            return CheckMessageIdUnit(unit);
        }

        public bool CheckBroadcastMessage(long uid, long mid)
        {
            MessageIdUnit unit;
            unit.messageType = MessageCategories.BroadcastMessageType;
            unit.bizId = 0;
            unit.uid = uid;
            unit.mid = mid;

            return CheckMessageIdUnit(unit);
        }

        private bool CheckMessageIdUnit(MessageIdUnit unit)
        {
            long now = ClientEngine.GetCurrentSeconds();

            lock (interLocker)
            {
                if (midCache.ContainsKey(unit))
                {
                    midCache[unit] = now;
                    return false;
                }
                else
                {
                    midCache.Add(unit, now);
                    ClearExpired(now);
                    return true;
                }
            }
        }

        private void ClearExpired(long now)
        {
            now -= expireSeconds;
            List<MessageIdUnit> expired = new List<MessageIdUnit>();

            foreach (KeyValuePair<MessageIdUnit, long> kvp in midCache)
            {
                if (kvp.Value <= now)
                    expired.Add(kvp.Key);
            }

            foreach (MessageIdUnit unit in expired)
                midCache.Remove(unit);
        }
    }
}
