using System;
using System.Collections;
using System.Threading;

namespace com.fpnn
{
    public delegate void FPProcessorDelegate(FPData data);

    public class FPProcessor
    {
        private Hashtable EventMap { get; set; }

        public FPProcessor()
        {
            EventMap = new Hashtable();
        }

        public void AddListener(string type, FPProcessorDelegate cb)
        {
            lock(EventMap)
            {
                ArrayList queue;
                if (!EventMap.ContainsKey(type))
                {
                    queue = new ArrayList();
                    EventMap[type] = queue;
                }
                else
                {
                    queue = (ArrayList)EventMap[type];
                }
                queue.Add(cb);
            }
        }

        public void RemoveListener() {
            lock(EventMap) {
                EventMap.Clear();
            }
        }

        public void RemoveListener(string type) {

            lock(EventMap)
            {
                EventMap.Remove(type);
            }
        }

        public void RemoveListener(string type, FPProcessorDelegate cb)
        {
            lock(EventMap)
            {
                if (EventMap.ContainsKey(type))
                {
                    ArrayList queue = ((ArrayList)EventMap[type]);
                    int index = queue.IndexOf(cb);
                    if (index != -1)
                        queue.Remove(index);
                }
            }
        }

        public void FireEvent(string type, FPData data)
        {
            lock(EventMap)
            {
                if (!EventMap.ContainsKey(type))
                    return;

                ArrayList queue = (ArrayList)EventMap[type];
                IEnumerator ie = queue.GetEnumerator();
                while(ie.MoveNext())
                {
                    FPProcessorDelegate cb = (FPProcessorDelegate)ie.Current;
                    ThreadPool.QueueUserWorkItem( (state) =>
                    {
                        try
                        {
                            cb(data);
                        }
                        catch (Exception e)
                        {
                            ErrorRecorderHolder.recordError(e);
                        }
                    });
                }
            }
        }
    }
}