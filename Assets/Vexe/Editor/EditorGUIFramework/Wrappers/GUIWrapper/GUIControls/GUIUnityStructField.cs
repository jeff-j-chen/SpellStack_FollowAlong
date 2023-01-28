using System;
using EditorGUIFramework.Helpers;
using UnityEngine;

namespace EditorGUIFramework
{
	public abstract class GUIUnityStructField<T> : GUIValueField<T>
	{
		protected abstract T DoField(Rect rect, GUIContent content, T value);

		public override void Draw()
		{
			Blocks.ChangeBlock(() =>
				SetValue(DoField(Rect, Content, Value)),
				OnChange);
			base.Draw();
		}
	}
}