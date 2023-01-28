using UnityEngine;
using System;
using System.Linq;
using UnityEditor;
using Option = EditorGUIFramework.GUIOption;
using Vexe.Editor.Helpers;

namespace EditorGUIFramework
{
	/// <summary>
	/// A wrapper that makes it easy to use GUI methods in a GUILayout fashion in GUILayout-resitricted areas (such as in PropertyDrawer.OnGUI)
	/// Author @vexe
	/// </summary>
	public partial class GUIWrapper : BaseWrapper<Option>, IMustBeNotifiedOnHeightChange
	{
		/* <<< Member fields >>> */
		#region
		public const float DefaultVerticalOffset = 2f;
		public const float DefaultHorizontalOffset = 3.5f;

		private Rectangle currentStart;
		private VerticalGUIControlBlock mainBlock;
		private BlockStack stack;
		private float height;

		private bool hasChanged = true;
		#endregion

		/* <<< Properties >>> */
		#region
		public float Height { get { return height; } }
		private IBlock LastBlock { get { return stack.LastBlock; } }
		private GUIControlBlock LastGUIBlock { get { return stack.LastGUIBlock; } }
		#endregion

		public GUIWrapper()
		{
			stack = new BlockStack();
		}

		public void HeightHasChanged()
		{
			hasChanged = true;
		}

		private void AddControl(GUIControl control, Option option)
		{
			LastBlock.AddControl(control, option);
		}

		/* <<< Blocks >>> */
		#region

		/* <<< Main, Vertical & Horizontal >>> */
		#region
		public float Layout(Action block)
		{
			return Layout(GUIStyle.none, block);
		}
		public float Layout(GUIStyle style, Action block)
		{
			if (hasChanged)
			{
				//Debug.Log("GUIWrapper new Layout");
				height = InternalLayout(new Rect(), style, block);
				hasChanged = false;
			}
			return height;
		}

		private float InternalLayout(Rect start, GUIStyle style, Action block)
		{
			currentStart = new Rectangle(start);
			MainBlockInternal(style, block);
			return mainBlock.Height.Value + DefaultVerticalOffset;
		}
		public void Draw(Rect start, Action block)
		{
			Draw(start, GUIStyle.none, block);
		}
		public void Draw(Rect start, GUIStyle style, Action block)
		{
			height = InternalLayout(start, style, block);
			MainStackBlock(mainBlock.Draw);
		}

		public override void HorizontalBlock(GUIStyle style, Action block)
		{
			Begin<HorizontalGUIControlBlock>(style);
			block();
			End();
		}
		public override void VerticalBlock(GUIStyle style, Action block)
		{
			Begin<VerticalGUIControlBlock>(style);
			block();
			End();
		}
		private void Begin<T>(GUIStyle style) where T : GUIControlBlock, new()
		{
			var block = new T { Start = currentStart, Style = style };
			AddControl(block, null);
			stack.Push(block);
		}
		private void End()
		{
			stack.Pop();
		}
		#endregion

		/* <<< Command blocks >>> */
		#region
		public override void GetLastRect(Action<Rect> code)
		{
			LastBlock.Entries.Last().control.OnDrawn += code;
		}
		public override void EnabledBlock(bool predicate, Action code)
		{
			CommandBlockInternal(new GUIEnabledBlock { State = predicate, LastBlock = LastBlock }, code);
		}
		public override void ColorBlock(Color color, Action code)
		{
			CommandBlockInternal(new GUIColorBlock { Color = color, LastBlock = LastBlock }, code);
		}
		public override void ChangeBlock(Action check, Action onChange)
		{
			CommandBlockInternal(new GUIChangedBlock { OnChangeHandler = onChange, LastBlock = LastBlock }, check);
		}
		public override void LabelWidthBlock(float width, Action code)
		{
			CommandBlockInternal(new GUILabelWidthBlock { LabelWidth = width, LastBlock = LastBlock }, code);
		}
		#endregion

		/* <<< Internals >>> */
		#region
		private void MainBlockInternal(GUIStyle style, Action block)
		{
			mainBlock = new VerticalGUIControlBlock { Start = currentStart, Style = style };
			MainStackBlock(block);
		}
		private void MainStackBlock(Action block)
		{
			stack.Push(mainBlock);
			block();
			End();
		}
		private void CommandBlockInternal(CommandBlock b, Action code)
		{
			stack.Push(b);
			code();
			End();
			b.Execute();
		}
		#endregion
		#endregion

		/* <<< Controls overrides >>> */
		#region
		public override void BoundsField(GUIContent content, Bounds value, Option option, Action<Bounds> setValue)
		{
			AddControl(new GUIBoundsField
			{
				Content = content,
				Value = value,
				SetValue = setValue
			}, option);
		}

		public override void Box(GUIContent content, GUIStyle style, Option option)
		{
			AddControl(new GUIBox { Content = content, Style = style }, option);
		}

		public override void Button(GUIContent content, GUIStyle style, Option option, Action code)
		{
			AddControl(new GUIButton { Content = content, Style = style, Code = code }, option);
		}

		public override void MiniButton(GUIContent content, GUIStyle style, Option option, Action code)
		{
			AddControl(new GUIMiniButton
			{
				Content = content,
				Style = style,
				Code = code,
			}, option);
		}

		public override void ColorField(GUIContent content, Color value, Option option, Action<Color> setValue)
		{
			AddControl(new GUIColorField
			{
				Content = content,
				Value = value,
				SetValue = setValue
			}, option);
		}

		public override void FloatField(GUIContent content, float value, GUIStyle style, Option option, Action<float> setValue)
		{
			AddControl(new GUIFloatField
			{
				Value = value,
				Content = content,
				Style = style,
				SetValue = setValue
			}, option);
		}

		public override void Foldout(GUIContent content, bool foldout, GUIStyle style, Option option, Action<bool> setValue)
		{
			AddControl(new GUIFoldout
			{
				Value = foldout,
				Content = content,
				ToggleOnLabelClick = true,
				Style = style,
				SetValue = setValue
			}, option);
		}

		public override void IntField(GUIContent content, int value, GUIStyle style, Option option, Action<int> setValue)
		{
			AddControl(new GUIIntField
			{
				Value = value,
				Content = content,
				Style = style,
				SetValue = setValue
			}, option);
		}

		public override void Label(GUIContent content, GUIStyle style, Option option)
		{
			AddControl(new GUILabel { Content = content, Style = style }, option);
		}

		public override void ObjectField(GUIContent content, UnityEngine.Object value, Type type, bool allowSceneObjects, Option option, Action<UnityEngine.Object> setValue)
		{
			AddControl(new GUIObjectField
			{
				Content = content,
				Value = value,
				Type = type,
				allowSceneObjects = allowSceneObjects,
				SetValue = setValue,
				Style = EditorStyles.objectField,
			}, option);
		}

		public override void Splitter(float thickness)
		{
			AddControl(new GUISplitter(), new Option { Height = thickness });
		}

		public override void Popup(GUIContent content, int selectedIndex, GUIContent[] displayedOptions, GUIStyle style, Option option, Action<int> setCurrentIndex)
		{
			AddControl(new GUIPopup
			{
				Content = content,
				SelectedIndex = selectedIndex,
				SetCurrentIndex = setCurrentIndex,
				DisplayedOptions = displayedOptions,
				Style = style
			}, option);
		}

		public override void PropertyField(SerializedProperty sp, GUIContent content, bool includeChildren, Option option)
		{
			AddControl(new GUIPropertyField { Property = sp, Content = content, IncludeChildren = includeChildren }, option);
		}

		public override void RectField(GUIContent content, Rect value, Option option, Action<Rect> setValue)
		{
			AddControl(new GUIRectField
			{
				Content = content,
				Value = value,
				SetValue = setValue
			}, option);
		}

		public override void Space(float pixels)
		{
			if (pixels != 0)
			{
				var last = LastGUIBlock;
				if (last != null)
					AddControl(last.CreateSpace(pixels), null);
			}
		}

		public override void FlexibleSpace()
		{
			AddControl(new GUIFlexibleSpace(), null);
		}

		public override void TextField(GUIContent content, string value, GUIStyle style, Option option, Action<string> setValue)
		{
			AddControl(new GUITextField
			{
				Value = value,
				Content = content,
				Style = style,
				SetValue = setValue
			}, option);
		}

		public override void Toggle(GUIContent content, bool value, GUIStyle style, Option option, Action<bool> setValue)
		{
			AddControl(new GUIToggle
			{
				Value = value,
				Content = content,
				Style = style,
				SetValue = setValue
			}, option);
		}

		public override void Vector3Field(GUIContent content, Vector3 value, Option option, Action<Vector3> setValue)
		{
			AddControl(new GUIVector3Field
			{
				Content = content,
				Value = value,
				SetValue = setValue
			}, option);
		}

		public override void Vector2Field(GUIContent content, Vector2 value, Option option, Action<Vector2> setValue)
		{
			AddControl(new GUIVector2Field
			{
				Content = content,
				Value = value,
				SetValue = setValue
			}, option);
		}

		public override void HelpBox(string message, MessageType type)
		{
			var content = new GUIContent(message, GuiHelper.GetHelpIcon(type), "");
			float height = GuiHelper.HelpBox.CalcSize(content).y;

			AddControl(new GUIHelpBox
			{
				Message = message,
				Type = type
			}, new Option { Height = height });
		}

		public override void MaskField(GUIContent label, int mask, string[] displayedOptions, GUIStyle style, Option option, Action<int> setMask)
		{
			AddControl(new GUIMaskField
			{
				Content = label,
				Mask = mask,
				DisplayedOptions = displayedOptions,
				Style = style,
				SetMask = setMask
			}, option);
		}

		public override void Slider(GUIContent label, float value, float leftValue, float rightValue, Option option, Action<float> setValue)
		{
			AddControl(new GUISlider
			{
				Content = label,
				Value = value,
				LeftValue = leftValue,
				RightValue = rightValue,
				SetValue = setValue,
				Style = GUI.skin.horizontalSlider
			}, option);
		}
		#endregion
	}
}