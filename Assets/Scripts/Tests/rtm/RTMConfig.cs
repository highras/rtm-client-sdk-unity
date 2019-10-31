using System;

namespace com.rtm {

    public class RTMConfig {

        public static string VERSION = "1.2.1";

        public static int MID_TTL = 5 * 1000;                           //MID缓存超时时间(ms)
        public static int RECONN_COUNT_ONCE = 1;                        //一次重新连接流程中的尝试次数
        public static int CONNCT_INTERVAL = 40 * 1000;                  //客户端尝试重新连接的时间间隔(ms)
        public static int RECV_PING_TIMEOUT = 40 * 1000;                //客户端收到Ping超时时间(ms)

        public class FILE_TYPE {

            public static byte image = 40;          //图片
            public static byte audio = 41;          //语音
            public static byte video = 42;          //视频
            public static byte file = 50;           //泛指文件，服务器会修改此值（如果服务器可以判断出具体类型的话，仅在mtype=50的情况下）
        }

        public class CHAT_TYPE {

            public static byte text = 30;           //文本
            public static byte audio = 31;          //语音
            public static byte cmd = 32;            //命令
        }

        public static string KICKOUT = "kickout";

        public class SERVER_PUSH {

            public static string kickOutRoom = "kickoutroom";
            public static string recvPing = "ping";

            public static string recvMessage = "pushmsg";
            public static string recvGroupMessage = "pushgroupmsg";
            public static string recvRoomMessage = "pushroommsg";
            public static string recvBroadcastMessage = "pushbroadcastmsg";

            public static string recvFile = "pushfile";
            public static string recvGroupFile = "pushgroupfile";
            public static string recvRoomFile = "pushroomfile";
            public static string recvBroadcastFile = "pushbroadcastfile";

            public static string recvChat = "pushchat";
            public static string recvGroupChat = "pushgroupchat";
            public static string recvRoomChat = "pushroomchat";
            public static string recvBroadcastChat = "pushbroadcastchat";

            public static string recvAudio = "pushaudio";
            public static string recvGroupAudio = "pushgroupaudio";
            public static string recvRoomAudio = "pushroomaudio";
            public static string recvBroadcastAudio = "pushbroadcastaudio";

            public static string recvCmd = "pushcmd";
        }

        public class SERVER_EVENT {

            public static string login = "login";
            public static string logout = "logout";
        }

        public class TRANS_LANGUAGE {

            public static string ar = "ar";             //阿拉伯语
            public static string nl = "nl";             //荷兰语
            public static string en = "en";             //英语
            public static string fr = "fr";             //法语
            public static string de = "de";             //德语
            public static string el = "el";             //希腊语
            public static string id = "id";             //印度尼西亚语
            public static string it = "it";             //意大利语
            public static string ja = "ja";             //日语
            public static string ko = "ko";             //韩语
            public static string no = "no";             //挪威语
            public static string pl = "pl";             //波兰语
            public static string pt = "pt";             //葡萄牙语
            public static string ru = "ru";             //俄语
            public static string es = "es";             //西班牙语
            public static string sv = "sv";             //瑞典语
            public static string tl = "tl";             //塔加路语（菲律宾语）
            public static string th = "th";             //泰语
            public static string tr = "tr";             //土耳其语
            public static string vi = "vi";             //越南语
            public static string zh_cn = "zh-CN";       //中文（简体）
            public static string zh_tw = "zh-TW";       //中文（繁体）
        }

        public class ERROR_CODE {

            public static int RTM_EC_INVALID_PROJECT_ID_OR_USER_ID = 200001;
            public static int RTM_EC_INVALID_PROJECT_ID_OR_SIGN = 200002;
            public static int RTM_EC_INVALID_FILE_OR_SIGN_OR_TOKEN = 200003;
            public static int RTM_EC_ATTRS_WITHOUT_SIGN_OR_EXT = 200004;

            public static int RTM_EC_API_FREQUENCY_LIMITED = 200010;
            public static int RTM_EC_MESSAGE_FREQUENCY_LIMITED = 200011;

            public static int RTM_EC_FORBIDDEN_METHOD = 200020;
            public static int RTM_EC_PERMISSION_DENIED = 200021;
            public static int RTM_EC_UNAUTHORIZED = 200022;
            public static int RTM_EC_DUPLCATED_AUTH = 200023;
            public static int RTM_EC_AUTH_DENIED = 200024;
            public static int RTM_EC_ADMIN_LOGIN = 200025;
            public static int RTM_EC_ADMIN_ONLY = 200026;

            public static int RTM_EC_LARGE_MESSAGE_OR_ATTRS = 200030;
            public static int RTM_EC_LARGE_FILE_OR_ATTRS = 200031;
            public static int RTM_EC_TOO_MANY_ITEMS_IN_PARAMETERS = 200032;
            public static int RTM_EC_EMPTY_PARAMETER = 200033;

            public static int RTM_EC_NOT_IN_ROOM = 200040;
            public static int RTM_EC_NOT_GROUP_MEMBER = 200041;
            public static int RTM_EC_MAX_GROUP_MEMBER_COUNT = 200042;
            public static int RTM_EC_NOT_FRIEND = 200043;
            public static int RTM_EC_BANNED_IN_GROUP = 200044;
            public static int RTM_EC_BANNED_IN_ROOM = 200045;
            public static int RTM_EC_EMPTY_GROUP = 200046;
            public static int RTM_EC_ENTER_TOO_MANY_ROOMS = 200047;

            public static int RTM_EC_UNSUPPORTED_LANGUAGE = 200050;
            public static int RTM_EC_EMPTY_TRANSLATION = 200051;
            public static int RTM_EC_SEND_TO_SELF = 200052;
            public static int RTM_EC_DUPLCATED_MID = 200053;
            public static int RTM_EC_SENSITIVE_WORDS = 200054;

            public static int RTM_EC_UNKNOWN_ERROR = 200999;
        }
    }
}