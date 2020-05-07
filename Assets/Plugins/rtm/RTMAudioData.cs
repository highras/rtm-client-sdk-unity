using System;
using System.Collections.Generic;
using com.fpnn.msgpack;

namespace com.fpnn.rtm
{
    public class RTMAudioData
    {

        private RTMAudioHeader rtmAudioHeader;

        private readonly byte[] audio;  // Processed audio data with rtm-header
        private float[] pcmData;  // Original PCM data
        private long duration;   // Duration in ms
        private int frequency;
        private int samples;

        public RTMAudioData(RTMAudioHeader rtmAudioHeader, byte[] audio, float[] pcmData, long duration, int samples, int frequency)
        {
            this.rtmAudioHeader = rtmAudioHeader;
            this.audio = audio;
            this.pcmData = pcmData;
            this.duration = duration;
            this.samples = samples;
            this.frequency = frequency;
        }

        public RTMAudioData(byte[] audio)
        {
            this.audio = audio;
            ParseAudioData(audio);
        }

        private void ParseAudioData(byte[] audio)
        {
            rtmAudioHeader = new RTMAudioHeader();

            int offset = 0;
            if (audio.Length < 4)
                return;
            rtmAudioHeader.version = audio[0];
            rtmAudioHeader.containerType = (RTMAudioHeader.ContainerType)audio[1];
            rtmAudioHeader.codecType = (RTMAudioHeader.CodecType)audio[2];

            byte infoDataCount = audio[3];

            offset += 4;

            for (byte i = 0; i < infoDataCount; i++) {
                int sectionLength = BitConverter.ToInt32(audio, offset);
                offset += 4;

                Dictionary<Object, Object> infoData = MsgUnpacker.Unpack(audio, offset, sectionLength);
                object value;
                if (infoData.TryGetValue("lang", out value))
                {
                    rtmAudioHeader.lang = (string)value;
                }
                if (infoData.TryGetValue("dur", out value))
                {
                    rtmAudioHeader.duration = Convert.ToInt64(value);
                    duration = rtmAudioHeader.duration;
                }
                if (infoData.TryGetValue("srate", out value))
                {
                    rtmAudioHeader.sampleRate = Convert.ToInt32(value);
                    frequency = rtmAudioHeader.sampleRate;
                }
                if (infoData.TryGetValue("rtext", out value))
                {
                    rtmAudioHeader.rtext = (string)value;
                }
                if (infoData.TryGetValue("rlang", out value))
                {
                    rtmAudioHeader.rlang = (string)value;
                }

                offset += sectionLength;
            }
            if (offset >= audio.Length) {
                return;
            }

            byte[] amrBuffer = new byte[audio.Length - offset];
            Array.Copy(audio, offset, amrBuffer, 0, audio.Length - offset);
            byte[] wavBuffer = AudioConvert.ConvertToWav(amrBuffer);
            

            int channelCount = wavBuffer[22];
            
            int pos = 12; 
            
            while(! (wavBuffer[pos] == 100 && wavBuffer[pos+1] == 97 && wavBuffer[pos+2] == 116 && wavBuffer[pos+3] == 97)) {
                pos += 4;
                int chunkSize = wavBuffer[pos] + wavBuffer[pos + 1] * 256 + wavBuffer[pos + 2] * 65536 + wavBuffer[pos + 3] * 16777216;
                pos += 4 + chunkSize;
            }
            pos += 8;
            
            samples = (wavBuffer.Length - pos) /2 ;
            
            pcmData = new float[samples];
            
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

        public int Samples
        {
            get
            {
                return samples;
            }
        }

        public int Frequency
        {
            get
            {
                return frequency;
            }
        }

        public RTMAudioHeader RtmAudioHeader
        {
            get
            {
                return rtmAudioHeader;
            }
        }

        public string RecognitionText
        {
            get
            {
                return rtmAudioHeader.rtext;
            }
        }

        public string RecognitionLang
        {
            get
            {
                return rtmAudioHeader.rlang;
            }
        }
        
    }
}