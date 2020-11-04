# RTM Client Unity SDK API Docs: Event Process

# Index

[TOC]

## Class RTMQuestProcessor

    public class RTMQuestProcessor
    {
        //----------------[ System Events ]-----------------//
        public virtual void SessionClosed(int ClosedByErrorCode) { }    //-- ErrorCode: com.fpnn.ErrorCode & com.fpnn.rtm.ErrorCode

        //-- Return true for starting relogin, false for stopping relogin.
        public virtual bool ReloginWillStart(int lastErrorCode, int retriedCount) { return true; }
        public virtual void ReloginCompleted(bool successful, bool retryAgain, int errorCode, int retriedCount) { }

        public virtual void Kickout() { }
        public virtual void KickoutRoom(long roomId) { }

        //----------------[ Message Interfaces ]-----------------//
        //-- Messages
        public virtual void PushMessage(RTMMessage message) { }
        public virtual void PushGroupMessage(RTMMessage message) { }
        public virtual void PushRoomMessage(RTMMessage message) { }
        public virtual void PushBroadcastMessage(RTMMessage message) { }

        //-- Chat
        public virtual void PushChat(RTMMessage message) { }
        public virtual void PushGroupChat(RTMMessage message) { }
        public virtual void PushRoomChat(RTMMessage message) { }
        public virtual void PushBroadcastChat(RTMMessage message) { }

        //-- Cmd
        public virtual void PushCmd(RTMMessage message) { }
        public virtual void PushGroupCmd(RTMMessage message) { }
        public virtual void PushRoomCmd(RTMMessage message) { }
        public virtual void PushBroadcastCmd(RTMMessage message) { }

        //-- Files
        public virtual void PushFile(RTMMessage message) { }
        public virtual void PushGroupFile(RTMMessage message) { }
        public virtual void PushRoomFile(RTMMessage message) { }
        public virtual void PushBroadcastFile(RTMMessage message) { }
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

#### ReloginWillStart & ReloginCompleted

Will triggered when connection lost after **first successful login** if user's token is available and user isn't forbidden.