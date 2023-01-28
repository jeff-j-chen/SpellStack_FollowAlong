using UnityEngine;
using System;
using TOption = EditorGUIFramework.GUIOption;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void ColorField(Color value, Action<Color> setValue)
		{
			ColorField("", value, setValue);
		}
		public void ColorField(string label, Color value, Action<Color> setValue)
		{
			ColorField(label, value, null, setValue);
		}
		public void ColorField(string label, string tooltip, Color value, Action<Color> setValue)
		{
			ColorField(label, tooltip, value, null, setValue);
		}
		public void ColorField(string label, Color value, TOption option, Action<Color> setValue)
		{
			ColorField(label, "", value, option, setValue);
		}
		public void ColorField(string label, string tooltip, Color value, TOption option, Action<Color> setValue)
		{
			ColorField(new GUIContent(label, tooltip), value, option, setValue);
		}
		public abstract void ColorField(GUIContent content, Color value, TOption option, Action<Color> setValue);
	}
}