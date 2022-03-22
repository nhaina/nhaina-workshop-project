using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TinyConsole : MonoBehaviour
{
    private Text text;
    private StringBuilder builder;
    private int linesCounter;
	private void Awake()
	{
        text = GetComponent<Text>();
        builder = new StringBuilder();
        linesCounter = 0;
	}

	void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
	{
		if (linesCounter == 0)
		{
            builder.Clear();
		}
        builder.AppendLine(logString);
        linesCounter = (linesCounter + 1) % 10;
        text.text = builder.ToString();
    }
}
