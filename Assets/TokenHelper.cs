using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using com.fpnn;
using com.fpnn.proto;
using com.fpnn.rtm;
using UnityEngine;

static public class TokenHelper 
{
    static string secretKey = "00000000-0000-0000-0000-000000000000";
    static string serverGateEndpoint = "rtm-nx-back.ilivedata.com:13315";

    private static string GetMD5(string str, bool upper)
    {
        byte[] inputBytes = Encoding.ASCII.GetBytes(str);
        return GetMD5(inputBytes, upper);
    }

    private static string GetMD5(byte[] bytes, bool upper)
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

    private static long GenerateSalt()
    {
        return MidGenerator.Gen();
    }

    private static Quest GenerateQuest(string cmd, long projectId)
    {
        long ts = ClientEngine.GetCurrentSeconds();
        long salt = GenerateSalt();
        string sign = GetMD5(projectId + ":" + secretKey + ":" + salt + ":" + cmd + ":" + ts, true);
        Quest quest = new Quest(cmd);
        quest.Param("pid", projectId);
        quest.Param("salt", salt);
        quest.Param("sign", sign);
        quest.Param("ts", ts);
        return quest;
    }


    public static string GetToken(long uid, long projectId)
    {
        Quest quest = GenerateQuest("gettoken", projectId);
        quest.Param("uid", uid);
        quest.Param("version", "csharp-" + RTMConfig.SDKVersion);
        TCPClient client = TCPClient.Create(serverGateEndpoint, true);
        client.SetQuestProcessor(new RTMMasterProcessor());

        Answer answer = client.SendQuest(quest);
        string token = answer.Get<string>("token", null);
        client.Close();
        return token;
    }

    static string hmacSecret = "secretkey";
    public static string GetTokenV2(long uid, long projectId, out long ts)
    {
        ts = ClientEngine.GetCurrentSeconds();
        string text = projectId.ToString() + ":" + uid.ToString() + ":" + ts.ToString();
        
        using (var hmacsha256 = new HMACSHA256(Convert.FromBase64String(hmacSecret)))
        {
            var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToBase64String(hash);
        }
    }
}
