using System;
using UnityEngine;
using UnityEditor;
using EditorGUIFramework.Helpers;
using Vexe.Runtime.Extensions;
using Object = UnityEngine.Object;

namespace EditorGUIFramework
{
	/// <summary>
	/// A GUILayout wrapper that uses the same API that GUIWrapper does
	/// </summary>
	public class GLWrapper : BaseWrapper<GLOption>
	{
		public override void HorizontalBlock(GUIStyle style, Action block)
		{
			GUILayout.BeginHorizontal(style);
			block();
			GUILayout.EndHorizontal();
		}

		public override void VerticalBlock(GUIStyle style, Action block)
		{
			GUILayout.BeginVertical(style);
			block();
			GUILayout.EndVertical();
		}

		public override void GetLastRect(Action<Rect> code)
		{
			code(GUILayoutUtility.GetLastRect());
		}

		public override void EnabledBlock(bool predicate, Action code)
		{
			Blocks.StateBlock(predicate, code);
		}

		public override void ColorBlock(Color color, Action code)
		{
			Blocks.ColorBlock(color, code);
		}

		public override void ChangeBlock(Action check, Action onChange)
		{
			Blocks.ChangeBlock(check, onChange);
		}

		public override void LabelWidthBlock(float width, Action block)
		{
			Blocks.LabelWidthBlock(width, block);
		}

		/* <<< Controls overrides >>> */
		#region
		public override void BoundsField(GUIContent content, Bounds value, GLOption option, Action<Bounds> setValue)
		{
			setValue(EditorGUILayout.BoundsField(content, value, option));
		}

		public override void Box(GUIContent content, GUIStyle style, GLOption option)
		{
			GUILayout.Box(content, style, option);
		}

		public override void Button(GUIContent content, GUIStyle style, GLOption option, Action code)
		{
			if (GUILayout.Button(content, style, option))
				code();
		}

		public override void MiniButton(GUIContent content, GUIStyle style, GLOption option, Action code)
		{
			Button(content, style, option, code);
		}

		public override void ColorField(GUIContent content, Color value, GLOption option, Action<Color> setValue)
		{
			setValue(EditorGUILayout.ColorField(content, value, option));
		}

		public override void FloatField(GUIContent content, float value, GUIStyle style, GLOption option, Action<float> setValue)
		{
			setValue(EditorGUILayout.FloatField(content, value, option));
		}

		public override void Foldout(GUIContent content, bool foldout, GUIStyle style, GLOption option, Action<bool> setValue)
		{
			var rect = GUILayoutUtility.GetRect(content, style, option);
			setValue(EditorGUI.Foldout(rect, foldout, content, true, style));
		}

		public override void IntField(GUIContent content, int value, GUIStyle style, GLOption option, Action<int> setValue)
		{
			setValue(EditorGUILayout.IntField(content, value, style, option));
		}

		public override void Label(GUIContent content, GUIStyle style, GLOption option)
		{
			GUILayout.Label(content, style, option);
		}

		public override void ObjectField(GUIContent content, Object value, Type type, bool allowSceneObjects, GLOption option, Action<Object> setValue)
		{
			// If we pass an empty content, ObjectField will still reserve space for an empty label ~__~
			Func<Object> field = () => content.text.IsNullOrEmpty() ?
				EditorGUILayout.ObjectField(value, type, allowSceneObjects, option) :
				EditorGUILayout.ObjectField(content, value, type, allowSceneObjects, option);
			setValue(field());
		}

		public override void Splitter(float thickness)
		{
			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(thickness));
		}

		public override void Popup(GUIContent content, int selectedIndex, GUIContent[] displayedOptions, GUIStyle style, GLOption option, Action<int> setCurrentIndex)
		{
			setCurrentIndex(EditorGUILayout.Popup(selectedIndex, displayedOptions, style, option));
		}

		public override void PropertyField(SerializedProperty sp, GUIContent content, bool includeChildren, GLOption option)
		{
			EditorGUILayout.PropertyField(sp, content, includeChildren, option);
		}

		public override void RectField(GUIContent content, Rect value, GLOption option, Action<Rect> setValue)
		{
			setValue(EditorGUILayout.RectField(content, value, option));
		}

		public override void Space(float pixels)
		{
			GUILayout.Space(pixels);
		}

		public override void FlexibleSpace()
		{
			GUILayout.FlexibleSpace();
		}

		public override void TextField(GUIContent content, string value, GUIStyle style, GLOption option, Action<string> setValue)
		{
			setValue(EditorGUILayout.TextField(content, value, style, option));
		}

		public override void Toggle(GUIContent content, bool value, GUIStyle style, GLOption option, Action<bool> setValue)
		{
			setValue(EditorGUILayout.Toggle(content, value, style, option));
		}

		public override void Vector3Field(GUIContent content, Vector3 value, GLOption option, Action<Vector3> setValue)
		{
			setValue(EditorGUILayout.Vector3Field(content, value, option));
		}

		public override void Vector2Field(GUIContent content, Vector2 value, GLOption option, Action<Vector2> setValue)
		{
			setValue(EditorGUILayout.Vector2Field(content, value, option));
		}

		public override void HelpBox(string message, MessageType type)
		{
			EditorGUILayout.HelpBox(message, type);
		}

		public override void MaskField(GUIContent label, int mask, string[] displayedOptions, GUIStyle style, GLOption option, Action<int> setMask)
		{
			setMask(EditorGUILayout.MaskField(label, mask, displayedOptions, style, option));
		}

		public override void Slider(GUIContent label, float value, float leftValue, float rightValue, GLOption option, Action<float> setValue)
		{
			setValue(EditorGUILayout.Slider(label, value, leftValue, rightValue, option));
		}
		#endregion
	}
}