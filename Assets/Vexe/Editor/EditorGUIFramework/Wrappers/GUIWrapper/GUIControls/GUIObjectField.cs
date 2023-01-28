using UnityEngine;
using EditorGUIFramework.Helpers;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

namespace EditorGUIFramework
{
	public class GUIObjectField : GUIControl, IChangableGUIControl
	{
		public Action OnChange { get; set; }
		public Action<Object> SetValue { get; set; }
		public Object Value { get; set; }
		public Type Type { get; set; }
		public bool allowSceneObjects { get; set; }

		public override void Draw()
		{
			Blocks.ChangeBlock(() =>
				SetValue(EditorGUI.ObjectField(Rect, Content, Value, Type, allowSceneObjects)),
				OnChange);
			base.Draw();
		}
	}
}