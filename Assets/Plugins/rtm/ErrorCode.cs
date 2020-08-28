using System;
namespace com.fpnn.rtm
{
    public static class ErrorCode
    {
        public const int RTM_EC_INVALID_PID_OR_UID = 200001;
        public const int RTM_EC_INVALID_PID_OR_SIGN = 200002;
        public const int RTM_EC_INVALID_FILE_OR_SIGN_OR_TOKEN = 200003;
        public const int RTM_EC_ATTRS_WITHOUT_SIGN_OR_EXT = 200004;
        public const int RTM_EC_INVALID_MTYPE = 200005;
        public const int RTM_EC_SAME_SIGN = 200006;
        public const int RTM_EC_INVALID_FILE_MTYPE = 200007;
        public const int RTM_EC_INVALID_SERFVER_TIME = 200008;

        public const int RTM_EC_FREQUENCY_LIMITED = 200010;
        public const int RTM_EC_REFRESH_SCREEN_LIMITED = 200011;
        public const int RTM_EC_KICKOUT_SELF = 200012;

        public const int RTM_EC_FORBIDDEN_METHOD = 200020;
        public const int RTM_EC_PERMISSION_DENIED = 200021;
        public const int RTM_EC_UNAUTHORIZED = 200022;
        public const int RTM_EC_DUPLCATED_AUTH = 200023;
        public const int RTM_EC_AUTH_DENIED = 200024;
        public const int RTM_EC_ADMIN_LOGIN = 200025;
        public const int RTM_EC_ADMIN_ONLY = 200026;
        public const int RTM_EC_INVALID_AUTH_TOEKN = 200027;

        public const int RTM_EC_LARGE_MESSAGE_OR_ATTRS = 200030;
        public const int RTM_EC_LARGE_FILE_OR_ATTRS = 200031;
        public const int RTM_EC_TOO_MANY_ITEMS_IN_PARAMETERS = 200032;
        public const int RTM_EC_EMPTY_PARAMETER = 200033;
        public const int RTM_EC_INVALID_PARAMETER = 200034;

        public const int RTM_EC_NOT_IN_ROOM = 200040;
        public const int RTM_EC_NOT_GROUP_MEMBER = 200041;
        public const int RTM_EC_MAX_GROUP_MEMBER_COUNT = 200042;
        public const int RTM_EC_NOT_FRIEND = 200043;
        public const int RTM_EC_BANNED_IN_GROUP = 200044;
        public const int RTM_EC_BANNED_IN_ROOM = 200045;
        public const int RTM_EC_EMPTY_GROUP = 200046;
        public const int RTM_EC_MAX_ROOM_COUNT = 200047;
        public const int RTM_EC_MAX_FRIEND_COUNT = 200048;
        public const int RTM_EC_BLOCKED_USER = 200049;

        public const int RTM_EC_UNSUPPORTED_LANGUAGE = 200050;
        public const int RTM_EC_EMPTY_TRANSLATION = 200051;
        public const int RTM_EC_SEND_TO_SELF = 200052;
        public const int RTM_EC_DUPLCATED_MID = 200053;
        public const int RTM_EC_SENSITIVE_WORDS = 200054;
        public const int RTM_EC_NOT_ONLINE = 200055;
        public const int RTM_EC_TRANSLATION_ERROR = 200056;
        public const int RTM_EC_PROFANITY_STOP = 200057;
        public const int RTM_EC_NO_CONFIG_IN_CONSOLE = 200060;
        public const int RTM_EC_UNSUPPORTED_TRASNCRIBE_TYPE = 200061;

        public const int RTM_EC_MESSAGE_NOT_FOUND = 200070;

        public const int RTM_EC_UNKNOWN_ERROR = 200999;
    }
}
