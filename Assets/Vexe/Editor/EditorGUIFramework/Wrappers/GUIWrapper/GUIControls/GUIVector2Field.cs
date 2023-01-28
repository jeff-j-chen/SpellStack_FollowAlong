using UnityEngine;
using UnityEditor;

namespace EditorGUIFramework
{
	public class GUIVector2Field : GUIUnityStructField<Vector2>
	{
		protected override Vector2 DoField(Rect rect, GUIContent content, Vector2 value)
		{
			return EditorGUI.Vector2Field(rect, content, value);
		}
	}
}