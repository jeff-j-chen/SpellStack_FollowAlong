using UnityEngine;
using uFAction;

public class UnityActionTest : MonoBehaviour
{
	[ShowDelegate("On Paramless Test")]
	public UnityAction onParamlessTest = new UnityAction();

	[ShowDelegate("On ParamTest")]
	public Vector3Action onParamTest = new Vector3Action();

	private bool paramlessTest;

	public void PublicParamlessMethod()
	{
		print("PublicParamlessMethod");
	}

	private void PrivateParamlessMethod()
	{
		print("PrivateParamlessMethod");
	}

	public static void StaticParamlessMethod()
	{
		print("StaticParamlessMethod");
	}

	public void Vector3Method1(Vector3 v)
	{
		//print("Vector3Method: " + v);
	}

	public void Vector3Method2(Vector3 v)
	{
		//print("Vector3Method2: " + v);
	}

	public void Vector3Method3(Vector3 v)
	{
		//print("Vector3Method3: " + v);
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
			onParamTest.Add(Vector3Method1);

		if (GUILayout.Button("Remove public test method"))
			onParamTest.Remove(Vector3Method1);

		if (GUILayout.Button("Set public test method"))
			onParamTest.Set(Vector3Method1);

		if (GUILayout.Button("is public test method contained?"))
			print(onParamTest.Contains(Vector3Method1));

		if (GUILayout.Button("Clear delegate"))
			onParamTest.Clear();

		if (GUILayout.Button("invoke delegate with (1, 2, 3)"))
			onParamTest.Invoke(new Vector3(1, 2, 3));
	}

	private void ParamlessTest()
	{
		if (GUILayout.Button("Switch to ParamTest"))
			paramlessTest = false;

		// This will fail - lamda expressions / anonymous methods are not supported
		if (GUILayout.Button("Add via lamda expression"))
			onParamlessTest.Add(() => print("ANON"));

		if (GUILayout.Button("Set public test method"))
			onParamlessTest.Set(PublicParamlessMethod);

		if (GUILayout.Button("Add public test method"))
			onParamlessTest += PublicParamlessMethod;

		if (GUILayout.Button("Remove public test method"))
			onParamlessTest -= PublicParamlessMethod;

		if (GUILayout.Button("is public test method contained?"))
			print(onParamlessTest.Contains(PublicParamlessMethod));

		if (GUILayout.Button("Clear delegate"))
			onParamlessTest.Clear();

		// This will fail if `NonPublic` was not ticked in the method bindings
		if (GUILayout.Button("Add private test method"))
			onParamlessTest.Add(PrivateParamlessMethod);

		if (GUILayout.Button("Remove private test method"))
			onParamlessTest.Remove(PrivateParamlessMethod);

		if (GUILayout.Button("Add static test method"))
			onParamlessTest.Add(StaticParamlessMethod);

		// This will fail if `DeclaredOnly` was ticked in the method bindings
		if (GUILayout.Button("Add CancelInvoke (inherited method)"))
			onParamlessTest += CancelInvoke;

		if (GUILayout.Button("Invoke"))
			onParamlessTest.Invoke();
	}
}