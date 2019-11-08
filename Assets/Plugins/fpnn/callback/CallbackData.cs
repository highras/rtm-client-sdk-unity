using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace com.fpnn {

    public class CallbackData {

        private FPData _data = null;

        public FPData GetData() {
            return this._data;
        }

        public CallbackData(FPData data) {
            this._data = data;
        }


        private Exception _exception = null;

        public Exception GetException() {
            return this._exception;
        }

        public CallbackData(Exception ex) {
            this._exception = ex;
        }


        private long _mid = 0;

        public long GetMid() {
            return this._mid;
        }

        public void SetMid(long value) {
            this._mid = value;
        }


        private object _payload = null;

        public object GetPayload() {
            return this._payload;
        }

        public CallbackData(object payload) {
            this._payload = payload;
        }

        public void CheckException(bool isAnswerException, IDictionary<string, object> data) {
            if (data == null && this._exception == null) {
                this._exception = new Exception("data is null!");
                this._payload = null;
            }

            if (this._exception == null && isAnswerException) {
                if (data.ContainsKey("code") && data.ContainsKey("ex")) {
                    StringBuilder sb = new StringBuilder(30);
                    sb.Append(Convert.ToString(data["code"]));
                    sb.Append(" : ");
                    sb.Append(Convert.ToString(data["ex"]));
                    this._exception = new Exception(sb.ToString());
                    this._payload = null;
                }
            }

            if (this._exception == null) {
                this._payload = data;
            }

            this._data = null;
        }
    }
}