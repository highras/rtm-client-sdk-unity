using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public class DevicePushOption
    {
        public Dictionary<long, HashSet<byte>> p2p;
        public Dictionary<long, HashSet<byte>> group;
    }
}

