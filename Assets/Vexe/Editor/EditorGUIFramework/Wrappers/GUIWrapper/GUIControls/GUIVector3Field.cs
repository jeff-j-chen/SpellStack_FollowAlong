using UnityEngine;
using UnityEditor;

namespace EditorGUIFramework
{
	public class GUIVector3Field : GUIUnityStructField<Vector3>
	{
		protected override Vector3 DoField(Rect rect, GUIContent content, Vector3 value)
		{
			return EditorGUI.Vector3Field(rect, content, value);
		}
	}
}