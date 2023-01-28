using UnityEngine;
using UnityEditor;

namespace EditorGUIFramework
{
	public class GUIColorField : GUIUnityStructField<Color>
	{
		protected override Color DoField(Rect rect, GUIContent content, Color value)
		{
			return EditorGUI.ColorField(rect, content, value);
		}
	}
}