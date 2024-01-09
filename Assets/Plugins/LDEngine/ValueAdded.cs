using System;
using System.Collections;
using System.Collections.Generic;
using com.fpnn.rtm;
using UnityEngine;
using static com.fpnn.rtm.RTMClient;

namespace com.fpnn.livedata
{
    public partial class ValueAdded
    {
        RTMClient client;
        public ValueAdded(in RTMClient client)
        {
            this.client = client;
        }

        public bool SetUserLanguage(DoneDelegate callback, TranslateLanguage language, int timeout = 0)
        {
            return client.SetTranslatedLanguage((int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(errorCode);
                });
            }, language, timeout);
        }

        public bool Translate(Action<TranslatedInfo, int> callback, string text,
            string destinationLanguage, string sourceLanguage = "",
            TranslateType type = TranslateType.Chat, ProfanityType profanity = ProfanityType.Off,
            int timeout = 0)
        {
            return client.Translate((TranslatedInfo info, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(info, errorCode);
                });
            }, text, destinationLanguage, sourceLanguage, type, profanity, timeout);
        }

        public bool SpeechToText(Action<string, string, int> callback, byte[] audioBinaryContent, string language, string codec = null, int sampleRate = 0, int timeout = 120)
        {
            return client.SpeechToText((string text, string resultLanguage, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(text, resultLanguage, errorCode);
                });
            }, audioBinaryContent, language, codec, sampleRate, timeout);
        }

        public bool SpeechTranslate(Action<TranslatedInfo, int> callback, byte[] audioBinaryContent, string speechLanguage, string textLanguage = null, string codec = null, int sampleRate = 0, int timeout = 120)
        {
            return client.SpeechTranslate((TranslatedInfo info, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(info, errorCode);
                });
            }, audioBinaryContent, speechLanguage, textLanguage, codec, sampleRate, timeout);
        }

        public bool TextCheck(Action<TextCheckResult, int> callback, string text, string strategyId = null, int timeout = 120)
        {
            return client.TextCheck((TextCheckResult result, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(result, errorCode);
                });
            }, text, strategyId, null, timeout);
        }

        public bool ImageCheck(Action<CheckResult, int> callback, string imageUrl, string strategyId = null, int timeout = 120)
        {
            return client.ImageCheck((CheckResult result, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(result, errorCode);
                });
            }, imageUrl, strategyId, timeout);
        }

        public bool ImageCheck(Action<CheckResult, int> callback, byte[] imageContent, string strategyId = null, int timeout = 120)
        { 
            return client.ImageCheck((CheckResult result, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(result, errorCode);
                });
            }, imageContent, strategyId, timeout);
        }

        public bool AudioCheck(Action<CheckResult, int> callback, string audioUrl, string language, string codec = null, int sampleRate = 0, string strategyId = null, int timeout = 120)
        {
            return client.AudioCheck((CheckResult result, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(result, errorCode);
                });
            }, audioUrl, language, codec, sampleRate, strategyId, timeout);
        }

        public bool AudioCheck(Action<CheckResult, int> callback, byte[] audioContent, string language, string codec = null, int sampleRate = 0, string strategyId = null, int timeout = 120)
        {
            return client.AudioCheck((CheckResult result, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(result, errorCode);
                });
            }, audioContent, language, codec, sampleRate, strategyId, timeout);
        }

        public bool VideoCheck(Action<CheckResult, int> callback, string videoUrl, string videoName, string strategyId = null, int timeout = 120)
        {
            return client.VideoCheck((CheckResult result, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(result, errorCode);
                });
            }, videoUrl, videoName, strategyId, timeout);
        }

        public bool VideoCheck(Action<CheckResult, int> callback, byte[] videoContent, string videoName, string strategyId = null, int timeout = 120)
        {
            return client.VideoCheck((CheckResult result, int errorCode) =>
            {
                RTMControlCenter.callbackQueue.PostAction(() =>
                {
                    callback(result, errorCode);
                });
            }, videoContent, videoName, strategyId, timeout);
        }
	}
}

