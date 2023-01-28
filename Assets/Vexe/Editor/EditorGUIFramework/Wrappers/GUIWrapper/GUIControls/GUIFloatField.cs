using UnityEditor;
using UnityEngine;

namespace EditorGUIFramework
{
	public class GUIFloatField : GUIPrimitiveField<float>
	{
		protected override float DoField(Rect rect, GUIContent content, float value, GUIStyle style)
		{
			return EditorGUI.FloatField(rect, content, value, style);
		}
	}
}