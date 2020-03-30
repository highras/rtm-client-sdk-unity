using System;
namespace com.fpnn.rtm
{
    public class RTMConfig
    {
        public static readonly string SDKVersion = "2.0.1";
        public static readonly string InterfaceVersion = "2.0.0";

        internal static int lostConnectionAfterLastPingInSeconds = 120;
        internal static int globalConnectTimeoutSeconds = 30;
        internal static int globalQuestTimeoutSeconds = 30;
        internal static int fileGateClientHoldingSeconds = 150;
        internal static common.ErrorRecorder errorRecorder = null;

        public int maxPingInterval;
        public int globalConnectTimeout;
        public int globalQuestTimeout;
        public int fileClientHoldingSeconds;
        public common.ErrorRecorder defaultErrorRecorder;

        public RTMConfig()
        {
            maxPingInterval = 120;
            globalConnectTimeout = 30;
            globalQuestTimeout = 30;
            fileClientHoldingSeconds = 150;
        }

        internal static void Config(RTMConfig config)
        {
            lostConnectionAfterLastPingInSeconds = config.maxPingInterval;
            globalConnectTimeoutSeconds = config.globalConnectTimeout;
            globalQuestTimeoutSeconds = config.globalQuestTimeout;
            fileGateClientHoldingSeconds = config.fileClientHoldingSeconds;
            errorRecorder = config.defaultErrorRecorder;
        }
    }

    public enum TranslateLanguage
    {
        ar,             //阿拉伯语
        nl,             //荷兰语
        en,             //英语
        fr,             //法语
        de,             //德语
        el,             //希腊语
        id,             //印度尼西亚语
        it,             //意大利语
        ja,             //日语
        ko,             //韩语
        no,             //挪威语
        pl,             //波兰语
        pt,             //葡萄牙语
        ru,             //俄语
        es,             //西班牙语
        sv,             //瑞典语
        tl,             //塔加路语（菲律宾语）
        th,             //泰语
        tr,             //土耳其语
        vi,             //越南语
        zh_cn,       //中文（简体）
        zh_tw,       //中文（繁体）
        None
    }
}
