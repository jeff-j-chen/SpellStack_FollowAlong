using UnityEngine;
using UnityEditor;

namespace EditorGUIFramework
{
	public class GUIBoundsField : GUIUnityStructField<Bounds>
	{
		protected override Bounds DoField(Rect rect, GUIContent content, Bounds value)
		{
			return EditorGUI.BoundsField(rect, content, value);
		}
	}
}