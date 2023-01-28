using UnityEngine;
using TOption = EditorGUIFramework.GUIOption;
using UnityEditor;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		/// <summary>
		/// Creates a bold numeric label with a width of NUMERIC_LABEL_WIDTH
		/// </summary>
		public void NumericLabel(int n, GUIStyle style)
		{
			Label(n + "- ", style, new TOption { Width = NUMERIC_LABEL_WIDTH });
		}

		public void NumericLabel(int n)
		{
			NumericLabel(n, EditorStyles.miniLabel);
		}

		/// <summary>
		/// Creates a NumericLabel, with a TextFieldLabel beside it
		/// </summary>
		public void NumericTextFieldLabel(int n, string label)
		{
			HorizontalBlock(() =>
			{
				NumericLabel(n);
				TextFieldLabel(label);
			});
		}

		/// <summary>
		/// Creates a Label with a EditorStyles.textField style
		/// </summary>
		public void TextFieldLabel(string label)
		{
			Label(label, GUI.skin.textField);
		}

		public void BoldLabel(string text)
		{
			BoldLabel(text, null);
		}
		public void BoldLabel(string text, TOption option)
		{
			Label(text, EditorStyles.boldLabel, option);
		}

		public void Label(string text)
		{
			Label(text, (TOption)null);
		}
		public void Label(string text, TOption option)
		{
			Label(text, EditorStyles.label, option);
		}
		public void Label(string text, GUIStyle style)
		{
			Label(text, style, null);
		}
		public void Label(string text, GUIStyle style, TOption option)
		{
			Label(new GUIContent(text), style, option);
		}
		public abstract void Label(GUIContent content, GUIStyle style, TOption option);
	}
}