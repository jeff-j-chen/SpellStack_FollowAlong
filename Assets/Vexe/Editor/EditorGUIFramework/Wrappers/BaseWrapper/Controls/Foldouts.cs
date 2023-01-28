using UnityEngine;
using UnityEditor;
using sp = UnityEditor.SerializedProperty;
using Folds = Vexe.Editor.Helpers.GuiHelper.Folds;
using System;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		private const float FOLD_WIDTH = DEFAULT_MINI_WIDTH - 13f;

		public void Foldout(bool foldout, Action<bool> setValue)
		{
			Foldout(foldout, "", setValue);
		}

		public void Foldout(bool foldout, TOption option, Action<bool> setValue)
		{
			Foldout(foldout, "", option, setValue);
		}

		public void Foldout(bool foldout, string label, Action<bool> setValue)
		{
			Foldout(foldout, label, DefaultFoldoutStyle, setValue);
		}

		public void Foldout(bool foldout, string label, GUIStyle style, Action<bool> setValue)
		{
			Foldout(foldout, label, "", style, new TOption { Width = FOLD_WIDTH }, setValue);
		}

		public void Foldout(bool foldout, string label, TOption option, Action<bool> setValue)
		{
			Foldout(foldout, label, "", DefaultFoldoutStyle, option, setValue);
		}

		public void Foldout(bool foldout, string label, string tooltip, GUIStyle style, TOption option, Action<bool> setValue)
		{
			Foldout(new GUIContent(label, tooltip), foldout, style, option, setValue);
		}

		public abstract void Foldout(GUIContent content, bool foldout, GUIStyle style, TOption option, Action<bool> setValue);

		public void Foldout(sp spFold, string label, GUIStyle style)
		{
			Foldout(spFold.boolValue, label, style, newValue => spFold.boolValue = newValue);
		}

		public void Foldout(sp spFold, string label)
		{
			Foldout(spFold, label, DefaultFoldoutStyle);
		}

		public void Foldout(sp spFold)
		{
			Foldout(spFold, "");
		}

		public void CustomFoldout(bool value, string label, string expandSymbol, string foldSymbol, GUIStyle style, TOption option, Action<bool> setValue)
		{
			Label((value ? foldSymbol : expandSymbol) + label, FoldoutStyle, option);
			GetLastRect(lastRect =>
			{
				if (GUI.Button(lastRect, GUIContent.none, NoneStyle))
					setValue(!value);
			});
		}

		public void CustomFoldout(sp spFoldout, string expandSymbol, string foldSymbol)
		{
			CustomFoldout(spFoldout.boolValue, expandSymbol, foldSymbol, f => spFoldout.boolValue = f);
		}

		public void CustomFoldout(bool value, string expandSymbol, string foldSymbol, Action<bool> setValue)
		{
			CustomFoldout(value, "", expandSymbol, foldSymbol, null, new TOption { Width = FOLDOUT_WIDTH }, setValue);
		}

		public void CustomFoldout(bool value, string label, GUIStyle style, TOption option, Action<bool> setValue)
		{
			CustomFoldout(value, label, Folds.DefaultExpandSymbol, Folds.DefaultFoldSymbol, style, option, setValue);
		}

		public void CustomFoldout(bool value, string label, TOption option, Action<bool> setValue)
		{
			CustomFoldout(value, label, NoneStyle, option, setValue);
		}

		public void CustomFoldout(bool value, TOption option, Action<bool> setValue)
		{
			CustomFoldout(value, "", option, setValue);
		}

		public void CustomFoldout(bool value, Action<bool> setValue)
		{
			CustomFoldout(value, "", new TOption { Width = FOLD_WIDTH }, setValue);
		}
		public void CustomFoldout(sp spFold, string label, GUIStyle style, TOption option)
		{
			CustomFoldout(spFold.boolValue, label, style, option, newValue => spFold.boolValue = newValue);
		}
		public void CustomFoldout(sp spFold, string label, TOption option)
		{
			CustomFoldout(spFold, label, NoneStyle, option);
		}
		public void CustomFoldout(sp spFold, string label)
		{
			CustomFoldout(spFold, label, new TOption { Width = FOLD_WIDTH });
		}
		public void CustomFoldout(sp spFold)
		{
			CustomFoldout(spFold, "");
		}

		public void FoldoutBold(sp spFold, string label, TOption option)
		{
			CustomFoldout(spFold, label, EditorStyles.boldLabel, option);
		}
		public void FoldoutBold(sp spFold, string label)
		{
			FoldoutBold(spFold, label, null);
		}
		public void FoldoutBold(sp spFold)
		{
			FoldoutBold(spFold, "", new TOption { Width = FOLD_WIDTH });
		}
	}
}