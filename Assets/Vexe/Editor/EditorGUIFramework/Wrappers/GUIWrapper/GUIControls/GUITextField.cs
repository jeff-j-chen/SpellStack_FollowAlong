using UnityEditor;
using UnityEngine;

namespace EditorGUIFramework
{
	public class GUITextField : GUIPrimitiveField<string>
	{
		protected override string DoField(Rect rect, GUIContent content, string value, GUIStyle style)
		{
			return EditorGUI.TextField(rect, content, value, style);
		}
	}
}