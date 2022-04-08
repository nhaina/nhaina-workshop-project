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
		if (type == LogType.Error || type == LogType.Exception)
		{

			string[] splittedSt = stackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < Math.Min(5, splittedSt.Length); i++)
			{
				builder.AppendLine(splittedSt[i]);
			}
			linesCounter += 5;
		}
		linesCounter = (linesCounter + 1) % 10;
		text.text = builder.ToString();
	}
}
