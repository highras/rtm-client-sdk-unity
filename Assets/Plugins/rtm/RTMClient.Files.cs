using System;
using System.Text;
using System.Security.Cryptography;
using com.fpnn.proto;

namespace com.fpnn.rtm
{
    public partial class RTMClient
    {
        private enum FileTokenType
        {
            P2P,
            Group,
            Room
        }

        private class SendFileInfo
        {
            public FileTokenType actionType;

            public long xid;
            public byte mtype;
            public byte[] fileContent;
            public string filename;
            public string fileExtension;
            
            public string token;
            public string endpoint;
            public int remainTimeout;
            public long lastActionTimestamp;
            public ActTimeDelegate callback;
        }

        //===========================[ File Token ]=========================//
        //-- Action<token, endpoint, errorCode>
        private bool FileToken(Action<string, string, int> callback, FileTokenType tokenType, long xid, int timeout = 0)
        {
            TCPClient client = GetCoreClient();
            if (client == null)
                return false;

            Quest quest = new Quest("filetoken");
            switch (tokenType)
            {
                case FileTokenType.P2P:
                    quest.Param("cmd", "sendfile");
                    quest.Param("to", xid);
                    break;

                case FileTokenType.Group:
                    quest.Param("cmd", "sendgroupfile");
                    quest.Param("gid", xid);
                    break;

                case FileTokenType.Room:
                    quest.Param("cmd", "sendroomfile");
                    quest.Param("rid", xid);
                    break;
            }

            return client.SendQuest(quest, (Answer answer, int errorCode) => {

                string token = "";
                string endpoint = "";
                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        token = answer.Want<string>("token");
                        endpoint = answer.Want<string>("endpoint");
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }
                callback(token, endpoint, errorCode);
            }, timeout);
        }

        private int FileToken(out string token, out string endpoint, FileTokenType tokenType, long xid, int timeout = 0)
        {
            token = "";
            endpoint = "";

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            Quest quest = new Quest("filetoken");
            switch (tokenType)
            {
                case FileTokenType.P2P:
                    quest.Param("cmd", "sendfile");
                    quest.Param("to", xid);
                    break;

                case FileTokenType.Group:
                    quest.Param("cmd", "sendgroupfile");
                    quest.Param("gid", xid);
                    break;

                case FileTokenType.Room:
                    quest.Param("cmd", "sendroomfile");
                    quest.Param("rid", xid);
                    break;
            }

            Answer answer = client.SendQuest(quest, timeout);
            if (answer.IsException())
                return answer.ErrorCode();

            try
            {
                token = answer.Want<string>("token");
                endpoint = answer.Want<string>("endpoint");
                return fpnn.ErrorCode.FPNN_EC_OK;
            }
            catch (Exception)
            {
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
            }
        }

        //===========================[ File Utilies ]=========================//
        private void UpdateTimeout(ref int timeout, ref long lastActionTimestamp)
        {
            long currMsec = ClientEngine.GetCurrentMilliseconds();

            timeout -= (int)((currMsec - lastActionTimestamp) / 1000);

            lastActionTimestamp = currMsec;
        }

        private string ExtraFileExtension(string filename)
        {
            int idx = filename.LastIndexOf('.');
            if (idx == -1)
                return null;

            return filename.Substring(idx + 1);
        }

        private string GetMD5(string str, bool upper)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(str);
            return GetMD5(inputBytes, upper);
        }

        private string GetMD5(byte[] bytes, bool upper)
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(bytes);
            string f = "x2";

            if (upper)
            {
                f = "X2";
            }

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString(f));
            }

            return sb.ToString();
        }

        private string BuildFileAttrs(SendFileInfo info)
        {
            string fileMD5 = GetMD5(info.fileContent, false);
            string sign = GetMD5(fileMD5 + ":" + info.token, false);

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"sign\":\"");
            sb.Append(sign);
            sb.Append("\"");

            if (info.filename != null && info.filename.Length > 0)
            {
                sb.Append(",\"filename\":\"");
                sb.Append(info.filename);
                sb.Append("\"");

                if (info.fileExtension == null || info.fileExtension.Length == 0)
                {
                    info.fileExtension = ExtraFileExtension(info.filename);
                }
            }

            if (info.fileExtension != null && info.fileExtension.Length > 0)
            {
                sb.Append(",\"ext\":\"");
                sb.Append(info.fileExtension);
                sb.Append("\"");
            }

            sb.Append("}");

            return sb.ToString();
        }

        private Quest BuildSendFileQuest(SendFileInfo info)
        {
            Quest quest = null;
            switch (info.actionType)
            {
                case FileTokenType.P2P:
                    quest = new Quest("sendfile");
                    quest.Param("to", info.xid);
                    break;

                case FileTokenType.Group:
                    quest = new Quest("sendgroupfile");
                    quest.Param("gid", info.xid);
                    break;

                case FileTokenType.Room:
                    quest = new Quest("sendroomfile");
                    quest.Param("rid", info.xid);
                    break;
            }

            quest.Param("pid", projectId);
            quest.Param("from", uid);
            quest.Param("token", info.token);
            quest.Param("mtype", info.mtype);
            quest.Param("mid", MidGenerator.Gen());

            quest.Param("file", info.fileContent);
            quest.Param("attrs", BuildFileAttrs(info));

            return quest;
        }

        private int SendFileWithClient(SendFileInfo info, TCPClient client)
        {
            UpdateTimeout(ref info.remainTimeout, ref info.lastActionTimestamp);
            if (info.remainTimeout <= 0)
                return fpnn.ErrorCode.FPNN_EC_CORE_TIMEOUT;

            Quest quest = BuildSendFileQuest(info);
            bool success = client.SendQuest(quest, (Answer answer, int errorCode) => {

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                {
                    try
                    {
                        long mtime = answer.Want<long>("mtime");
                        info.callback(mtime, fpnn.ErrorCode.FPNN_EC_OK);

                        RTMControlCenter.ActiveFileGateClient(info.endpoint, client);
                        return;
                    }
                    catch (Exception)
                    {
                        errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_PACKAGE;
                    }
                }

                info.callback(0, errorCode);
            }, info.remainTimeout);

            if (success)
                return fpnn.ErrorCode.FPNN_EC_OK;
            else
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;
        }

        private int SendFileWithoutClient(SendFileInfo info, bool originalEndpoint)
        {
            string fileGateEndpoint;
            if (originalEndpoint)
                fileGateEndpoint = info.endpoint;
            else
            {
                if (ConvertIPv4EndpointToIPv6IPPort(info.endpoint, out string ipv6, out int port))
                {
                    fileGateEndpoint = ipv6 + ":" + port;
                }
                else
                {
                    return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;
                }
            }

            TCPClient client = TCPClient.Create(fileGateEndpoint, true);
            if (errorRecorder != null)
                client.SetErrorRecorder(errorRecorder);

            client.SetConnectionConnectedDelegate((Int64 connectionId, string endpoint, bool connected) => {
                int errorCode = fpnn.ErrorCode.FPNN_EC_OK;

                if (connected)
                {
                    RTMControlCenter.ActiveFileGateClient(info.endpoint, client);
                    errorCode = SendFileWithClient(info, client);
                }
                else if (originalEndpoint)
                {
                    errorCode = SendFileWithoutClient(info, false);
                }
                else
                {
                    errorCode = fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;
                }

                if (errorCode != fpnn.ErrorCode.FPNN_EC_OK)
                    info.callback(0, errorCode);
            });

            client.AsyncConnect();

            return fpnn.ErrorCode.FPNN_EC_OK;
        }

        private void GetFileTokenCallback(SendFileInfo info, string token, string endpoint, int errorCode)
        {
            if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
            {
                info.token = token;
                info.endpoint = endpoint;

                TCPClient fileClient = RTMControlCenter.FecthFileGateClient(info.endpoint);
                if (fileClient != null)
                    errorCode = SendFileWithClient(info, fileClient);
                else
                    errorCode = SendFileWithoutClient(info, true);

                if (errorCode == fpnn.ErrorCode.FPNN_EC_OK)
                    return;
            }
            else
                info.callback(0, errorCode);
        }

        //===========================[ Real Send File ]=========================//
        private bool RealSendFile(ActTimeDelegate callback, FileTokenType tokenType, long targetId, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            if (mtype < 40 || mtype > 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Send file require mtype between [40, 50], current mtype is " + mtype);

                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, ErrorCode.RTM_EC_INVALID_MTYPE);
                    });

                return false;
            }

            TCPClient client = GetCoreClient();
            if (client == null)
            {
                if (RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                    ClientEngine.RunTask(() =>
                    {
                        callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                    });

                return false;
            }

            SendFileInfo info = new SendFileInfo
            {
                actionType = tokenType,
                xid = targetId,
                mtype = mtype,
                fileContent = fileContent,
                filename = filename,
                fileExtension = fileExtension,
                remainTimeout = timeout,
                lastActionTimestamp = ClientEngine.GetCurrentMilliseconds(),
                callback = callback
            };

            bool asyncStarted = FileToken((string token, string endpoint, int errorCode) => {
                GetFileTokenCallback(info, token, endpoint, errorCode);
            }, tokenType, info.xid, timeout);

            if (!asyncStarted && RTMConfig.triggerCallbackIfAsyncMethodReturnFalse)
                ClientEngine.RunTask(() =>
                {
                    callback(0, fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION);
                });

            return asyncStarted;
        }

        private int RealSendFile(out long mtime, FileTokenType tokenType, long targetId, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            mtime = 0;

            //----------[ 1. check mtype ]---------------//

            if (mtype < 40 || mtype > 50)
            {
                if (errorRecorder != null)
                    errorRecorder.RecordError("Send file require mtype between [40, 50], current mtype is " + mtype);

                return ErrorCode.RTM_EC_INVALID_MTYPE;
            }

            //----------[ 2. Get File Token ]---------------//

            TCPClient client = GetCoreClient();
            if (client == null)
                return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;

            long lastActionTimestamp = ClientEngine.GetCurrentMilliseconds();

            int errorCode = FileToken(out string token, out string endpoint, tokenType, targetId, timeout);
            if (errorCode != fpnn.ErrorCode.FPNN_EC_OK)
                return errorCode;

            //----------[ 2.1 check timeout ]---------------//

            UpdateTimeout(ref timeout, ref lastActionTimestamp);
            if (timeout <= 0)
                return fpnn.ErrorCode.FPNN_EC_CORE_TIMEOUT;

            //----------[ 3. fetch file gate client ]---------------//

            TCPClient fileClient = RTMControlCenter.FecthFileGateClient(endpoint);
            if (fileClient == null)
            {
                fileClient = TCPClient.Create(endpoint, true);
                if (fileClient.SyncConnect())
                {
                    RTMControlCenter.ActiveFileGateClient(endpoint, fileClient);
                }
                else
                {
                    //----------[ 3.1 check timeout ]---------------//
                    UpdateTimeout(ref timeout, ref lastActionTimestamp);
                    if (timeout <= 0)
                        return fpnn.ErrorCode.FPNN_EC_CORE_TIMEOUT;

                    if (ConvertIPv4EndpointToIPv6IPPort(endpoint, out string ipv6, out int port))
                    {
                        fileClient = TCPClient.Create(ipv6 + ":" + port, true);
                        if (fileClient.SyncConnect())
                        {
                            RTMControlCenter.ActiveFileGateClient(endpoint, fileClient);
                        }
                        else
                        {
                            return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;
                        }
                    }
                    else
                    {
                        return fpnn.ErrorCode.FPNN_EC_CORE_INVALID_CONNECTION;
                    }
                }
            }

            //----------[ 3.2 check timeout ]---------------//

            UpdateTimeout(ref timeout, ref lastActionTimestamp);
            if (timeout <= 0)
                return fpnn.ErrorCode.FPNN_EC_CORE_TIMEOUT;

            //----------[ 4. build quest ]---------------//
            SendFileInfo info = new SendFileInfo
            {
                actionType = tokenType,
                xid = targetId,
                mtype = mtype,
                fileContent = fileContent,
                filename = filename,
                fileExtension = fileExtension,
                token = token,
            };

            Quest quest = BuildSendFileQuest(info);
            Answer answer = fileClient.SendQuest(quest, timeout);

            if (answer.IsException())
                return answer.ErrorCode();

            RTMControlCenter.ActiveFileGateClient(endpoint, fileClient);

            mtime = answer.Want<long>("mtime");
            return fpnn.ErrorCode.FPNN_EC_OK;
        }

        //===========================[ Sned File ]=========================//
        public bool SendFile(ActTimeDelegate callback, long peerUid, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            return RealSendFile(callback, FileTokenType.P2P, peerUid, mtype, fileContent, filename, fileExtension, timeout);
        }

        public int SendFile(out long mtime, long peerUid, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            return RealSendFile(out mtime, FileTokenType.P2P, peerUid, mtype, fileContent, filename, fileExtension, timeout);
        }

        //-- New interface: mtype in MessageType enumeration.
        public bool SendFile(ActTimeDelegate callback, long peerUid, MessageType type, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            return RealSendFile(callback, FileTokenType.P2P, peerUid, (byte)type, fileContent, filename, fileExtension, timeout);
        }

        public int SendFile(out long mtime, long peerUid, MessageType type, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            return RealSendFile(out mtime, FileTokenType.P2P, peerUid, (byte)type, fileContent, filename, fileExtension, timeout);
        }

        //===========================[ Sned Group File ]=========================//
        public bool SendGroupFile(ActTimeDelegate callback, long groupId, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            return RealSendFile(callback, FileTokenType.Group, groupId, mtype, fileContent, filename, fileExtension, timeout);
        }

        public int SendGroupFile(out long mtime, long groupId, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            return RealSendFile(out mtime, FileTokenType.Group, groupId, mtype, fileContent, filename, fileExtension, timeout);
        }

        //-- New interface: mtype in MessageType enumeration.
        public bool SendGroupFile(ActTimeDelegate callback, long groupId, MessageType type, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            return RealSendFile(callback, FileTokenType.Group, groupId, (byte)type, fileContent, filename, fileExtension, timeout);
        }

        public int SendGroupFile(out long mtime, long groupId, MessageType type, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            return RealSendFile(out mtime, FileTokenType.Group, groupId, (byte)type, fileContent, filename, fileExtension, timeout);
        }

        //===========================[ Sned Room File ]=========================//
        public bool SendRoomFile(ActTimeDelegate callback, long roomId, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            return RealSendFile(callback, FileTokenType.Room, roomId, mtype, fileContent, filename, fileExtension, timeout);
        }

        public int SendRoomFile(out long mtime, long roomId, byte mtype, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            return RealSendFile(out mtime, FileTokenType.Room, roomId, mtype, fileContent, filename, fileExtension, timeout);
        }

        //-- New interface: mtype in MessageType enumeration.
        public bool SendRoomFile(ActTimeDelegate callback, long roomId, MessageType type, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            return RealSendFile(callback, FileTokenType.Room, roomId, (byte)type, fileContent, filename, fileExtension, timeout);
        }

        public int SendRoomFile(out long mtime, long roomId, MessageType type, byte[] fileContent, string filename, string fileExtension = "", int timeout = 120)
        {
            return RealSendFile(out mtime, FileTokenType.Room, roomId, (byte)type, fileContent, filename, fileExtension, timeout);
        }
    }
}
