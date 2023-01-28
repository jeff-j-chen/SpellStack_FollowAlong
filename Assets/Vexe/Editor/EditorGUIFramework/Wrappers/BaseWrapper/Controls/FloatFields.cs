using System;
using UnityEditor;
using UnityEngine;
using TOption = EditorGUIFramework.GUIOption;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void FloatField(float value, Action<float> setValue)
		{
			FloatField("", value, setValue);
		}
		public void FloatField(string label, float value, Action<float> setValue)
		{
			FloatField(label, value, (TOption)null, setValue);
		}
		public void FloatField(string label, float value, TOption option, Action<float> setValue)
		{
			FloatField(label, value, "", option, setValue);
		}
		public void FloatField(string label, float value, string tooltip, Action<float> setValue)
		{
			FloatField(label, value, tooltip, null, setValue);
		}
		public void FloatField(string label, float value, string tooltip, TOption option, Action<float> setValue)
		{
			FloatField(new GUIContent(label, tooltip), value, option, setValue);
		}
		public void FloatField(GUIContent content, float value, Action<float> setValue)
		{
			FloatField(content, value, null, setValue);
		}
		public void FloatField(GUIContent content, float value, TOption option, Action<float> setValue)
		{
			FloatField(content, value, EditorStyles.numberField, option, setValue);
		}
		public abstract void FloatField(GUIContent content, float value, GUIStyle style, TOption option, Action<float> setValue);
	}
}