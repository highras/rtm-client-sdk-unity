using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace com.fpnn {

    public delegate void CallbackDelegate(CallbackData cbd);

    public class FPCallback {

        private Hashtable _cbMap = new Hashtable();
        private Hashtable _exMap = new Hashtable();

        public void AddCallback(string key, CallbackDelegate callback, int timeout) {

            lock(this._cbMap) {

                this._cbMap.Add(key, callback);
            }

            lock(this._exMap) {

                int ts = timeout <= 0 ? FPConfig.SEND_TIMEOUT : timeout;
                long expire = ts + ThreadPool.Instance.GetMilliTimestamp();
                this._exMap.Add(key, expire);
            }
        }

        public void RemoveCallback() {

            lock(this._cbMap) {

                this._cbMap.Clear();
            }

            lock(this._exMap) {

                this._exMap.Clear();
            }
        }

        public void ExecCallback(string key, FPData data) {

            CallbackDelegate callback = null;

            lock(this._cbMap) {

                if (this._cbMap.Contains(key)) {

                    callback = (CallbackDelegate)this._cbMap[key];
                    this._cbMap.Remove(key);
                }
            }

            lock(this._exMap) {

                if (this._exMap.Contains(key)) {

                    this._exMap.Remove(key);
                }
            }

            if (callback == null) {

                return;
            }

            ThreadPool.Instance.Execute((state) => {

                try {
                    
                    callback(new CallbackData(data));
                } catch (Exception e) {

                   ErrorRecorderHolder.recordError(e);
                }
            });
        }

        public void ExecCallback(string key, Exception ex) {

            CallbackDelegate callback = null;

            lock(this._cbMap) {

                if (this._cbMap.Contains(key)) {

                    callback = (CallbackDelegate)this._cbMap[key];
                    this._cbMap.Remove(key);
                }
            }

            lock(this._exMap) {

                if (this._exMap.Contains(key)) {

                    this._exMap.Remove(key);
                }
            }

            if (callback == null) {

                return;
            }

            ThreadPool.Instance.Execute((state) => {

                try {

                    callback(new CallbackData(ex));
                } catch (Exception e) {
                    
                    ErrorRecorderHolder.recordError(e);
                }
            });
        }

        public void OnSecond(long timestamp) {

            lock(this._exMap) {

                List<string> keys = new List<string>(); 

                foreach (DictionaryEntry entry in this._exMap) {

                    string key = (string)entry.Key;
                    long expire = (long)entry.Value;

                    if (expire > timestamp) {

                        continue;
                    }

                    keys.Add(key);
                }

                foreach (string rkey in keys) {

                    this._exMap.Remove(rkey);
                    this.ExecCallback(rkey, new Exception("timeout with expire"));
                }
            }
        }
    }
}