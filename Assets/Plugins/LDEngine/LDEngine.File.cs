using UnityEngine;
using System.Collections;
using com.fpnn.rtm;
using com.fpnn;
using System.Collections.Generic;
using com.fpnn.common;
using System.IO;
using System;
using com.fpnn.msgpack;

namespace com.fpnn.livedata
{
    static public class LDEngineFile
    {
        static string persistentDataPath;
        static readonly string LDEngineRootFolder = "/com.fpnn.ldengine/";
        static readonly string LDEngineFileName = "LDEngineCache";

        internal static void Init()
        {
            persistentDataPath = Application.persistentDataPath;
            InitDirectory();
        }

        private static void CheckDirecotry(string path)
        {
            if (Directory.Exists(path))
                return;

            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception e)
            {
                LDEngine.ErrorLog("Create path for LDEngine failed. Path = " + path + ", exctption = " + e.ToString());
                throw e;
            }
        }

        private static void InitDirectory()
        {
            string path = persistentDataPath + LDEngineRootFolder;
            try
            {
                CheckDirecotry(path);
            }
            catch (Exception e)
            {
                LDEngine.ErrorLog("Init path for LDEngine failed. Path = " + path + ", excetption = " + e.ToString());
                return;
            }
        }

        private static byte[] LoadBinaryFile(string path)
        {
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException)
                    return null;

                LDEngine.ErrorLog("Load binary file failed. Path = " + path + ", exception = " + e.ToString());
            }
            return null;
        }

        private static bool SaveBinaryFile(string path, byte[] data)
        {
            try
            {
                File.WriteAllBytes(path, data);
                return true;
            }
            catch (Exception e)
            {
                LDEngine.ErrorLog("Save binary file failed. Path = " + path + ", exception = " + e.ToString());
            }
            return false;
        }

        private static bool DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        internal static void SaveCacheFile(string path, Dictionary<object, object> values)
        {
            byte[] rawData;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    MsgPacker.PackDictionary(stream, values);
                    rawData = stream.ToArray();
                }
                SaveBinaryFile(path, rawData);
            }
            catch (Exception e)
            {
                LDEngine.ErrorLog("Save cache file failed. Exception = " + e.ToString());
            }
        }

        internal static Dictionary<object, object> LoadCacheFile(string path)
        {
            byte[] data = LoadBinaryFile(path);
            if (data == null)
                return null;
            try
            {
                object obj = MsgUnpacker.Unpack(data);
                if (obj is Dictionary<object, object> dict)
                    return dict;
            }
            catch (Exception e)
            {
                LDEngine.ErrorLog("Load cache file failed. Exception = " + e.ToString());
            }
            return null;
        }
    }
}
