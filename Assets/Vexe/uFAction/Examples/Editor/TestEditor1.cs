using EditorGUIFramework;
using uFAction;
using UnityEditor;

namespace Assets.Vexe.uFAction.Scripts.Examples.Editor
{
	/// <summary>
	/// An example editor to show how you can draw a delegate from editor scripts too
	/// so you're not just constrained to use [ShowDelegate]
	/// </summary>
	[CustomEditor(typeof(Target1))]
	public class TestEditor1 : BetterEditor<Target1>
	{
		// Declare our delegate drawer
		DelegateDrawer<GLWrapper, GLOption> delegateDrawer;

		void OnEnable()
		{
			// Instantiate and intiailize drawer passing it the gui we're using to draw the delegate
			// In this case a GLWrapper (GUILayout-Wrapper) cause we're inside a custom editor
			// GUIWrapper is not approriate to use here.
			delegateDrawer = new DelegateDrawer<GLWrapper, GLOption>(gui)
			{
				spDelegate = serializedObject.FindProperty("del"), // a SerializedProperty referencing the delegate
				delegateObject = GetFieldValue("del"), // The actual delegate object (System.Object)
				title = "Test Delegate" // The title we want
				// We could also pass in the other parameters: canSetArgsFromEditor and forceExpand
			};
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			// Finally, we draw the delegate
			delegateDrawer.Draw();
		}
	}
}