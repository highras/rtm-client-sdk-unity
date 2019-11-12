using System;

namespace com.fpnn {

    public abstract class ErrorRecorder {
        public abstract void recordError(Exception e);
    }

    public class DefaultErrorRecorder: ErrorRecorder {
        public override void recordError(Exception e) {
            //TODO
            // Unity
            // UnityEngine.Debug.LogError(e);
            // C#
            // Console.WriteLine(e.Message);
            // Console.WriteLine(e.StackTrace);
        }
    }

    public class ErrorRecorderHolder {
        private static ErrorRecorder uniqueInstance;
        private static object lock_obj = new object();

        private ErrorRecorderHolder() {}

        public static void setInstance(ErrorRecorder ins) {
            lock (lock_obj) {
                uniqueInstance = ins;
            }
        }

        public static ErrorRecorder getInstance() {
            lock (lock_obj) {
                if (uniqueInstance == null) {
                    uniqueInstance = new DefaultErrorRecorder();
                }

                return uniqueInstance;
            }
        }

        public static void recordError(Exception ex) {
            ErrorRecorderHolder.getInstance().recordError(ex);
        }
    }
}