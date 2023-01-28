using UnityEngine;
using UnityEditor;

namespace EditorGUIFramework
{
	public class GUIRectField : GUIUnityStructField<Rect>
	{
		protected override Rect DoField(Rect rect, GUIContent content, Rect value)
		{
			return EditorGUI.RectField(rect, content, value);
		}
	}
}