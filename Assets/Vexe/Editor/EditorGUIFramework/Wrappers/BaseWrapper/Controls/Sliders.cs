using UnityEngine;
using UnityEditor;
using System;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void Slider(SerializedProperty property, float leftValue, float rightValue)
		{
			Slider(property, leftValue, rightValue, null);
		}

		public void Slider(SerializedProperty property, float leftValue, float rightValue, TOption option)
		{
			Slider(property, "", leftValue, rightValue, option);
		}

		public void Slider(SerializedProperty property, string label, float leftValue, float rightValue, TOption option)
		{
			Slider(property, new GUIContent(label), leftValue, rightValue, option);
		}

		public void Slider(SerializedProperty property, GUIContent label, float leftValue, float rightValue, TOption option)
		{
			Slider(label, property.floatValue, leftValue, rightValue, option, newValue =>
			{
				if (!Mathf.Approximately(property.floatValue, newValue))
				{
					property.floatValue = newValue;
					property.serializedObject.ApplyModifiedProperties();
				}
			});
		}


		public void Slider(float value, float leftValue, float rightValue, Action<float> setValue)
		{
			Slider(value, leftValue, rightValue, null, setValue);
		}

		public void Slider(float value, float leftValue, float rightValue, TOption option, Action<float> setValue)
		{
			Slider("", value, leftValue, rightValue, option, setValue);
		}

		public void Slider(string label, float value, float leftValue, float rightValue, Action<float> setValue)
		{
			Slider(label, value, leftValue, rightValue, null, setValue);
		}

		public void Slider(string label, float value, float leftValue, float rightValue, TOption option, Action<float> setValue)
		{
			Slider(new GUIContent(label), value, leftValue, rightValue, option, setValue);
		}

		public abstract void Slider(GUIContent label, float value, float leftValue, float rightValue, TOption option, Action<float> setValue);
	}
}