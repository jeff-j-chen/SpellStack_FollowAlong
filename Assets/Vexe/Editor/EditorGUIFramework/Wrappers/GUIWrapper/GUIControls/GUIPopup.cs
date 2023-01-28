using UnityEngine;
using UnityEditor;
using System;
using EditorGUIFramework.Helpers;

namespace EditorGUIFramework
{
	public class GUIPopup : GUIControl, IChangableGUIControl
	{
		public Action OnChange { get; set; }
		public GUIContent[] DisplayedOptions { get; set; }
		public int SelectedIndex { get; set; }
		public Action<int> SetCurrentIndex { get; set; }
		public override void Draw()
		{
			Blocks.ChangeBlock(() =>
				SetCurrentIndex(EditorGUI.Popup(Rect, Content, SelectedIndex, DisplayedOptions, Style)),
				OnChange);
			base.Draw();
		}
	}
}