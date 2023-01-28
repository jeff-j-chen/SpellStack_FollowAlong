using System;
using UnityEditor;
using EditorGUIFramework.Helpers;

namespace EditorGUIFramework
{
	public class GUIPropertyField : GUIControl, IChangableGUIControl
	{
		public SerializedProperty Property;
		public Action OnChange { get; set; }
		public bool IncludeChildren { get; set; }

		public override void Draw()
		{
			Blocks.ChangeBlock(() =>
				EditorGUI.PropertyField(Rect, Property, Content, IncludeChildren),
				OnChange);
			base.Draw();
		}
	}
}