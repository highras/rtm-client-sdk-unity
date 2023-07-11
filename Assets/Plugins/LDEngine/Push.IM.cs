using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
	public class IMPushProcessor 
	{
        public KickoutRoomDelegate KickoutRoomCallback;

        public RTMPushMessageDelegate PushChatMessageCallback;
        public RTMPushMessageDelegate PushFileMessageCallback;

        //-- IMLIB Friend
        public IMLIB_PushFriendChangedDelegate IMLIB_PushFriendChangedCallback;
        public IMLIB_PushAddFriendApplyDelegate IMLIB_PushAddFriendApplyCallback;
        public IMLIB_PushAcceptFriendApplyDelegate IMLIB_PushAcceptFriendApplyCallback;
        public IMLIB_PushRefuseFriendApplyDelegate IMLIB_PushRefuseFriendApplyCallback;

        //-- IMLIB Group
        public IMLIB_PushEnterGroupApplyDelegate IMLIB_PushEnterGroupApplyCallback;
        public IMLIB_PushAcceptEnterGroupApplyDelegate IMLIB_PushAcceptEnterGroupApplyCallback;
        public IMLIB_PushRefuseEnterGroupApplyDelegate IMLIB_PushRefuseEnterGroupApplyCallback;
        public IMLIB_PushInvitedIntoGroupDelegate IMLIB_PushInvitedIntoGroupCallback;
        public IMLIB_PushAccpetInvitedIntoGroupDelegate IMLIB_PushAccpetInvitedIntoGroupCallback;
        public IMLIB_PushRefuseInvitedIntoGroupDelegate IMLIB_PushRefuseInvitedIntoGroupCallback;
        public IMLIB_PushGroupChangedDelegate IMLIB_PushGroupChangedCallback;
        public IMLIB_PushGroupMemberChangedDelegate IMLIB_PushGroupMemberChangedCallback;
        public IMLIB_PushGroupOwnerChangedDelegate IMLIB_PushGroupOwnerChangedCallback;
        public IMLIB_PushGroupManagerChangedDelegate IMLIB_PushGroupManagerChangedCallback;

        //-- IMLIB Room
        public IMLIB_PushEnterRoomApplyDelegate IMLIB_PushEnterRoomApplyCallback;
        public IMLIB_PushAcceptEnterRoomApplyDelegate IMLIB_PushAcceptEnterRoomApplyCallback;
        public IMLIB_PushRefuseEnterRoomApplyDelegate IMLIB_PushRefuseEnterRoomApplyCallback;
        public IMLIB_PushInvitedIntoRoomDelegate IMLIB_PushInvitedIntoRoomCallback;
        public IMLIB_PushAccpetInvitedIntoRoomDelegate IMLIB_PushAccpetInvitedIntoRoomCallback;
        public IMLIB_PushRefuseInvitedIntoRoomDelegate IMLIB_PushRefuseInvitedIntoRoomCallback;
        public IMLIB_PushRoomChangedDelegate IMLIB_PushRoomChangedCallback;
        public IMLIB_PushRoomMemberChangedDelegate IMLIB_PushRoomMemberChangedCallback;
        public IMLIB_PushRoomOwnerChangedDelegate IMLIB_PushRoomOwnerChangedCallback;
        public IMLIB_PushRoomManagerChangedDelegate IMLIB_PushRoomManagerChangedCallback;
    }
}

