using UnityEngine;
using UnityEditor;
using System;
using EditorGUIFramework.Helpers;

namespace EditorGUIFramework
{
	public class GUISlider : GUIControl, IChangableGUIControl
	{
		public Action OnChange { get; set; }
		public Action<float> SetValue { get; set; }
		public float Value { get; set; }
		public float LeftValue { get; set; }
		public float RightValue { get; set; }
		public override void Draw()
		{
			Blocks.ChangeBlock(() =>
				SetValue(EditorGUI.Slider(Rect, Content, Value, LeftValue, RightValue)),
				OnChange);
			base.Draw();
		}
	}
}