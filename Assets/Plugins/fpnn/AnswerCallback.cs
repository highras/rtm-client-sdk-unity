using System;
using System.Threading;
using com.fpnn.proto;

namespace com.fpnn
{
    public delegate void AnswerDelegate(Answer answer, int errorCode);

    public interface IAnswerCallback
    {
        void OnAnswer(Answer answer);
        void OnException(Answer answer, int errorCode);
    }

    internal class AnswerDelegateCallback: IAnswerCallback
    {
        private AnswerDelegate callback;

        public AnswerDelegateCallback(AnswerDelegate answerDelegate)
        {
            callback = answerDelegate;
        }

        public void OnAnswer(Answer answer)
        {
            callback(answer, answer.IsException() ? answer.ErrorCode() : ErrorCode.FPNN_EC_OK);
        }
        public void OnException(Answer answer, int errorCode)
        {
            callback(answer, errorCode);
        }
    }

    internal class SyncAnswerCallback: IAnswerCallback
    {
        private ManualResetEvent syncEvent;
        private Answer answer;
        private UInt32 seqNum;

        public SyncAnswerCallback(Quest quest)
        {
            syncEvent = new ManualResetEvent(false);
            seqNum = quest.SeqNum();
        }

        public Answer GetAnswer()
        {
            syncEvent.WaitOne();
            syncEvent.Close();
            return answer;
        }

        public void OnAnswer(Answer answer)
        {
            this.answer = answer;
            syncEvent.Set();
        }
        public void OnException(Answer answer, int errorCode)
        {
            if (answer != null)
                this.answer = answer;
            else
            {
                this.answer = new Answer(seqNum);
                this.answer.FillErrorCode(errorCode);
            }

            syncEvent.Set();
        }
    }
}
