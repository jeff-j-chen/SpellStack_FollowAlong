using UnityEditor;
using UnityEngine;

namespace EditorGUIFramework
{
	public class GUIToggle : GUIPrimitiveField<bool>
	{
		protected override bool DoField(Rect rect, GUIContent content, bool value, GUIStyle style)
		{
			return EditorGUI.Toggle(rect, content, value, style);
		}
	}
}