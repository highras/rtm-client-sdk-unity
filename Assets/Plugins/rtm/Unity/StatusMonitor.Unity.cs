#if UNITY_2017_1_OR_NEWER

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using AOT;
using UnityEngine;

namespace com.fpnn.rtm
{
    public enum NetworkType
    { 
        NetworkType_Uninited = -2,
        NetworkType_Unknown = -1,
        NetworkType_Unreachable = 0,
        NetworkType_4G = 1,
        NetworkType_Wifi = 2,
    }
    public class StatusMonitor : Singleton<StatusMonitor>
    {
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
#elif (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
        [DllImport("RTMNative")]
        private static extern void initNetworkStatusChecker(NetworkStatusDelegate callback);
#elif UNITY_IOS
        [DllImport("__Internal")]
        private static extern void initNetworkStatusChecker(NetworkStatusDelegate callback);
#elif UNITY_ANDROID
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void HeadsetStatusDelegate(int networkStatus);

        [MonoPInvokeCallback(typeof(HeadsetStatusDelegate))]
        public static void HeadsetStatusCallback(int headsetType)
        {
        }

        static AndroidJavaObject AndroidNativeManager= null;
        class NetChangeListener : AndroidJavaProxy
        {
            Action<int> msgCallback;
            public NetChangeListener(Action<int> callback) : base("com.NetForUnity.INetChange") { msgCallback = callback; }
            public void netChangeNotify(int type)
            {
                msgCallback(type);
            }
        }
        class HeadsetListener: AndroidJavaProxy
        {
            Action<int> headsetCallback;
            public HeadsetListener(Action<int> callback) : base("com.NetForUnity.IHeadsetChange") { headsetCallback = callback; }

            public void headsetChange(int type) // //0-无网 1-移动网络 2-wifi
            {
                headsetCallback(type);
            }
        }
        private static void initNetworkStatusChecker(Action<int> netChangeCallback, Action<int> headersetCallback)
        {
            if (AndroidNativeManager == null)
            {
                AndroidJavaClass playerClass = new AndroidJavaClass("com.NetForUnity.ListenUnity");
                AndroidNativeManager = playerClass.CallStatic<AndroidJavaObject>("getInstance");
            }
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var context = jc.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidNativeManager.Call("registerNetChange", context, new NetChangeListener(netChangeCallback));
            AndroidNativeManager.Call("registerHeadsetChange", context, new HeadsetListener(headersetCallback));
        }

#else
#endif
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void NetworkStatusDelegate(int networkStatus);

        [MonoPInvokeCallback(typeof(NetworkStatusDelegate))]
        static void NetworkStatusCallback(int networkStatus)
        {
            //RTMControlCenter.NetworkReachableChanged(networkReachable);
            RTMControlCenter.NetworkChanged((NetworkType)networkStatus);
        }

        public void Init() 
        {
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)

#elif (UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
            initNetworkStatusChecker(NetworkStatusCallback);
#elif UNITY_IOS
            initNetworkStatusChecker(NetworkStatusCallback);
#elif UNITY_ANDROID
            initNetworkStatusChecker(NetworkStatusCallback, HeadsetStatusCallback);
#else
#endif
        }
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
        public void Start()
        {
            StartCoroutine(PerSecondCoroutine());
        }

        public void OnDestroy()
        {
            StopAllCoroutines();
        }

        private IEnumerator PerSecondCoroutine()
        {
            yield return new WaitForSeconds(1.0f);

            while (true)
            {
                CheckNetworkChange();

                yield return new WaitForSeconds(1.0f);
            }
        }

        private void CheckNetworkChange()
        {
            int networkStatus = (int)Application.internetReachability;
            RTMControlCenter.NetworkChanged((NetworkType)networkStatus);
        }
#endif
    }
}
#endif
