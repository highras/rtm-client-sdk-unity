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
            if (string.IsNullOrEmpty(key)) {
                ErrorRecorderHolder.recordError(new Exception("callback key is null or empty"));
                return;
            }

            if (callback == null) {
                ErrorRecorderHolder.recordError(new Exception("CallbackDelegate is null"));
                return;
            }

            lock (this._cbMap) {
                if (!this._cbMap.Contains(key)) {
                    this._cbMap.Add(key, callback);
                }
            }

            lock (this._exMap) {
                if (!this._exMap.Contains(key)) {
                    int ts = timeout <= 0 ? FPConfig.SEND_TIMEOUT : timeout;
                    long expire = ts + FPManager.Instance.GetMilliTimestamp();
                    this._exMap.Add(key, expire);
                }
            }
        }

        public void RemoveCallback() {
            lock (this._cbMap) {
                this._cbMap.Clear();
            }

            lock (this._exMap) {
                this._exMap.Clear();
            }
        }

        public void ExecCallback(string key, FPData data) {
            if (string.IsNullOrEmpty(key)) {
                ErrorRecorderHolder.recordError(new Exception("callback key is null or empty"));
                return;
            }

            CallbackDelegate callback = null;

            lock (this._cbMap) {
                if (this._cbMap.Contains(key)) {
                    callback = (CallbackDelegate)this._cbMap[key];
                    this._cbMap.Remove(key);
                }
            }

            lock (this._exMap) {
                if (this._exMap.Contains(key)) {
                    this._exMap.Remove(key);
                }
            }

            if (callback != null) {
                FPManager.Instance.CallbackTask(callback, new CallbackData(data));
            }
        }

        public void ExecCallback(string key, Exception ex) {
            if (string.IsNullOrEmpty(key)) {
                ErrorRecorderHolder.recordError(new Exception("callback key is null or empty"));
                return;
            }

            CallbackDelegate callback = null;

            lock (this._cbMap) {
                if (this._cbMap.Contains(key)) {
                    callback = (CallbackDelegate)this._cbMap[key];
                    this._cbMap.Remove(key);
                }
            }

            lock (this._exMap) {
                if (this._exMap.Contains(key)) {
                    this._exMap.Remove(key);
                }
            }

            if (callback != null) {
                FPManager.Instance.CallbackTask(callback, new CallbackData(ex));
            }
        }

        public void OnSecond(long timestamp) {
            List<string> keys = new List<string>();

            lock (this._exMap) {
                foreach (DictionaryEntry entry in this._exMap) {
                    string key = (string)entry.Key;
                    long expire = (long)entry.Value;

                    if (expire > timestamp) {
                        continue;
                    }

                    keys.Add(key);
                }
            }

            foreach (string rkey in keys) {
                this.ExecCallback(rkey, new Exception("timeout with expire"));
            }
        }
    }
}