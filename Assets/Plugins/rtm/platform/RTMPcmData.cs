using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace com.rtm {
    public class RTMPcmData
    {
        static float BytesToFloat(byte firstByte, byte secondByte) {
             short s = (short)((secondByte << 8) | firstByte);
             return s / 32768.0F;
         }

         static int BytesToInt(byte[] bytes,int offset=0){
             int value=0;
             for(int i=0;i<4;i++){
                 value |= ((int)bytes[offset+i])<<(i*8);
             }
             return value;
         }

         public float[] LeftChannel{get; internal set;}
         public float[] RightChannel{get; internal set;}
         public int ChannelCount {get;internal set;}
         public int SampleCount {get;internal set;}
         public int Frequency {get;internal set;}

        public RTMPcmData(byte[] wav) {

            ChannelCount = wav[22];
            Frequency = BytesToInt(wav, 24);
            
            int pos = 12;   // First Subchunk ID from 12 to 16
            
            // Keep iterating until we find the data chunk (i.e. 64 61 74 61 ...... (i.e. 100 97 116 97 in decimal))
            while(! (wav[pos] == 100 && wav[pos+1] == 97 && wav[pos+2] == 116 && wav[pos+3] == 97)) {
                pos += 4;
                int chunkSize = wav[pos] + wav[pos + 1] * 256 + wav[pos + 2] * 65536 + wav[pos + 3] * 16777216;
                pos += 4 + chunkSize;
            }
            pos += 8;
            
            SampleCount = (wav.Length - pos)/2;     // 2 bytes per sample (16 bit sound mono)
            if (ChannelCount == 2) SampleCount /= 2;        // 4 bytes per sample (16 bit stereo)
            
            // Allocate memory (right will be null if only mono sound)
            LeftChannel = new float[SampleCount];
            if (ChannelCount == 2) RightChannel = new float[SampleCount];
            else RightChannel = null;
            
            // Write to double array/s:
            int i = 0;
            while (pos < wav.Length) {
                LeftChannel[i] = BytesToFloat(wav[pos], wav[pos + 1]);
                pos += 2;
                if (ChannelCount == 2) {
                    RightChannel[i] = BytesToFloat(wav[pos], wav[pos + 1]);
                    pos += 2;
                }
                i++;
            }
        }
         
    }
}
