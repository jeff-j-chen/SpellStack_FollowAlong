using UnityEngine;
using System;
using TOption = EditorGUIFramework.GUIOption;
using UnityEditor;
using sp = UnityEditor.SerializedProperty;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void Popup(sp spIndex, string[] displayedOptions)
		{
			Popup(spIndex, displayedOptions, i => spIndex.intValue = i);
		}
		public void Popup(sp spIndex, string[] displayedOptions, Action<int> setCurrentIndex)
		{
			Popup(spIndex.intValue, displayedOptions, setCurrentIndex);
		}
		public void Popup(int selectedIndex, string[] displayedOptions, Action<int> setCurrentIndex)
		{
			Popup(selectedIndex, displayedOptions, null, setCurrentIndex);
		}
		public void Popup(int selectedIndex, string[] displayedOptions, TOption option, Action<int> setCurrentIndex)
		{
			Popup("", selectedIndex, displayedOptions, option, setCurrentIndex);
		}
		public void Popup(string text, int selectedIndex, string[] displayedOptions, Action<int> setCurrentIndex)
		{
			Popup(text, selectedIndex, displayedOptions, null, setCurrentIndex);
		}
		public void Popup(string text, int selectedIndex, string[] displayedOptions, TOption option, Action<int> setCurrentIndex)
		{
			Popup(new GUIContent(text), selectedIndex, displayedOptions, option, setCurrentIndex);
		}
		public void Popup(GUIContent content, int selectedIndex, string[] displayedOptions, Action<int> setCurrentIndex)
		{
			Popup(content, selectedIndex, displayedOptions, null, setCurrentIndex);
		}
		public void Popup(GUIContent content, int selectedIndex, string[] displayedOptions, TOption option, Action<int> setCurrentIndex)
		{
			Popup(content, selectedIndex, GetContentFromStringArray(displayedOptions), EditorStyles.popup, option, setCurrentIndex);
		}
		public abstract void Popup(GUIContent content, int selectedIndex, GUIContent[] displayedOptions, GUIStyle style, TOption option, Action<int> setCurrentIndex);
	}
}