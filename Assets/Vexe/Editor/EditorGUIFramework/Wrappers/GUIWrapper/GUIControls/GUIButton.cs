using System;
using EditorGUIFramework.Helpers;
using UnityEngine;

namespace EditorGUIFramework
{
	public class GUIButton : GUIControl, IChangableGUIControl
	{
		public Action Code { get; set; }
		public Action OnChange { get; set; }

		public override void Draw()
		{
			Blocks.ChangeBlock(() =>
			{
				if (GUI.Button(Rect, Content, Style))
					Code();
			}, OnChange);
			base.Draw();
		}
	}
}