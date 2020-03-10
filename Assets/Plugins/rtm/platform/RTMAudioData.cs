using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace com.rtm {
    public class RTMAudioData
    {
        public byte[] AmrData;
        public long Duration;

        public RTMAudioData(byte[] data, long duration)
        {
            this.AmrData = data;
            this.Duration = duration;
        }
    }
}
