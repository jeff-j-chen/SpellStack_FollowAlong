using System;
using UnityEditor;
using UnityEngine;
using TOption = EditorGUIFramework.GUIOption;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void TextField(string value, Action<string> setValue)
		{
			TextField("", value, setValue);
		}
		public void TextField(string label, string value, Action<string> setValue)
		{
			TextField(label, value, (TOption)null, setValue);
		}
		public void TextField(string label, string value, TOption option, Action<string> setValue)
		{
			TextField(label, value, "", option, setValue);
		}
		public void TextField(string label, string value, string tooltip, Action<string> setValue)
		{
			TextField(label, value, tooltip, null, setValue);
		}
		public void TextField(string label, string value, string tooltip, TOption option, Action<string> setValue)
		{
			TextField(new GUIContent(label, tooltip), value, option, setValue);
		}
		public void TextField(GUIContent content, string value, TOption option, Action<string> setValue)
		{
			TextField(content, value, EditorStyles.numberField, option, setValue);
		}
		public abstract void TextField(GUIContent content, string value, GUIStyle style, TOption option, Action<string> setValue);
	}
}