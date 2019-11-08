using System;
using System.IO;
using System.Collections;

using UnityEditor;
using UnityEngine;

public class ExportPackage {

    private const string SDK_PLUGIN_PATH = "Assets/Plugins/rtm/";
    private const string BASE_PACKAGE_PATCH = "rtm-sdk-unity";

    private static string Plugin_Version = com.rtm.RTMConfig.VERSION;

    [MenuItem ("Assets/[LiveData] ExportPlugin")]
    public static void ExportPlugin() {

        Debug.Log("[LiveData] Exporting Unity Package...");
        string path = OutputPath;

        try {

            string[] files = (string[])Directory.GetFiles(SDK_PLUGIN_PATH, "*.*", SearchOption.AllDirectories);
            AssetDatabase.ExportPackage(files, path, ExportPackageOptions.Recurse);
        } catch (Exception ex) {

            Debug.LogError(ex);
        }

        Debug.Log("[LiveData] Finished Exporting! Path: " + path);
    }

    private static string OutputPath {

        get {

            string project_root = Directory.GetCurrentDirectory();
            var output_directory = new DirectoryInfo(Path.Combine(project_root, "Dist"));

            // Create the directory if it doesn't exist
            output_directory.Create();

            string package_name = string.Format("{0}-{1}.unitypackage", BASE_PACKAGE_PATCH, Plugin_Version.Replace(".", "-"));
            return Path.Combine(output_directory.FullName, package_name);
        }
    }
}