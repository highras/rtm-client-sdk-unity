#if UNITY_2017_1_OR_NEWER

using System;
using System.Runtime.InteropServices;

namespace com.fpnn.rtm
{
    internal static class AudioConvert
    {
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

        public static byte[] ConvertToAmrwb(byte[] wavBuffer)
        {
            int status = 0;
            int amrSize = 0;
            
            IntPtr wavSrcPtr = Marshal.AllocHGlobal(wavBuffer.Length);
            Marshal.Copy(wavBuffer, 0, wavSrcPtr, wavBuffer.Length);

            IntPtr amrPtr = AudioConvert.convert_wav_to_amrwb(wavSrcPtr, wavBuffer.Length, ref status, ref amrSize);

            Marshal.FreeHGlobal(wavSrcPtr);

            if (amrPtr != null && status == 0) {
                byte[] amrBuffer = new byte[amrSize];
                Marshal.Copy(amrPtr, amrBuffer, 0, amrSize);
                AudioConvert.free_memory(amrPtr);
                return amrBuffer;
            }

            if (amrPtr != null)
                AudioConvert.free_memory(amrPtr);

            return null;
        }

        public static byte[] ConvertToWav(byte[] amrBuffer)
        {
            int status = 0;
            int wavSize = 0;
            
            IntPtr amrSrcPtr = Marshal.AllocHGlobal(amrBuffer.Length);
            Marshal.Copy(amrBuffer, 0, amrSrcPtr, amrBuffer.Length);

            IntPtr wavPtr = AudioConvert.convert_amrwb_to_wav(amrSrcPtr, amrBuffer.Length, ref status, ref wavSize);

            Marshal.FreeHGlobal(amrSrcPtr);

            if (wavPtr != null && status == 0) {
                byte[] wavBuffer = new byte[wavSize];
                Marshal.Copy(wavPtr, wavBuffer, 0, wavSize);
                AudioConvert.free_memory(wavPtr);
                return wavBuffer;
            }

            if (wavPtr != null)
                AudioConvert.free_memory(wavPtr);

            return null;
        }
    }
}

#endif