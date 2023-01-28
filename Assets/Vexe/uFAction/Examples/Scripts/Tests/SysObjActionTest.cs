using UnityEngine;
using System;
using uFAction;

/// <summary>
/// A test class for SysObjAction
/// With SysObjAction/Func, you could target any _non_ UnityEngine.Object object
/// The target must also not contain any UnityEngine.Object otherwise serialization will fail
/// SysObj delegates (Actions/Funcs) don't have full read/write editor support like UnityDelegates,
/// they only have a Readonly-Editor which will show up when you use ShowDelegate
/// </summary>
public class SysObjActionTest : MonoBehaviour
{
	[ShowDelegate("On Paramless")]
	public SysObjAction onParamlessTest = new SysObjAction();

	[ShowDelegate("On Param")]
	public SysObjActionString onParamTest = new SysObjActionString();

	private bool paramlessTest;

	[HideInInspector]
	public NonUnityEngineObjectTarget test = new NonUnityEngineObjectTarget();

	public void PublicParamlessMethodInAMonoBehaviour()
	{
		print("PublicParamlessMethodInAMonoBehaviour");
	}

	void OnGUI()
	{
		if (paramlessTest)
			ParamlessTest();
		else ParamTest();
	}

	private void ParamTest()
	{
		if (GUILayout.Button("Switch to ParamlessTest"))
			paramlessTest = true;

		if (GUILayout.Button("Add public test method"))
			onParamTest.Add(test.Say);

		if (GUILayout.Button("Remove public test method"))
			onParamTest.Remove(test.Say);

		if (GUILayout.Button("Set to public test method"))
			onParamTest.Set(test.Say);

		if (GUILayout.Button("is public test method contained?"))
			print(onParamTest.Contains(test.Say));

		if (GUILayout.Button("Clear delegate"))
			onParamTest.Clear();

		if (GUILayout.Button("invoke delegate with `Hello`"))
			onParamTest.Invoke("Hello");
	}

	private void ParamlessTest()
	{
		if (GUILayout.Button("Switch to ParamTest"))
			paramlessTest = false;

		if (GUILayout.Button("Add public method from MonoBehaviour"))
			onParamlessTest += PublicParamlessMethodInAMonoBehaviour;

		if (GUILayout.Button("Add public test method"))
			onParamlessTest.Add(test.PublicParamlessMethod);

		if (GUILayout.Button("Remove public test method"))
			onParamlessTest -= test.PublicParamlessMethod;

		if (GUILayout.Button("Set to public test method"))
			onParamlessTest.Set(test.PublicParamlessMethod);

		if (GUILayout.Button("is public test method contained?"))
			print(onParamlessTest.Contains(test.PublicParamlessMethod));

		if (GUILayout.Button("Clear delegate"))
			onParamlessTest.Clear();

		if (GUILayout.Button("Add static test method"))
			onParamlessTest.Add(NonUnityEngineObjectTarget.StaticParamlessMethod);

		if (GUILayout.Button("Invoke"))
			onParamlessTest.Invoke();
	}

	[Serializable]
	public class SysObjActionString : SysObjAction<string> { }

	[Serializable]
	public class NonUnityEngineObjectTarget
	{
		//target must not contain any UnityEngine.Object for serialization to succeed
		//public Transform t;

		public void PublicParamlessMethod()
		{
			print("PublicParamlessMethod");
		}

		public static void StaticParamlessMethod()
		{
			print("StaticParamlessMethod");
		}

		public void Say(string what)
		{
			print(what);
		}
	}
}