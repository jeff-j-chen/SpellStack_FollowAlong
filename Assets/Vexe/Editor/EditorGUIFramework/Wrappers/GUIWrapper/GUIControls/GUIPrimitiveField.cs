using System;
using EditorGUIFramework.Helpers;
using UnityEngine;

namespace EditorGUIFramework
{
	public abstract class GUIPrimitiveField<T> : GUIValueField<T>
	{
		protected abstract T DoField(Rect rect, GUIContent content, T value, GUIStyle style);

		public override void Draw()
		{
			Blocks.ChangeBlock(() =>
				SetValue(DoField(Rect, Content, Value, Style)),
				OnChange);
			base.Draw();
		}
	}
}