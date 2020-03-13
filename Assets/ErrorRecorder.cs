using System;
using UnityEngine;

class ErrorRecorder : com.fpnn.common.ErrorRecorder
{
    public void RecordError(Exception e)
    {
        Debug.Log("Exception: " + e.ToString());
    }
    public void RecordError(string message)
    {
        Debug.Log("Error: " + message);
    }
    public void RecordError(string message, Exception e)
    {
        Debug.Log("Error: " + message + ", exception: " + e.ToString());
    }
}
