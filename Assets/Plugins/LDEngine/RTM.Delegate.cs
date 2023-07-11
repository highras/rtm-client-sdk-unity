using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public delegate void GetFriendListDelegate(HashSet<long> friendList, int errorCode);
    public delegate void GetBlackListDelegate(HashSet<long> blackList, int errorCode);

    public delegate void GetDevicePushOptionDelegate(DevicePushOption option, int errorCode);
}

