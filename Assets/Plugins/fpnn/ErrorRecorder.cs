using System;
using UnityEngine;

namespace com.fpnn {

    public abstract class ErrorRecorder {

        public abstract void recordError(Exception e);
    }

    public class DefaultErrorRecorder:ErrorRecorder {

        public override void recordError(Exception e) {

            Debug.LogError(e);
        }
    }

    public class ErrorRecorderHolder {

        private static ErrorRecorder uniqueInstance;
        private static readonly System.Object locker = new System.Object();

        private ErrorRecorderHolder() {}

        public static void setInstance(ErrorRecorder ins) {

            lock (locker) {

                uniqueInstance = ins;
            }
        }

        public static ErrorRecorder getInstance() {

            if (uniqueInstance == null) {

                lock (locker) {

                    if (uniqueInstance == null) {

                        uniqueInstance = new DefaultErrorRecorder();
                    }
                }
            }

            return uniqueInstance;
        }

        public static void recordError(Exception e){
            
            ErrorRecorderHolder.getInstance().recordError(e);
        }
    }
}