using UnityEngine;
using UnityEditor;
using System;
using EditorGUIFramework.Helpers;

namespace EditorGUIFramework
{
	public class GUIMaskField : GUIControl, IChangableGUIControl
	{
		public Action OnChange { get; set; }
		public Action<int> SetMask { get; set; }
		public int Mask { get; set; }
		public string[] DisplayedOptions { get; set; }

		public override void Draw()
		{
			Blocks.ChangeBlock(() =>
				SetMask(EditorGUI.MaskField(Rect, Content, Mask, DisplayedOptions, Style)),
			OnChange);
			base.Draw();
		}
	}
}