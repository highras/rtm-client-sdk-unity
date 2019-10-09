using System;

namespace com.fpnn {

    public class EventData {

        private string _type;

        public string GetEventType() {
            return this._type;
        }

        public EventData(string type) {
            this._type = type;
        }

        private FPData _data = null;

        public FPData GetData() {
            return this._data;
        }

        public EventData(string type, FPData data) {
            this._type = type;
            this._data = data;
        }

        private Exception _exception = null;

        public Exception GetException() {
            return this._exception;
        }

        public EventData(string type, Exception ex) {
            this._type = type;
            this._exception = ex;
        }

        private long _timestamp = 0;

        public long GetTimestamp() {
            return this._timestamp;
        }

        public EventData(string type, long timestamp) {
            this._type = type;
            this._timestamp = timestamp;
        }

        private Object _payload;

        public Object GetPayload() {
            return this._payload;
        }

        public EventData(string type, Object payload) {
            this._type = type;
            this._payload = payload;
        }

        private bool _retry;

        public bool HasRetry() {
            return this._retry;
        }

        public EventData(string type, bool retry) {
            this._type = type;
            this._retry = retry;
        }
    }
}