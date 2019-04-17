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

            queue.Add(lisr);
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

            ArrayList queue = ((ArrayList)this._listeners[type]);

            int index = queue.IndexOf(lisr);

            if (index != -1) {

                queue.Remove(index);
            }
        }

        public void FireEvent(EventData evd) {
            
            string type = evd.GetType();

            if (this._listeners.Contains(type)) {

                ArrayList queue = (ArrayList)this._listeners[type];

                lock(queue) {

                    IEnumerator ie = queue.GetEnumerator();

                    while(ie.MoveNext()) {

                        EventDelegate cb = (EventDelegate)ie.Current;

                        ThreadPool.Instance.Execute((state) => {

                            try {
                                cb(evd);
                            } catch (Exception e) {}
                        });
                    }
                }
            }
        }
    }
}