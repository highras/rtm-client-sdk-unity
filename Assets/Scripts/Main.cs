using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.test;

public class Main : MonoBehaviour
{

    private TestCase _testCase;

    // Start is called before the first frame update
    void Start() {

        Debug.Log("hello rtm!");

        // TestCase
        this.BaseTest();
    }

    // Update is called once per frame
    void Update() {
        
    }

    void BaseTest() {

        this._testCase = new TestCase();
    }

    void OnApplicationQuit() {

        if (this._testCase != null) {

            this._testCase.Close();
        }
    }

    void OnApplicationPause() {
    	
        if (this._testCase != null) {

            this._testCase.Close();
        }
    }
}
