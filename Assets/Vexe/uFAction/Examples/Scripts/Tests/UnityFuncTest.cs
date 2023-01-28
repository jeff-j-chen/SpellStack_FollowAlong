using UnityEngine;
using uFAction;
using System;

/// <summary>
/// One thing to note about invoking a func from the editor (via the "I" button) - is that it invokes all the hooked methods yes,
/// but it doesn't return the func's value, since you need to have a variable to assign the value to, and how does the editor know where to assign the returned value?
/// in other words, invoking a func from the editor is like doing:
/// myFunc.Invoke(args, etc);
/// so you can't do:
/// myVar = myFunc.Invoke(args, etc);
/// Just cause it's from the editor
/// 
/// Another thing about funcs in general, is that if you do:
/// myFunc.Add(handler1);
/// myFunc.Add(handler2);
/// var value1 = myFunc.Invoke();
/// var value2 = handler2();
/// value1 will be equal to value2 because handler2 was the last method to be invoked and so the return value we get
/// from invoking the delegate, is actually the return value of handler2
/// </summary>
public class UnityFuncTest : MonoBehaviour
{
	[ShowDelegate("On Paramless")]
	public UnityFuncString onParamlessTest = new UnityFuncString();

	[ShowDelegate("On Param")]
	public UnityVector3FuncVector3 onParamTest = new UnityVector3FuncVector3();

	private bool paramlessTest = true;

	// You could follow another naming convention, I chose to follow: UnityXFuncY where X: ParamTypes, Y: ReturnType
	// just because the last generic arg to a func is its return type and whatever before are the parameter types
	[Serializable]
	public class UnityFuncString : UnityFunc<string> { }

	[Serializable]
	public class UnityVector3FuncVector3 : UnityFunc<Vector3, Vector3> { }

	public string PublicParamlessMethod()
	{
		return "TestString";
	}

	private string PrivateParamlessMethod()
	{
		return "PrivateParamlessMethod";
	}

	public static string StaticParamlessMethod()
	{
		return "StaticParamlessMethod";
	}

	public Vector3 AddVector3One(Vector3 v)
	{
		print("Vector to add: " + v);
		return v + Vector3.one;
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
			onParamTest.Add(AddVector3One);

		if (GUILayout.Button("Remove public test method"))
			onParamTest.Remove(AddVector3One);

		if (GUILayout.Button("Set public test method"))
			onParamTest.Set(AddVector3One);

		if (GUILayout.Button("is public test method contained?"))
			print(onParamTest.Contains(AddVector3One));

		if (GUILayout.Button("Clear delegate"))
			onParamTest.Clear();

		if (GUILayout.Button("invoke delegate with (1, 2, 3) and print the return value"))
			print("Return value: " + onParamTest.Invoke(new Vector3(1, 2, 3)));
	}
	
	private void ParamlessTest()
	{
		if (GUILayout.Button("Switch to ParamTest"))
			paramlessTest = false;

		if (GUILayout.Button("Add public test method"))
			onParamlessTest.Add(PublicParamlessMethod);

		if (GUILayout.Button("Remove public test method"))
			onParamlessTest.Remove(PublicParamlessMethod);

		if (GUILayout.Button("Set public test method"))
			onParamlessTest.Set(PublicParamlessMethod);

		if (GUILayout.Button("is public test method contained?"))
			print(onParamlessTest.Contains(PublicParamlessMethod));

		if (GUILayout.Button("Clear delegate"))
			onParamlessTest.Clear();

		if (GUILayout.Button("Add private test method"))
			onParamlessTest.Add(PrivateParamlessMethod);

		if (GUILayout.Button("Remove private test method"))
			onParamlessTest.Remove(PrivateParamlessMethod);

		if (GUILayout.Button("Add static test method"))
			onParamlessTest.Add(StaticParamlessMethod);

		if (GUILayout.Button("Invoke and print return value"))
			print(onParamlessTest.Invoke());
	}
}