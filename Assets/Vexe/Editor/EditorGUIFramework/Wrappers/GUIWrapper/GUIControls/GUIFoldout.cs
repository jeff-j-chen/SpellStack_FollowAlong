using UnityEngine;
using EditorGUIFramework.Helpers;
using UnityEditor;
using System;

namespace EditorGUIFramework
{
	public class GUIFoldout : GUIControl, IChangableGUIControl
	{
		public Action OnChange { get; set; }
		public Action<bool> SetValue { get; set; }
		public bool Value { get; set; }
		public bool ToggleOnLabelClick { get; set; }
		public override void Draw()
		{
			Blocks.ChangeBlock(() =>
				SetValue(EditorGUI.Foldout(Rect, Value, Content, ToggleOnLabelClick, Style)),
				OnChange);
			base.Draw();
		}
	}
}