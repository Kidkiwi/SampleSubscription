using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyLog : MonoBehaviour
{
    string myLog;
    List<string> myLogQueue = new List<string>();

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLog = logString;

        string newString = "\n [" + type + "] : " + myLog;

        myLogQueue.Insert(0, (newString));

        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            myLogQueue.Insert(0, (newString));
        }

        myLog = string.Empty;

        foreach (string mylog in myLogQueue)
        {
            myLog += mylog;
        }
    }

    private void Update()
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = myLog;
    }
}