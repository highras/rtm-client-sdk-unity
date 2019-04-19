using System;

namespace com.fpnn {

    public delegate void AnswerDelegate(object payload, bool exception);

    public class FPProcessor {

        public interface IProcessor {

            void Service(FPData data, AnswerDelegate answer);
            void OnSecond(long timestamp);
            FPEvent GetEvent();
        }

        private class BaseProcessor:IProcessor {

            private FPEvent _event = new FPEvent();

            public void Service(FPData data, AnswerDelegate answer) {

                if (data.GetFlag() == 0) {

                    this._event.FireEvent(new EventData(data.GetMethod(), data.JsonPayload()));
                }

                if (data.GetFlag() == 1) {

                    this._event.FireEvent(new EventData(data.GetMethod(), data.MsgpackPayload()));
                }
            }

            public FPEvent GetEvent() {

                return this._event;
            }

            public void OnSecond(long timestamp) {}
        }

        private IProcessor _processor;

        public FPEvent GetEvent() {

            if (this._processor != null) {

                return this._processor.GetEvent();
            }

            return null;
        }

        public void SetProcessor(IProcessor processor) {

            this._processor = processor;
        }

        public void Service(FPData data, AnswerDelegate answer) {

            if (this._processor == null) {

                this._processor = new BaseProcessor();  
            }

            this._processor.Service(data, answer);
        }

        public void OnSecond(long timestamp) {

            if (this._processor != null) {

                this._processor.OnSecond(timestamp);
            }
        }

        public void Destroy() {}
    }
}