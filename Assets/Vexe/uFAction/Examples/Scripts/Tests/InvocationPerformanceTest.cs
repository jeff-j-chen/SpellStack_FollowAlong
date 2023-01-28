using UnityEngine;
using uFAction;
using System.Diagnostics;
using System;

public class InvocationPerformanceTest : MonoBehaviour
{
	public int numRuns;

	[ShowDelegate]
	public StringAction testDelegate = new StringAction();

	void InvokeWithEditorArgsMemoized()
	{
		RunTest(testDelegate.InvokeWithEditorArgs, numRuns);
	}

	void InvokeWithEditorArgsNotMemoized()
	{
		RunTest(testDelegate.InvokeWithEditorArgsNotMemoized, numRuns);
	}

	void RegularInvokeWithString()
	{
		RunTest(() => testDelegate.Invoke("Test"), numRuns);
	}

	void OnGUI()
	{
		if (GUILayout.Button("Normal (direct) Invoke"))
		{
			RegularInvokeWithString();
		}
		if (GUILayout.Button("Non-Memoized (reflection) Invoke"))
		{
			InvokeWithEditorArgsNotMemoized();
		}
		if (GUILayout.Button("Memoized (cached) Invoke"))
		{
			InvokeWithEditorArgsMemoized();
		}
	}

	void RunTest(Action test, int nTimes)
	{
		var w = Stopwatch.StartNew();
		for (int i = 0; i < nTimes; i++)
		{
			test();
		}
		w.Stop();
		UnityEngine.Debug.Log(w.ElapsedMilliseconds);
	}
}