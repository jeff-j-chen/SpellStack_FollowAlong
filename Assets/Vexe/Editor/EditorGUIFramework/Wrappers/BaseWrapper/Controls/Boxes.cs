using UnityEditor;
using UnityEngine;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void Box(string text, TOption option)
		{
			Box(text, GUI.skin.box, option);
		}
		public void Box(string text, GUIStyle style, TOption option)
		{
			Box(new GUIContent(text), style, option);
		}

		public abstract void Box(GUIContent content, GUIStyle style, TOption option);
		public abstract void HelpBox(string message, MessageType type);
	}
}