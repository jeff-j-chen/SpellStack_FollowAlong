using UnityEngine;
using UnityEditor;
using System;

namespace EditorGUIFramework.Helpers
{
	public static class Blocks
	{
		private static bool GUIState { get { return GUI.enabled; } set { GUI.enabled = value; } }
		private static Color GUIColor { get { return GUI.color; } set { GUI.color = value; } }

		public static void StateBlock(bool newState, Action block)
		{
			var state = GUIState;
			GUIState = newState;
			block();
			GUIState = state;
		}

		public static void ColorBlock(Color newColor, Action block)
		{
			var color = GUIColor;
			GUIColor = newColor;
			block();
			GUIColor = color;
		}

		public static void ChangeBlock(Action check, Action onChange)
		{
			EditorGUI.BeginChangeCheck();
			check();
			if (EditorGUI.EndChangeCheck())
				if (onChange != null)
					onChange();
		}

		public static void LabelWidthBlock(float width, Action block)
		{
			EditorGUIUtility.labelWidth = width;
			block();
			EditorGUIUtility.labelWidth = 0;
		}
	}
}