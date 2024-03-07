using System.Collections;
using System.Collections.Generic;
using com.fpnn;
using com.fpnn.rtm;
using UnityEngine;
using UnityEditor;

public class ExportUnityPackage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    [MenuItem("AssetDatabase/Export")]
    static void Export()
    {
        var exportedPackageAssetList = new List<string>();
        exportedPackageAssetList.Add("Assets/Examples");
        exportedPackageAssetList.Add("Assets/Main.cs");
        exportedPackageAssetList.Add("Assets/ErrorRecorder.cs");
        exportedPackageAssetList.Add("Assets/RTMExampleQuestProcessor.cs");
        exportedPackageAssetList.Add("Assets/Scenes/SampleScene.unity");
        exportedPackageAssetList.Add("Assets/Plugins/Android");
        exportedPackageAssetList.Add("Assets/Plugins/IOS");
        exportedPackageAssetList.Add("Assets/Plugins/MacOS");
        exportedPackageAssetList.Add("Assets/Plugins/fpnn");
        exportedPackageAssetList.Add("Assets/Plugins/rtm");
        exportedPackageAssetList.Add("Assets/Plugins/x86");
        exportedPackageAssetList.Add("Assets/Plugins/x86_64");
        string fileName = "Dist/rtm-sdk-unity-"+ RTMConfig.SDKVersion +"-with-fpnn-sdk-" + Config.Version + ".unitypackage";
        AssetDatabase.ExportPackage(exportedPackageAssetList.ToArray(), fileName,
            ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
    }
}
