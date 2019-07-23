using System;
using System.Collections;
using System.Threading;

namespace com.fpnn {

    public delegate void EventDelegate(EventData evd);

    public class FPEvent {

        private Hashtable _listeners = new Hashtable();

        public FPEvent() {}

        public void AddListener(string type, EventDelegate lisr) {

            ArrayList queue = null;

            if (!this._listeners.Contains(type)) {

                queue = new ArrayList();

                lock(this._listeners) {

                    this._listeners[type] = queue;
                }
            } else {

                queue = (ArrayList)this._listeners[type];
            }

            lock (queue) {

                queue.Add(lisr);
            }
        }

        public void RemoveListener() {

            lock(this._listeners) {

                this._listeners.Clear();
            }
        }

        public void RemoveListener(string type) {

            lock(this._listeners) {

                this._listeners.Remove(type);
            }
        }

        public void RemoveListener(string type, EventDelegate lisr) {

            if (!this._listeners.Contains(type)) {

                return;
            }

            ArrayList queue = null;

            lock(this._listeners) {

                queue = ((ArrayList)this._listeners[type]);
            }

            if (queue == null) {

                return;
            }

            lock (queue) {

                int index = queue.IndexOf(lisr);

                if (index != -1) {

                    queue.RemoveAt(index);
                }
            }
        }

        public void FireEvent(EventData evd) {
            
            ArrayList queue = null;
            string type = evd.GetEventType();

            lock(this._listeners) {

                if (this._listeners.Contains(type)) {

                    queue = (ArrayList)this._listeners[type];
                }
            }

            if (queue == null) {

                return;
            }

            lock (queue) {

                IEnumerator ie = queue.GetEnumerator();

                while(ie.MoveNext()) {

                    EventDelegate cb = (EventDelegate)ie.Current;

                    if (cb == null) {

                        continue;
                    }

                    ThreadPool.Instance.Execute((state) => {

                        try {
                            
                            if (cb != null) {

                                cb(evd);
                            }
                        } catch (ThreadAbortException tex){
                        } catch (Exception e) {

                            ErrorRecorderHolder.recordError(e);
                        }
                    });
                }
            }
        }
    }
}