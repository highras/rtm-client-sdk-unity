using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
    public class BasePushProcessor
	{
        public SessionClosedDelegate SessionClosedCallback;
        public ReloginWillStartDelegate ReloginWillStartCallback;
        public ReloginCompletedDelegate ReloginCompletedCallback;
        public KickOutDelegate KickoutCallback;
	}
}

