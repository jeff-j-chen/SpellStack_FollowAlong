using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;

namespace EditorGUIFramework
{
	public abstract partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public const float INDENT_AMOUNT = 20f;
		public const float NUMERIC_LABEL_WIDTH = 21f;
		public const float DEFAULT_HEIGHT = 16.0f;
		public const float DEFAULT_MINI_WIDTH = 20f;
		public const float DEFAULT_MINI_HEIGHT = 16f;
		public const float FOLDOUT_WIDTH = 10f;

		private static GUIStyle NoneStyle { get { return GUIStyle.none; } }
		private static readonly TOption DefaultMultiFieldOption = new TOption { Height = DEFAULT_HEIGHT * 2 };
		protected static GUIStyle DefaultFoldoutStyle { get { return EditorStyles.foldout; } }

		public void IndentedBlock(Action code)
		{
			IndentedBlock(GUIStyle.none, 0, code);
		}
		public void IndentedBlock(float indentLevel, Action code)
		{
			IndentedBlock(GUIStyle.none, indentLevel, code);
		}
		public void IndentedBlock(GUIStyle style, Action code)
		{
			IndentedBlock(style, 0f, code);
		}
		public void IndentedBlock(GUIStyle style, float indentLevel, Action code)
		{
			IndentedBlock(style, indentLevel, 0, 0, code);
		}
		public void IndentedBlock(GUIStyle style, float indentLevel, float beginningVerticalSpace, float endingVerticalSpace, Action block)
		{
			HorizontalBlock(() =>
			{
				Indent(indentLevel);
				VerticalBlock(style, () =>
				{
					Space(beginningVerticalSpace);
					block();
					Space(endingVerticalSpace);
				});
			});
		}
		public void HorizontalBlock(Action block)
		{
			HorizontalBlock(GUIStyle.none, block);
		}
		public void VerticalBlock(Action block)
		{
			VerticalBlock(GUIStyle.none, block);
		}
		public void ColorBlock(Color? color, Action code)
		{
			if (color.HasValue)
				ColorBlock(color.Value, code);
			else code();
		}
		public void ApplyAfterChange(SerializedObject obj, Action code)
		{
			ChangeBlock(code, () => obj.ApplyModifiedProperties());
		}
		public abstract void HorizontalBlock(GUIStyle style, Action block);
		public abstract void VerticalBlock(GUIStyle style, Action block);
		public abstract void GetLastRect(Action<Rect> code);
		public abstract void EnabledBlock(bool predicate, Action code);
		public abstract void ColorBlock(Color color, Action code);
		public abstract void ChangeBlock(Action check, Action onChange);
		public abstract void LabelWidthBlock(float width, Action block);

		public void DrawDefaultInspector(Object target)
		{
			DrawDefaultInspector(target, true);
		}

		public void DrawDefaultInspector(Object target, bool excludeScript)
		{
			DrawDefaultInspector(new SerializedObject(target), excludeScript);
		}

		public void DrawDefaultInspector(SerializedObject obj, bool excludeScript)
		{
			obj.Update();
			SerializedProperty iterator = obj.GetIterator();
			bool enterChildren = true;
			while (iterator.NextVisible(enterChildren))
			{
				if (excludeScript && iterator.name == "m_Script")
					continue;
				PropertyField(iterator, true);
				enterChildren = false;
			}
			obj.ApplyModifiedProperties();
		}

		/* <<< Helpers >>> */
		#region
		private static GUIStyle GetStyle(MiniButtonStyle style)
		{
			switch (style)
			{
				case MiniButtonStyle.Left: return EditorStyles.miniButtonLeft;
				case MiniButtonStyle.Right: return EditorStyles.miniButtonRight;
				case MiniButtonStyle.ModLeft: return ModButtonLeft;
				case MiniButtonStyle.ModRight: return ModButtonRight;
				case MiniButtonStyle.ModMid: return ModButtonMid;
				default: return EditorStyles.miniButtonMid;
			}
		}

		private static GUIStyle GetModButtonStyle(string name, ref GUIStyle style)
		{
			if (style == null)
				style = new GUIStyle(name)
				{
					fontSize = 12,
					contentOffset = new Vector2(-1f, -.8f),
					clipping = TextClipping.Overflow
				};
			return style;
		}
		private static GUIStyle modButtonLeft;
		private static GUIStyle modButtonMid;
		private static GUIStyle modButtonRight;
		private static GUIStyle foldoutStyle;
		private static GUIStyle FoldoutStyle
		{
			get
			{
				if (foldoutStyle == null)
				{
					foldoutStyle = new GUIStyle();
					foldoutStyle.normal = new GUIStyleState
					{
						textColor = EditorStyles.foldout.normal.textColor
					};
				}
				return foldoutStyle;
			}
		}
		private static GUIStyle ModButtonLeft { get { return GetModButtonStyle("miniButtonLeft", ref modButtonLeft); } }
		private static GUIStyle ModButtonMid { get { return GetModButtonStyle("miniButtonMid", ref modButtonMid); } }
		private static GUIStyle ModButtonRight { get { return GetModButtonStyle("miniButtonRight", ref modButtonRight); } }

		public static GUIContent[] GetContentFromStringArray(string[] arr)
		{
			int size = arr.Length;
			var contents = new GUIContent[size];
			for (int i = 0; i < size; i++)
			{
				contents[i] = new GUIContent(arr[i]);
			}
			return contents;
		}
		#endregion
	}
}