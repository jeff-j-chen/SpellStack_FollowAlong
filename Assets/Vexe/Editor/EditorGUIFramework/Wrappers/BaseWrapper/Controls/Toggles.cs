using System;
using UnityEditor;
using UnityEngine;
using TOption = EditorGUIFramework.GUIOption;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void Toggle(bool value, Action<bool> setValue)
		{
			Toggle("", value, setValue);
		}
		public void Toggle(string label, bool value, Action<bool> setValue)
		{
			Toggle(label, value, (TOption)null, setValue);
		}
		public void Toggle(string label, bool value, TOption option, Action<bool> setValue)
		{
			Toggle(label, value, "", option, setValue);
		}
		public void Toggle(string label, bool value, string tooltip, Action<bool> setValue)
		{
			Toggle(label, value, tooltip, null, setValue);
		}
		public void Toggle(string label, bool value, string tooltip, TOption option, Action<bool> setValue)
		{
			Toggle(new GUIContent(label, tooltip), value, option, setValue);
		}
		public void Toggle(GUIContent content, bool value, TOption option, Action<bool> setValue)
		{
			Toggle(content, value, EditorStyles.toggle, option, setValue);
		}
		public abstract void Toggle(GUIContent content, bool value, GUIStyle style, TOption option, Action<bool> setValue);
	}
}