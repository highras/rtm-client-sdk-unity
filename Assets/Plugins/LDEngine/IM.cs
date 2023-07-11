using System.Collections;
using System.Collections.Generic;
using com.fpnn.common;
using com.fpnn.rtm;
using UnityEngine;

namespace com.fpnn.livedata
{
	public partial class IM 
    {
        RTMClient client;

        public IM(in RTMClient client)
        {
            this.client = client;
        }

        public bool Login(AuthDelegate callback, string token, int timeout = 0)
        {
            return client.Login((long projectId, long uid, bool successful, int errorCode) => 
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(projectId, uid, successful, errorCode);
                });
            }, token, timeout);
        }

        public bool LoginV2(AuthDelegate callback, string token, long ts, int timeout = 0)
        {
            return client.Login((long projectId, long uid, bool successful, int errorCode) => 
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(projectId, uid, successful, errorCode);
                });
            }, token, ts, timeout);
        }

        public void Close()
        {
            client.Close();
        }
    }
}

