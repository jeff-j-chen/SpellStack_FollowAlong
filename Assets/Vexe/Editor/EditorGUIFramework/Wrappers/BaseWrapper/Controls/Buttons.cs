using System;
using System.Collections.Generic;
using UnityEngine;
using sp = UnityEditor.SerializedProperty;
using Vexe.Editor.Extensions;
using Vexe.Runtime.Extensions;
using Vexe.Editor.Helpers;

namespace EditorGUIFramework
{
	public enum MiniButtonStyle { Left, Mid, Right, ModLeft, ModMid, ModRight }

	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public const MiniButtonStyle DefaultMiniStyle = MiniButtonStyle.Mid;
		public const MiniButtonStyle DefaultModStyle = MiniButtonStyle.ModMid;
		public static TOption DefaultMiniOption = new TOption { Width = DEFAULT_MINI_WIDTH, Height = DEFAULT_MINI_HEIGHT };
		private static GUIStyle DefaultButtonStyle { get { return GUI.skin.button; } }

		public void Button(string text)
		{
			Button(text, delegate { });
		}
		public void Button(string text, Action code)
		{
			Button(text, "", code);
		}
		public void Button(string text, TOption option, Action code)
		{
			Button(text, "", option, code);
		}
		public void Button(string text, string tooltip, Action code)
		{
			Button(text, tooltip, (TOption)null, code);
		}
		public void Button(string text, string tooltip, TOption option, Action code)
		{
			Button(text, tooltip, DefaultButtonStyle, option, code);
		}
		public void Button(string text, string tooltip, GUIStyle style, Action code)
		{
			Button(text, tooltip, style, null, code);
		}
		public void Button(string text, string tooltip, GUIStyle style, TOption option, Action code)
		{
			Button(new GUIContent(text, tooltip), style, option, code);
		}
		public void Button(GUIContent content, Action code)
		{
			Button(content, DefaultButtonStyle, null, code);
		}
		public abstract void Button(GUIContent content, GUIStyle style, TOption option, Action code);

		public void InvisibleButton(TOption option, Action code)
		{
			Button(GUIContent.none, GUIStyle.none, option, code);
		}
		public void InvisibleButton()
		{
			InvisibleButton(null, delegate { });
		}

		public void MiniButton(string text)
		{
			MiniButton(text, delegate { });
		}
		public void MiniButton(string text, Action code)
		{
			MiniButton(text, "", code);
		}
		public void MiniButton(string text, string tooltip, Action code)
		{
			MiniButton(text, tooltip, DefaultMiniStyle, code);
		}
		public void MiniButton(string text, MiniButtonStyle style, Action code)
		{
			MiniButton(text, style, DefaultMiniOption, code);
		}
		public void MiniButton(string text, TOption option, Action code)
		{
			MiniButton(text, DefaultMiniStyle, option, code);
		}

		public void MiniButton(string text, MiniButtonStyle style, TOption option, Action code)
		{
			MiniButton(text, "", style, option, code);
		}
		public void MiniButton(string text, string tooltip, MiniButtonStyle style, Action code)
		{
			MiniButton(text, tooltip, style, DefaultMiniOption, code);
		}
		public void MiniButton(string text, string tooltip, MiniButtonStyle style, TOption option, Action code)
		{
			MiniButton(new GUIContent(text, tooltip), style, option, code);
		}
		public void MiniButton(GUIContent content, MiniButtonStyle style, Action code)
		{
			MiniButton(content, style, DefaultMiniOption, code);
		}
		public void MiniButton(GUIContent content, MiniButtonStyle style, TOption option, Action code)
		{
			MiniButton(content, GetStyle(style), option, code);
		}
		public void MiniButton(string text, string tooltip, GUIStyle style, TOption option, Action code)
		{
			MiniButton(new GUIContent(text, tooltip), style, option, code);
		}
		public void MiniButton(string text, string tooltip, GUIStyle style, Action code)
		{
			MiniButton(new GUIContent(text, tooltip), style, DefaultMiniOption, code);
		}
		public void MiniButton(string text, string tooltip, TOption option, Action code)
		{
			MiniButton(new GUIContent(text, tooltip), GetStyle(DefaultMiniStyle), option ?? new TOption { Height = DEFAULT_MINI_HEIGHT }, code);
		}
		public abstract void MiniButton(GUIContent content, GUIStyle style, TOption option, Action code);

		public void AddButton(string target, Action code)
		{
			AddButton(target, DefaultModStyle, code);
		}
		public void AddButton(string target, MiniButtonStyle style, Action code)
		{
			MiniButton("+", "Add a new " + target, style, code);
		}

		public void ClearButton(sp spArray, string target)
		{
			ClearButton(spArray, target, DefaultMiniStyle);
		}
		public void ClearButton(sp spArray, string target, MiniButtonStyle style)
		{
			ClearButton(target, style, spArray.ClearArray);
		}
		public void ClearButton(string target, Action code)
		{
			ClearButton(target, DefaultModStyle, code);
		}
		public void ClearButton(string target, MiniButtonStyle style, Action code)
		{
			MiniButton("x", "Clear " + target, style, code);
		}

		public void RemoveButton(sp spArray, int atIndex, string target)
		{
			RemoveButton(spArray, atIndex, target, DefaultModStyle);
		}
		public void RemoveButton(sp spArray, int atIndex, string target, MiniButtonStyle style)
		{
			RemoveButton(spArray, atIndex, target, style, delegate { });
		}
		public void RemoveButton(sp spArray, int atIndex, string target, MiniButtonStyle style, Action code)
		{
			RemoveButton(target, style, (() => spArray.RemoveAt(atIndex)) + code);
		}
		public void RemoveButton(string target, Action code)
		{
			RemoveButton(target, DefaultModStyle, code);
		}
		public void RemoveButton(string target, MiniButtonStyle style, Action code)
		{
			MiniButton("-", "Remove " + target, style, code);
		}

		public void MoveUpButton(MiniButtonStyle style, Action moveUp)
		{
			MiniButton("▲", "Move element up", style, moveUp);
		}
		public void MoveUpButton(Action moveUp)
		{
			MoveUpButton(DefaultMiniStyle, moveUp);
		}
		public void MoveUpButton<T>(List<T> list, int atIndex, MiniButtonStyle style)
		{
			MoveUpButton(style, () => list.MoveElementUp(atIndex));
		}
		public void MoveUpButton<T>(List<T> list, int atIndex)
		{
			MoveUpButton(list, atIndex, DefaultMiniStyle);
		}

		public void MoveDownButton(MiniButtonStyle style, Action moveDown)
		{
			MiniButton("▼", "Move element down", style, moveDown);
		}
		public void MoveDownButton(Action moveDown)
		{
			MoveDownButton(DefaultMiniStyle, moveDown);
		}
		public void MoveDownButton<T>(List<T> list, int atIndex, MiniButtonStyle style)
		{
			MoveDownButton(style, () => list.MoveElementDown(atIndex));
		}
		public void MoveDownButton<T>(List<T> list, int atIndex)
		{
			MoveDownButton(list, atIndex, DefaultMiniStyle);
		}

		public void ToggleButton(sp spToggle, string on, string off, string whatToToggle, MiniButtonStyle style = MiniButtonStyle.Mid)
		{
			ToggleButton(spToggle.boolValue, value => spToggle.boolValue = value, on, off, whatToToggle, style);
		}
		public void ToggleButton(bool value, Action<bool> setToggle, string on, string off, string whatToToggle, MiniButtonStyle style = MiniButtonStyle.Mid)
		{
			MiniButton(value ? on : off, "Toggle " + whatToToggle, style, () => setToggle(!value));
		}

		public void FoldoutButton(bool value, Action<bool> setValue, string whatToFoldout, MiniButtonStyle style = MiniButtonStyle.Mid)
		{
			ToggleButton(value, setValue, GuiHelper.Folds.DefaultFoldSymbol, GuiHelper.Folds.DefaultExpandSymbol, whatToFoldout, style);
		}

		public void CheckButton(bool value, Action<bool> setToggle, string whatToToggle)
		{
			CheckButton(value, setToggle, whatToToggle, DefaultMiniStyle);
		}
		public void CheckButton(sp foldout, string whatToToggle)
		{
			CheckButton(foldout.boolValue, b => foldout.boolValue = b, whatToToggle);
		}
		public void CheckButton(bool value, Action<bool> setToggle, string whatToToggle, MiniButtonStyle style)
		{
			ToggleButton(value, setToggle, "✔", "", whatToToggle, style);
		}
		public void CheckButton(sp foldout, string whatToToggle, MiniButtonStyle style)
		{
			CheckButton(foldout.boolValue, b => foldout.boolValue = b, whatToToggle, style);
		}

		public void RefreshButton(TOption option, Action code)
		{
			Button("↶", "Refresh", GuiHelper.RefreshButtonStyle, option, code);
		}
		public void RefreshButton(Action code)
		{
			RefreshButton(DefaultMiniOption, code);
		}
		public void SelectionButton(string whatToSelect, TOption option, Action code)
		{
			Button("◉", "Select " + whatToSelect, GuiHelper.SelectionButtonStyle, option, code);
		}
		public void SelectionButton(string whatToSelect, Action code)
		{
			SelectionButton(whatToSelect, DefaultMiniOption, code);
		}
		public void SelectionButton(Action code)
		{
			SelectionButton("", code);
		}

		public void InspectButton(GameObject go, MiniButtonStyle style, TOption option)
		{
			MiniButton("⏎", "Inspect", style, option, () => EditorHelper.InspectTarget(go));
		}
		public void InspectButton(GameObject go, MiniButtonStyle style)
		{
			InspectButton(go, style, DefaultMiniOption);
		}
		public void InspectButton(GameObject go, TOption option)
		{
			InspectButton(go, DefaultMiniStyle, option);
		}
		public void InspectButton(GameObject go)
		{
			InspectButton(go, DefaultMiniStyle, DefaultMiniOption);
		}
	}
}