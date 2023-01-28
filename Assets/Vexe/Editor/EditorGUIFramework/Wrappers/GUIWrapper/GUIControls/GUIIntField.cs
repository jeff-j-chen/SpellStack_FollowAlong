using UnityEditor;
using UnityEngine;

namespace EditorGUIFramework
{
	public class GUIIntField : GUIPrimitiveField<int>
	{
		protected override int DoField(Rect rect, GUIContent content, int value, GUIStyle style)
		{
			return EditorGUI.IntField(rect, content, value, style);
		}
	}
}