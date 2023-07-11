using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public delegate void RTMPushMessageDelegate(MessageCategory messageCategory, RTMMessage message);
	public class RTMPushProcessor 
	{
        public KickoutRoomDelegate KickoutRoomCallback;

        public RTMPushMessageDelegate PushBasicMessageCallback;
        public RTMPushMessageDelegate PushChatMessageCallback;
        public RTMPushMessageDelegate PushCmdMessageCallback;
        public RTMPushMessageDelegate PushFileMessageCallback;
        public RTMPushMessageDelegate PushAudioMessageCallback;
    }
}

