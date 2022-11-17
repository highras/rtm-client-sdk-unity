using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.fpnn.rtm
{
    public class RTMCallbackQueue : MonoBehaviour
    {
        private static Queue<Action> actionQueue = new Queue<Action>();

        public void Clear()
        {
            lock (actionQueue)
            {
                actionQueue.Clear();
            }
        }

        public void PostAction(Action action)
        {
            lock (actionQueue)
            {
                actionQueue.Enqueue(action);
            }
        }

        private Action GetAction()
        {
            lock (actionQueue)
            {
                Action action = actionQueue.Dequeue();
                return action;
            }
        }

        void Update()
        {
            while (actionQueue.Count > 0)
            {
                Action action = GetAction();
                action();
            }
        }
    }
}

