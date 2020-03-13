# RTM Client Unity SDK API Docs: Event Process

# Index

[TOC]

## Interface IRTMQuestProcessor

    public interface IRTMQuestProcessor
    {
        void SessionClosed(int ClosedByErrorCode);        //-- com.fpnn.ErrorCode & com.fpnn.rtm.ErrorCode

        void Kickout();
        void KickoutRoom(long roomId);

        //-- message for string format
        void PushMessage(long fromUid, long toUid, byte mtype, long mid, string message, string attrs, long mtime);
        void PushGroupMessage(long fromUid, long groupId, byte mtype, long mid, string message, string attrs, long mtime);
        void PushRoomMessage(long fromUid, long roomId, byte mtype, long mid, string message, string attrs, long mtime);
        void PushBroadcastMessage(long fromUid, byte mtype, long mid, string message, string attrs, long mtime);

        //-- message for binary format
        void PushMessage(long fromUid, long toUid, byte mtype, long mid, byte[] message, string attrs, long mtime);
        void PushGroupMessage(long fromUid, long groupId, byte mtype, long mid, byte[] message, string attrs, long mtime);
        void PushRoomMessage(long fromUid, long roomId, byte mtype, long mid, byte[] message, string attrs, long mtime);
        void PushBroadcastMessage(long fromUid, byte mtype, long mid, byte[] message, string attrs, long mtime);

        void PushChat(long fromUid, long toUid, long mid, string message, string attrs, long mtime);
        void PushGroupChat(long fromUid, long groupId, long mid, string message, string attrs, long mtime);
        void PushRoomChat(long fromUid, long roomId, long mid, string message, string attrs, long mtime);
        void PushBroadcastChat(long fromUid, long mid, string message, string attrs, long mtime);

        void PushChat(long fromUid, long toUid, long mid, TranslatedMessage message, string attrs, long mtime);
        void PushGroupChat(long fromUid, long groupId, long mid, TranslatedMessage message, string attrs, long mtime);
        void PushRoomChat(long fromUid, long roomId, long mid, TranslatedMessage message, string attrs, long mtime);
        void PushBroadcastChat(long fromUid, long mid, TranslatedMessage message, string attrs, long mtime);

        void PushAudio(long fromUid, long toUid, long mid, byte[] message, string attrs, long mtime);
        void PushGroupAudio(long fromUid, long groupId, long mid, byte[] message, string attrs, long mtime);
        void PushRoomAudio(long fromUid, long roomId, long mid, byte[] message, string attrs, long mtime);
        void PushBroadcastAudio(long fromUid, long mid, byte[] message, string attrs, long mtime);

        void PushCmd(long fromUid, long toUid, long mid, string message, string attrs, long mtime);
        void PushGroupCmd(long fromUid, long groupId, long mid, string message, string attrs, long mtime);
        void PushRoomCmd(long fromUid, long roomId, long mid, string message, string attrs, long mtime);
        void PushBroadcastCmd(long fromUid, long mid, string message, string attrs, long mtime);

        void PushFile(long fromUid, long toUid, byte mtype, long mid, string message, string attrs, long mtime);
        void PushGroupFile(long fromUid, long groupId, byte mtype, long mid, string message, string attrs, long mtime);
        void PushRoomFile(long fromUid, long roomId, byte mtype, long mid, string message, string attrs, long mtime);
        void PushBroadcastFile(long fromUid, byte mtype, long mid, string message, string attrs, long mtime);
    }

### Session Close Event

	void SessionClosed(int ClosedByErrorCode)

Parameters:

+ `int ClosedByErrorCode`

	0 or com.fpnn.ErrorCode.FPNN_EC_OK means closed by user or kickout cmd.

	Others are the reason for failed.

### Server Pushed Events

All methods in IRTMQuestProcessor interface except for SessionClosed() are server pushed events.

#### Kickout

Current client is kicked.

The session is closed by RTM SDK automatically before the method is called.

#### Push Files Methods

The parameter `message` is the URL of file in CDN.