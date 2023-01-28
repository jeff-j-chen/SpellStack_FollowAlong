using System;
using UnityEditor;
using UnityEngine;
using TOption = EditorGUIFramework.GUIOption;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void IntField(int value, Action<int> setValue)
		{
			IntField("", value, setValue);
		}
		public void IntField(string label, int value, Action<int> setValue)
		{
			IntField(label, value, (TOption)null, setValue);
		}
		public void IntField(string label, int value, TOption option, Action<int> setValue)
		{
			IntField(label, value, "", option, setValue);
		}
		public void IntField(string label, int value, string tooltip, Action<int> setValue)
		{
			IntField(label, value, tooltip, null, setValue);
		}
		public void IntField(string label, int value, string tooltip, TOption option, Action<int> setValue)
		{
			IntField(new GUIContent(label, tooltip), value, option, setValue);
		}
		public void IntField(GUIContent content, int value, Action<int> setValue)
		{
			IntField(content, value, null, setValue);
		}
		public void IntField(GUIContent content, int value, TOption option, Action<int> setValue)
		{
			IntField(content, value, EditorStyles.numberField, option, setValue);
		}
		public abstract void IntField(GUIContent content, int value, GUIStyle style, TOption option, Action<int> setValue);
	}
}