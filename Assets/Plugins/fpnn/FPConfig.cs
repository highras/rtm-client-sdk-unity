using System;

namespace com.fpnn {

    public class FPConfig {

        public static string VERSION = "1.1.1";

        public static byte[] FPNN_VERSION = definedVersion();
        public static byte[] FP_FLAG = definedFlag();
        public static byte[] FP_MESSAGE_TYPE = definedMsgType();
        public static byte[] TCP_MAGIC = definedTcpMagic();
        public static byte[] HTTP_MAGIC = definedHttpMagic();

        public static int READ_BUFFER_LEN = 12;
        public static int SEND_TIMEOUT = 20 * 1000;

        private static byte[] definedVersion() {

            byte[] bytes = new byte[2];
            bytes[0] = (byte) 0x0;
            bytes[1] = (byte) 0x1;

            return bytes;
        }

        private static byte[] definedFlag() {

            byte b = 0;
            b |= 0x80;

            byte[] bytes = new byte[2];
            bytes[0] = (byte) 0x40;         // 64: FP_FLAG_JSON
            bytes[1] = b;                   // 128: FP_FLAG_MSGPACK

            return bytes;
        }

        private static byte[] definedMsgType() {

            byte[] bytes = new byte[3];
            bytes[0] = (byte) 0x0;          // 0: FP_MT_ONEWAY
            bytes[1] = (byte) 0x1;          // 1: FP_MT_TWOWAY
            bytes[2] = (byte) 0x2;          // 2: FP_MT_ANSWER

            return bytes;
        }

        private static byte[] definedTcpMagic() {

            return System.Text.Encoding.UTF8.GetBytes("FPNN");
        }

        private static byte[] definedHttpMagic() {

            return System.Text.Encoding.UTF8.GetBytes("POST");
        }

        public class ERROR_CODE {

            public static int FPNN_EC_PROTO_UNKNOWN_ERROR = 10001;        // 未知错误(协议解析错误)
            public static int FPNN_EC_PROTO_NOT_SUPPORTED = 10002;        // 不支持的协议
            public static int FPNN_EC_PROTO_INVALID_PACKAGE = 10003;      // 无效的数据包
            public static int FPNN_EC_PROTO_JSON_CONVERT = 10004;         // JSON转换错误
            public static int FPNN_EC_PROTO_STRING_KEY = 10005;           // 数据包错误
            public static int FPNN_EC_PROTO_MAP_VALUE = 10006;            // 数据包错误
            public static int FPNN_EC_PROTO_METHOD_TYPE = 10007;          // 请求错误
            public static int FPNN_EC_PROTO_PROTO_TYPE = 10008;           // 协议类型错误
            public static int FPNN_EC_PROTO_KEY_NOT_FOUND = 10009;        // 数据包错误
            public static int FPNN_EC_PROTO_TYPE_CONVERT = 10010;         // 数据包转换错误

            public static int FPNN_EC_CORE_UNKNOWN_ERROR = 20001;         // 未知错误(业务流程异常中断)
            public static int FPNN_EC_CORE_CONNECTION_CLOSED = 20002;     // 链接已关闭
            public static int FPNN_EC_CORE_TIMEOUT = 20003;               // 请求超时
            public static int FPNN_EC_CORE_UNKNOWN_METHOD = 20004;        // 错误的请求
            public static int FPNN_EC_CORE_ENCODING = 20005;              // 编码错误
            public static int FPNN_EC_CORE_DECODING = 20006;              // 解码错误
            public static int FPNN_EC_CORE_SEND_ERROR = 20007;            // 发送错误
            public static int FPNN_EC_CORE_RECV_ERROR = 20008;            // 接收错误
            public static int FPNN_EC_CORE_INVALID_PACKAGE = 20009;       // 无效的数据包
            public static int FPNN_EC_CORE_HTTP_ERROR = 20010;            // HTTP错误
            public static int FPNN_EC_CORE_WORK_QUEUE_FULL = 20011;       // 任务队列满
            public static int FPNN_EC_CORE_INVALID_CONNECTION = 20012;    // 无效的链接
            public static int FPNN_EC_CORE_FORBIDDEN = 20013;             // 禁止操作
            public static int FPNN_EC_CORE_SERVER_STOPPING = 20014;       // 服务器即将停止
        }
    }
}