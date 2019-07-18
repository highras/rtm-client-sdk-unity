using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using com.test;

using UnityEngine;

public class Main : MonoBehaviour
{

    private TestCase _testCase;
    private TestCase _testCase1;

    private SingleClientSend _singleClientSend;
    private SingleClientPush _singleClientPush;

    // Start is called before the first frame update
    void Start() {

        byte[] fileBytes = null;
        // byte[] fileBytes = LoadFile(Application.dataPath + "/StreamingAssets/key/test-secp256k1-public.der");

        //SingleClientSend
        this._singleClientSend = new SingleClientSend();

        //SingleClientPush
        // this._singleClientPush = new SingleClientPush();

        //TestCase
        // this.BaseTest(fileBytes);
    }

    byte[] LoadFile(string filePath) {

        FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        byte[] bytes = new byte[fs.Length];

        fs.Read(bytes, 0, bytes.Length);
        fs.Close();

        return bytes;
    } 

    // Update is called once per frame
    void Update() {
        
    }

    void BaseTest(byte[] fileBytes) {

        this._testCase = new TestCase(777779, "204841DE531E4C39EEF54AC2046A4C4B", fileBytes);
    }

    void OnApplicationQuit() {

        if (this._testCase != null) {

            this._testCase.Close();
        }

        if (this._testCase1 != null) {

            this._testCase1.Close();
        }

        if (this._singleClientSend != null) {

            this._singleClientSend.Close();
        }

        if (this._singleClientPush != null) {

            this._singleClientPush.Close();
        }
    }

    void OnApplicationPause() {
    	
        if (this._testCase != null) {

            this._testCase.Close();
        }
        
        if (this._testCase1 != null) {

            this._testCase1.Close();
        }

        if (this._singleClientSend != null) {

            this._singleClientSend.Close();
        }

        if (this._singleClientPush != null) {

            this._singleClientPush.Close();
        }
    }
}
