using UnityEngine;
using uFAction;
using System;
using Vexe.Runtime.Types;
using Vexe.Runtime.Helpers;

public class KickassDelegateTest : MonoBehaviour
{
	[ShowDelegate("On A** Kicked")]
	public KickassDelegate kickass = new KickassDelegate();
	private string privateString { get { return "SecretMessage"; } }

	void OnGUI()
	{
		// Will fail - must cast value to int
		if (GUILayout.Button("Add with float"))
		{
			kickass.AddUsingValues((Action<int>)MethodThatTakesAnInt, 1.3f);
		}

		// Will succeed
		if (GUILayout.Button("Add with valid values"))
		{
			kickass.AddUsingValues((Action<string>)Say, "What?");
		}

		// will throw an ArgumentTypeMismatch exception
		if (GUILayout.Button("Add with invalid value type"))
		{
			kickass.AddUsingValues((Action<string>)Say, 1);
		}

		// will throw an ArgumentNumberMismatch exception
		if (GUILayout.Button("Add with invalid number of values"))
		{
			kickass.AddUsingValues((Action<string>)Say, "String", 1.4f, 1002);
		}

		// Will succeed
		if (GUILayout.Button("Add with valid sources"))
		{
			kickass.AddUsingSource((Action<Vector3, int>)InversePositionAndMultiply,
				new SourceSet(transform, "position"),
				new SourceSet(transform, "childCount"));
		}

		// will throw an ArgumentException cause there's no such property as "psh!" inside of transform
		if (GUILayout.Button("Add with invalid sources"))
		{
			kickass.AddUsingSource((Action<Vector3, int>)InversePositionAndMultiply,
				new SourceSet(transform, "psh!"),
				new SourceSet(transform, "childCount"));
		}

		// will throw an ArgumentNumberMismatch exception
		if (GUILayout.Button("Add with invalid number of sources"))
		{
			kickass.AddUsingSource((Action<Vector3, int>)InversePositionAndMultiply,
				new SourceSet(this, "cool"),
				new SourceSet(transform, "psh!"),
				new SourceSet(transform, "childCount"));
		}

		// for this to succeed, `NonPublic` must be ticked from source bindings in the settings
		if (GUILayout.Button("Add with a private source"))
		{
			kickass.AddUsingSource((Action<string>)Say,
				new SourceSet(this, "privateString")
			);
		}

		// for this to succeed, `DeclaredOnly` must be unticked from source bindings in the settings
		if (GUILayout.Button("Add with a source declared in higher hierarchy"))
		{
			kickass.AddUsingSource((Action<string>)Say,
				new SourceSet(this, "name")
			);
		}

		if (GUILayout.Button("Add with a method source"))
		{
			kickass.AddUsingSource((Action<Vector3, int>)InversePositionAndMultiply,
				new SourceSet(this, "GetVector"),
				new SourceSet(transform, "childCount"));
		}

		if (GUILayout.Button("Add void method"))
		{
			kickass.Add((Action)Ping);
		}

		if (GUILayout.Button("Remove void method"))
		{
			kickass.Remove((Action)Ping);
		}

		if (GUILayout.Button("Contains void method?"))
		{
			print(kickass.Contains((Action)Ping));
		}

		if (GUILayout.Button("Invoke"))
			kickass.InvokeWithEditorArgs();
	}

	public void MethodThatTakesAnInt(int x) { }
	public Vector3 GetVector()
	{
		return Vector3.one;
	}
	public void Ping()
	{
		Say("Ping");
	}
	public void Say(string something) { print(something); }
	public void InversePositionAndMultiply(Vector3 v, int n)
	{
		transform.position = -v * n;
	}
}