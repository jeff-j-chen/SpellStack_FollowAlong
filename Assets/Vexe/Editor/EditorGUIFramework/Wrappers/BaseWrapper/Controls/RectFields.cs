using UnityEngine;
using System;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void RectField(Rect value, Action<Rect> setValue)
		{
			RectField("", value, setValue);
		}
		public void RectField(string label, Rect value, Action<Rect> setValue)
		{
			RectField(label, value, DefaultMultiFieldOption, setValue);
		}
		public void RectField(string label, string tooltip, Rect value, Action<Rect> setValue)
		{
			RectField(label, tooltip, value, DefaultMultiFieldOption, setValue);
		}
		public void RectField(string label, Rect value, TOption option, Action<Rect> setValue)
		{
			RectField(label, "", value, option, setValue);
		}
		public void RectField(string label, string tooltip, Rect value, TOption option, Action<Rect> setValue)
		{
			RectField(new GUIContent(label, tooltip), value, option, setValue);
		}
		public abstract void RectField(GUIContent content, Rect value, TOption option, Action<Rect> setValue);
	}
}