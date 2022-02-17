using System;
using System.Collections.Generic;
using com.fpnn.msgpack;

namespace com.fpnn.rtm
{
    public class RTMAudioData
    {

        public static string DefaultCodec = "amr-wb";
        private string codecType;
        private string lang;
        private readonly byte[] audio;  // Current is AMR-WB
        private float[] pcmData;  // Original PCM data
        private long duration;   // Duration in ms
        private int frequency;
        private int lengthSamples;

        public RTMAudioData(byte[] audio, float[] pcmData, string codecType, string lang, long duration, int lengthSamples, int frequency)
        {
            this.audio = audio;
            this.pcmData = pcmData;
            this.codecType = codecType;
            this.lang = lang;
            this.duration = duration;
            this.lengthSamples = lengthSamples;
            this.frequency = frequency;
        }

        public RTMAudioData(byte[] audio, FileInfo fileInfo)
        {
            codecType = DefaultCodec;
            lang = fileInfo.language;
            duration = fileInfo.duration;
            this.audio = audio;
            frequency = AudioRecorder.RECORD_SAMPLE_RATE;
            ParseAudioData();
        }

        public RTMAudioData(byte[] audio, string language, long duration)
        {
            codecType = DefaultCodec;
            lang = language;
            this.duration = duration;
            this.audio = audio;
            frequency = AudioRecorder.RECORD_SAMPLE_RATE;
            ParseAudioData();
        }

        private void ParseAudioData()
        {
            byte[] wavBuffer = AudioConvert.ConvertToWav(audio);
            
            int channelCount = wavBuffer[22];
            
            int pos = 12; 
            
            while(! (wavBuffer[pos] == 100 && wavBuffer[pos+1] == 97 && wavBuffer[pos+2] == 116 && wavBuffer[pos+3] == 97)) {
                pos += 4;
                int chunkSize = wavBuffer[pos] + wavBuffer[pos + 1] * 256 + wavBuffer[pos + 2] * 65536 + wavBuffer[pos + 3] * 16777216;
                pos += 4 + chunkSize;
            }
            pos += 8;
            
            lengthSamples = (wavBuffer.Length - pos) /2 ;
            
            pcmData = new float[lengthSamples];
            
            int idx = 0;
            while (pos < wavBuffer.Length) {
                pcmData[idx] = BytesToFloat(wavBuffer[pos], wavBuffer[pos + 1]);
                pos += 2;
                idx++;
            }
        }

        private static float BytesToFloat(byte firstByte, byte secondByte) {
            short s = (short)((secondByte << 8) | firstByte);
            return s / 32768.0F;
        }

        public byte[] Audio
        {
            get
            {
                return audio;
            }
        }

        public float[] PcmData
        {
            get
            {
                return pcmData;
            }
        }

        public long Duration
        {
            get
            {
                return duration;
            }
        }

        public string Language
        {
            get
            {
                return lang;
            }
        }

        public string CodecType
        {
            get
            {
                return codecType;
            }
        }

        public int LengthSamples
        {
            get
            {
                return lengthSamples;
            }
        }

        public int Frequency
        {
            get
            {
                return frequency;
            }
        }
    }
}
