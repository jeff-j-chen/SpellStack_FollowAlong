using UnityEngine;
using sp = UnityEditor.SerializedProperty;
using Vexe.Editor.Extensions;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void PropertyField(sp sp)
		{
			PropertyField(sp, sp.NiceName());
		}

		public void PropertyField(sp sp, TOption option)
		{
			PropertyField(sp, sp.NiceName(), option);
		}

		public void PropertyField(sp sp, string text)
		{
			PropertyField(sp, text, null);
		}

		public void PropertyField(sp sp, string text, TOption option)
		{
			PropertyField(sp, new GUIContent(text), option);
		}

		public void ReadonlyPropertyField(sp sp, GUIContent content)
		{
			ReadonlyPropertyField(sp, content, false);
		}

		public void ReadonlyPropertyField(sp sp, GUIContent content, bool includeChildren)
		{
			var value = sp.GetValue();
			ChangeBlock(
				() => PropertyField(sp, content, includeChildren),
				() => sp.SetValue(value)
			);
		}

		public void PropertyField(sp sp, GUIContent content)
		{
			PropertyField(sp, content, null);
		}

		public void PropertyField(sp sp, GUIContent content, TOption option)
		{
			PropertyField(sp, content, false, option);
		}

		public void PropertyField(sp property, bool includeChildren)
		{
			PropertyField(property, property.NiceName(), includeChildren);
		}

		public void PropertyField(sp property, string text, bool includeChildren)
		{
			PropertyField(property, new GUIContent(text), includeChildren);
		}

		public void PropertyField(sp property, GUIContent content, bool includeChildren)
		{
			PropertyField(property, content, includeChildren, null);
		}

		public abstract void PropertyField(sp property, GUIContent content, bool includeChildren, TOption option);
	}
}