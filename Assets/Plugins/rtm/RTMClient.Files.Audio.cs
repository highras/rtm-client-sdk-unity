using System.Collections.Generic;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        private Dictionary<string, object> BuildAudioMessageAttrs(RTMAudioData audioData)
        {
            Dictionary<string, object> rtmAttrs = new Dictionary<string, object>();
            rtmAttrs.Add("type", "audiomsg");
            rtmAttrs.Add("codec", audioData.CodecType);
            rtmAttrs.Add("srate", audioData.Frequency);
            rtmAttrs.Add("lang", audioData.Language);
            rtmAttrs.Add("duration", audioData.Duration);
            return rtmAttrs;
        }

        //===========================[ Send RTM-Audio File ]=========================//
        public bool SendFile(MessageIdDelegate callback, long peerUid, RTMAudioData audioData, string attrs = "", int timeout = 120)
        {
            return RealSendFile(callback, FileTokenType.P2P, peerUid, (byte)MessageType.AudioFile, audioData.Audio, "", "", attrs, BuildAudioMessageAttrs(audioData), timeout);
        }

        public int SendFile(out long messageId, long peerUid, RTMAudioData audioData, string attrs = "", int timeout = 120)
        {
            return RealSendFile(out messageId, FileTokenType.P2P, peerUid, (byte)MessageType.AudioFile, audioData.Audio, "", "", attrs, BuildAudioMessageAttrs(audioData), timeout);
        }

        //===========================[ Sned RTM-Audio Group File ]=========================//
        public bool SendGroupFile(MessageIdDelegate callback, long groupId, RTMAudioData audioData, string attrs = "", int timeout = 120)
        {
            return RealSendFile(callback, FileTokenType.Group, groupId, (byte)MessageType.AudioFile, audioData.Audio, "", "", attrs, BuildAudioMessageAttrs(audioData), timeout);
        }

        public int SendGroupFile(out long messageId, long groupId, RTMAudioData audioData, string attrs = "", int timeout = 120)
        {
            return RealSendFile(out messageId, FileTokenType.Group, groupId, (byte)MessageType.AudioFile, audioData.Audio, "", "", attrs, BuildAudioMessageAttrs(audioData), timeout);
        }

        //===========================[ Sned Sned RTM-Audio Room File ]=========================//
        public bool SendRoomFile(MessageIdDelegate callback, long roomId, RTMAudioData audioData, string attrs = "", int timeout = 120)
        {
            return RealSendFile(callback, FileTokenType.Room, roomId, (byte)MessageType.AudioFile, audioData.Audio, "", "", attrs, BuildAudioMessageAttrs(audioData), timeout);
        }

        public int SendRoomFile(out long messageId, long roomId, RTMAudioData audioData, string attrs = "", int timeout = 120)
        {
            return RealSendFile(out messageId, FileTokenType.Room, roomId, (byte)MessageType.AudioFile, audioData.Audio, "", "", attrs, BuildAudioMessageAttrs(audioData), timeout);
        }
    }
}
