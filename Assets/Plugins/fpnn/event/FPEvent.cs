using System;
using System.Collections;
using System.Threading;

namespace com.fpnn {

    public delegate void EventDelegate(EventData evd);

    public class FPEvent {

        private object self_locker = new object();
        private Hashtable _listeners = new Hashtable();

        public FPEvent() {}

        public void AddListener(string type, EventDelegate lisr) {

            ArrayList queue = null;

            lock(self_locker) {

                if (!this._listeners.Contains(type)) {

                    this._listeners.Add(type, new ArrayList());
                }

                queue = (ArrayList)this._listeners[type];

                if (queue.IndexOf(lisr) == -1) {

                    queue.Add(lisr);
                }
            }
        }

        public void RemoveListener() {

            lock (self_locker) {

                this._listeners.Clear();
            }
        }

        public void RemoveListener(string type) {

            lock (self_locker) {

                this._listeners.Remove(type);
            }
        }

        public void RemoveListener(string type, EventDelegate lisr) {

            ArrayList queue = null;

            lock (self_locker) {

                if (!this._listeners.Contains(type)) {

                    return;
                }

                queue = ((ArrayList)this._listeners[type]);

                int index = queue.IndexOf(lisr);

                if (index != -1) {

                    queue.RemoveAt(index);
                }
            }
        }

        public void FireEvent(EventData evd) {
            
            ArrayList queue = null;
            string type = evd.GetEventType();

            lock (self_locker) {

                if (!this._listeners.Contains(type)) {

                    return;
                }

                queue = ((ArrayList)this._listeners[type]);

                IEnumerator ie = queue.GetEnumerator();

                while(ie.MoveNext()) {

                    EventDelegate callback = (EventDelegate)ie.Current;

                    if (callback != null) {

                        FPManager.Instance.AddEventCall(callback, evd);
                    }
                }
            }
        }
    }
}