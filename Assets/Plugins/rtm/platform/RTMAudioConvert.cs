using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace com.rtm {

    public class RTMAudioConvert {

        #if UNITY_IOS

            [DllImport("__Internal")]
            public static extern IntPtr convert_wav_to_amrwb(IntPtr wavSrc, int wavSrcSize, ref int status, ref int amrSize);

            [DllImport("__Internal")]
            public static extern IntPtr convert_amrwb_to_wav(IntPtr amrSrc, int amrSrcSize, ref int status, ref int wavSize);

            [DllImport("__Internal")]
            public static extern void free_memory(IntPtr ptr);

        #else

            [DllImport("audio-convert")]
            public static extern IntPtr convert_wav_to_amrwb(IntPtr wavSrc, int wavSrcSize, ref int status, ref int amrSize);

            [DllImport("audio-convert")]
            public static extern IntPtr convert_amrwb_to_wav(IntPtr amrSrc, int amrSrcSize, ref int status, ref int wavSize);
            
            [DllImport("audio-convert")]
            public static extern void free_memory(IntPtr ptr);

        #endif

    }

}