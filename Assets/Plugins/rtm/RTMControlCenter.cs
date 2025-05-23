﻿using System;
using System.Collections.Generic;
using System.Threading;
using static com.fpnn.rtm.RTMClient;

#if UNITY_2017_1_OR_NEWER
using UnityEngine;
#endif

namespace com.fpnn.rtm
{
    public static class RTMControlCenter
    {
        private static object interLocker = new object();
        //private static volatile bool networkReachable = true;
        private static volatile NetworkType networkType = NetworkType.NetworkType_Uninited;
        private static Dictionary<Int64, RTMClient> rtmClients = new Dictionary<long, RTMClient>();
        private static Dictionary<Int64, Dictionary<Int64, RTMClient>> pidUidClients = new Dictionary<Int64, Dictionary<Int64, RTMClient>>();
        private static Dictionary<RTMClient, Int64> reloginClients = new Dictionary<RTMClient, Int64>();

        private static Dictionary<string, Dictionary<TCPClient, long>> fileClients = new Dictionary<string, Dictionary<TCPClient, long>>();

        private static volatile bool routineInited;
        private static volatile bool routineRunning;
        private static Thread routineThread;
        private static GameObject rtmGameObject;
        public static RTMCallbackQueue callbackQueue;

        static RTMControlCenter()
        {
            routineInited = false;
        }

        public static NetworkType NetworkStatus 
        {
            get { return networkType; }
        }
        //===========================[ Session Functions ]=========================//
        internal static void RegisterSession(Int64 connectionId, RTMClient client)
        {
            CheckRoutineInit();

            lock (interLocker)
            {
                try
                {
                    rtmClients.Add(connectionId, client);
                }
                catch (ArgumentException)
                {
                    
                }
            }
        }

        internal static void UnregisterSession(Int64 connectionId)
        {
            lock (interLocker)
            {
                rtmClients.Remove(connectionId);
            }
        }

        internal static void CloseSession(Int64 connectionId)
        {
            RTMClient client = null;
            lock (interLocker)
            {
                rtmClients.TryGetValue(connectionId, out client);
            }

            if (client != null)
                client.Close();
        }

        internal static ClientStatus GetClientStatus(Int64 connectionId)
        { 
            RTMClient client = null;
            lock (interLocker)
            {
                rtmClients.TryGetValue(connectionId, out client);
            }

            if (client != null)
                return client.Status;
            else
                return ClientStatus.Closed;
        }
        internal static void AddClient(Int64 projectId, Int64 uid, RTMClient client)
        { 
            lock (interLocker)
            { 
                pidUidClients.TryGetValue(projectId, out Dictionary<Int64, RTMClient> clients);
                if (clients == null)
                {
                    clients = new Dictionary<long, RTMClient>{ { uid, client } };
                    pidUidClients.Add(projectId, clients);
                }
                else
                {
                    clients.TryGetValue(uid, out RTMClient rtmClient);
                    if (rtmClient == null)
                        clients.Add(uid, client);
                    else
                        throw new Exception("duplicated RTMClient pid = " + projectId.ToString() + ", uid = " + uid.ToString());
                }
            }
        }

        internal static RTMClient FetchClient(Int64 projectId, Int64 uid)
        {
            RTMClient client = null;
            lock (interLocker)
            {
                pidUidClients.TryGetValue(projectId, out Dictionary<Int64, RTMClient> clients);
                if (clients == null)
                    return null;
                clients.TryGetValue(uid, out client);
            }
            return client;
        }

        //===========================[ Relogin Functions ]=========================//
        internal static void DelayRelogin(RTMClient client, long triggeredMs)
        {
            CheckRoutineInit();

            lock (interLocker)
            {
                try
                {
                    reloginClients.Add(client, triggeredMs);
                }
                catch (ArgumentException)
                {
                     //-- Do nothing.
                }
            }
        }

        private static void ReloginCheck()
        {
            //if (!networkReachable)
            //return;
            if (networkType != NetworkType.NetworkType_4G && networkType != NetworkType.NetworkType_Wifi)
                return;

            HashSet<RTMClient> clients = new HashSet<RTMClient>();
            long now = ClientEngine.GetCurrentMilliseconds();

            lock (interLocker)
            {
                foreach (KeyValuePair<RTMClient, Int64> kvp in reloginClients)
                {
                    if (kvp.Value <= now)
                        clients.Add(kvp.Key);
                }

                foreach (RTMClient client in clients)
                    reloginClients.Remove(client);
            }

            foreach (RTMClient client in clients)
            {
                ClientEngine.RunTask(() => {
                    if (client.CheckRelogin(out bool stopByNetwork))
                        client.StartRelogin();
                    
                    if (stopByNetwork)
                        DelayRelogin(client, now);
                });
            }
        }

        //internal static void NetworkReachableChanged(bool reachable)
        //{
        //    if (reachable != networkReachable)
        //    {
        //        networkReachable = reachable;
        //        long now = ClientEngine.GetCurrentMilliseconds();
        //        if (reachable)
        //        {
        //            Dictionary<RTMClient, Int64> clients = new Dictionary<RTMClient, Int64>();
        //            lock (interLocker)
        //            {
        //                foreach (KeyValuePair<RTMClient, Int64> kvp in reloginClients)
        //                    clients.Add(kvp.Key, now);

        //                reloginClients = clients;
        //            }
        //        }
        //        else
        //        {
        //            lock (interLocker)
        //            {
        //                foreach (KeyValuePair<UInt64, RTMClient> kvp in rtmClients)
        //                {
        //                    kvp.Value.Close();
        //                    reloginClients.Add(kvp.Value, now);
        //                }
        //            }
        //        }
        //    }
        //}

        internal static void NetworkChanged(NetworkType type)
        {
            if (networkType == NetworkType.NetworkType_Uninited)
                networkType = type;
            if (type == NetworkType.NetworkType_Unknown)
                type = NetworkType.NetworkType_Unreachable;
            if (networkType == type)
                return;
            long now = ClientEngine.GetCurrentMilliseconds();
            NetworkType oldType = networkType;
            networkType = type;
            ClientEngine.RunTask(()=>
            {
                if (oldType == NetworkType.NetworkType_Unreachable && (type == NetworkType.NetworkType_4G || type == NetworkType.NetworkType_Wifi))
                {//之前没有网络，现在有网络
                    Dictionary<RTMClient, Int64> clients = new Dictionary<RTMClient, Int64>();
                    List<RTMClient> activeClients = new List<RTMClient>();
                    lock (interLocker)
                    {
                        foreach (KeyValuePair<RTMClient, Int64> kvp in reloginClients)
                            clients.Add(kvp.Key, now);

                        foreach (KeyValuePair<Int64, RTMClient> kvp in rtmClients)
                        {
                            if (!clients.ContainsKey(kvp.Value))
                            {
                                if (kvp.Value.Status != ClientStatus.Connecting)
                                {
                                    activeClients.Add(kvp.Value);
                                    clients.Add(kvp.Value, now);
                                }
                            }
                        }
                        foreach (RTMClient client in activeClients)
                            client.Close(false, false);
                    
                        reloginClients = clients;
                    }
                }
                else if ((type == NetworkType.NetworkType_4G && oldType == NetworkType.NetworkType_Wifi) || (oldType == NetworkType.NetworkType_4G && type == NetworkType.NetworkType_Wifi))
                { 
                    lock (interLocker)
                    {
                        List<RTMClient> clients = new List<RTMClient>();
                        foreach (KeyValuePair<Int64, RTMClient> kvp in rtmClients)
                        {
                            if (kvp.Value.Status != ClientStatus.Connecting)
                            {
                                clients.Add(kvp.Value);
                            }
                        }
                        foreach (RTMClient client in clients)
                            client.Close(false, false);
                    }
                }
                else
                {
                    lock (interLocker)
                    {
                        List<RTMClient> clients = new List<RTMClient>();
                        foreach (KeyValuePair<Int64, RTMClient> kvp in rtmClients)
                        {
                            if (kvp.Value.Status != ClientStatus.Connecting)
                            {
                                clients.Add(kvp.Value);
                                DelayRelogin(kvp.Value, now);
                            }
                        }
                        foreach (RTMClient client in clients)
                            client.Close(false, false, RTMConfig.callCloseEventWhenNetworkClosed);
                    }
                }
            });
            
        }

        //===========================[ File Gate Client Functions ]=========================//
        internal static void ActiveFileGateClient(string endpoint, TCPClient client)
        {
            lock (interLocker)
            {
                if (fileClients.TryGetValue(endpoint, out Dictionary<TCPClient, long> clients))
                {
                    if (clients.ContainsKey(client))
                        clients[client] = ClientEngine.GetCurrentSeconds();
                    else
                        clients.Add(client, ClientEngine.GetCurrentSeconds());
                }
                else
                {
                    clients = new Dictionary<TCPClient, long>
                    {
                        { client, ClientEngine.GetCurrentSeconds() }
                    };
                    fileClients.Add(endpoint, clients);
                }
            }
        }

        internal static TCPClient FecthFileGateClient(string endpoint)
        {
            lock (interLocker)
            {
                if (fileClients.TryGetValue(endpoint, out Dictionary<TCPClient, long> clients))
                {
                    foreach (KeyValuePair<TCPClient, long> kvp in clients)
                        return kvp.Key;
                }
            }

            return null;
        }

        private static void CheckFileGateClients()
        {
            HashSet<string> emptyEndpoints = new HashSet<string>();

            lock (interLocker)
            {
                long threshold = ClientEngine.GetCurrentSeconds() - RTMConfig.fileGateClientHoldingSeconds;

                foreach (KeyValuePair<string, Dictionary<TCPClient, long>> kvp in fileClients)
                {
                    HashSet<TCPClient> unactivedClients = new HashSet<TCPClient>();

                    foreach (KeyValuePair<TCPClient, long> subKvp in kvp.Value)
                    {
                        if (subKvp.Value <= threshold)
                            unactivedClients.Add(subKvp.Key);
                    }

                    foreach (TCPClient client in unactivedClients)
                    {
                        kvp.Value.Remove(client);
                    }

                    if (kvp.Value.Count == 0)
                        emptyEndpoints.Add(kvp.Key);
                }

                foreach (string endpoint in emptyEndpoints)
                    fileClients.Remove(endpoint);
            }
        }

        //===========================[ Init & Routine Functions ]=========================//
        public static void Init()
        {
            Init(null);
        }

        public static void Init(RTMConfig config)
        {
#if UNITY_2017_1_OR_NEWER
            StatusMonitor.Instance.Init();
#endif
            InitCallbackQueue();
            if (config == null)
                return;

            RTMConfig.Config(config);
        }

        private static void InitCallbackQueue()
        {
            rtmGameObject = new GameObject(RTMConfig.RTMGameObjectName);
            callbackQueue = rtmGameObject.AddComponent<RTMCallbackQueue>();
            GameObject.DontDestroyOnLoad(rtmGameObject);
            rtmGameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        private static void CheckRoutineInit()
        {
            if (routineInited)
                return;

            lock (interLocker)
            {
                if (routineInited)
                    return;

                routineRunning = true;

                routineThread = new Thread(RoutineFunc)
                {
                    Name = "RTM.ControlCenter.RoutineThread",
#if UNITY_2017_1_OR_NEWER
#else
                    IsBackground = true
#endif
                };
                routineThread.Start();


                routineInited = true;
            }
        }

        private static void RoutineFunc()
        {
            while (routineRunning)
            {
                Thread.Sleep(1000);

                HashSet<RTMClient> clients;
                try
                {
                    clients = new HashSet<RTMClient>();
                }
                catch (Exception e)
                {
                    RTMConfig.errorRecorder?.RecordError(e);
                    continue;
                }

                lock (interLocker)
                {
                    foreach (KeyValuePair<Int64, RTMClient> kvp in rtmClients)
                        clients.Add(kvp.Value);
                }

                foreach (RTMClient client in clients)
                    if (client.ConnectionIsAlive() == false)
                        client.Close(false, true);

                CheckFileGateClients();
                ReloginCheck();
            }
        }

        public static void Close()
        {
#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN)
            AudioRecorderNative.destroy();
#endif
            StatusMonitor.Instance.Close();

            lock (interLocker)
            {
                if (!routineInited)
                    return;

                if (!routineRunning)
                    return;

                routineRunning = false;
            }

#if UNITY_2017_1_OR_NEWER
            routineThread.Join();
#endif
            HashSet<RTMClient> clients = new HashSet<RTMClient>();

            lock (interLocker)
            {
                foreach (KeyValuePair<Int64, RTMClient> kvp in rtmClients)
                    clients.Add(kvp.Value);
            }

            foreach (RTMClient client in clients)
                    client.Close(true, true);

            rtmClients.Clear();
            pidUidClients.Clear();
            reloginClients.Clear();
            fileClients.Clear();
        }

        public static void Clear() 
        {
            rtmClients.Clear();
            pidUidClients.Clear();
            reloginClients.Clear();
            fileClients.Clear();
        }
    }
}
