using UnityEngine;
using System.Collections;

namespace com.fpnn.livedata
{
    public class LDEngineConfig
    {
        //readonly public static string RTMEndpoint_NX = "rtm-nx-front.ilivedata.com:13321";
        readonly public static string RTMEndpoint_NX = "161.189.171.91:13321";
        //readonly public static string RTCEndpoint_NX = "rtc-nx-front.ilivedata.com:13702";
        readonly public static string RTCEndpoint_NX = "161.189.171.91:13702";
        readonly public static string RTMEndpoint_INTL = "rtm-intl-frontgate.ilivedata.com:13321";
        readonly public static string RTCEndpoint_INTL = "rtc-intl-frontgate.ilivedata.com:13702";
    }

    public enum RTMENDPOINT
    {
        RTMENDPOINT_NX,
        RTMENDPOINT_INTL,
    }
}
