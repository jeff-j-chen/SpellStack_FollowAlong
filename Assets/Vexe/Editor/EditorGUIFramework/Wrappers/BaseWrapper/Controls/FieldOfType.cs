using System;
using UnityEngine;
using UnityEditor;
using Vexe.Editor.Helpers;
using Object = UnityEngine.Object;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void FieldOfType<T>(T value, Action<T> setValue)
		{
			FieldOfType(value, typeof(T), newValue => setValue((T)newValue));
		}

		public void FieldOfType(object value, Type fieldType, Action<object> setValue)
		{
			FieldOfType(value, "", fieldType, setValue);
		}

		public void FieldOfType<T>(T value, string label, Action<T> setValue)
		{
			FieldOfType(value, label, typeof(T), newValue => setValue((T)newValue));
		}

		public void FieldOfType(object value, string label, Type fieldType, Action<object> setValue)
		{
			if (fieldType == typeof(int))
			{
				IntField(label, (int)value, newValue => setValue(newValue));
			}
			else if (fieldType == typeof(float))
			{
				FloatField(label, (float)value, newValue => setValue(newValue));
			}
			else if (fieldType == typeof(bool))
			{
				Toggle(label, (bool)value, newValue => setValue(newValue));
			}
			else if (fieldType == typeof(string))
			{
				TextField(label, (string)value, newValue => setValue(newValue));
			}
			else if (fieldType == typeof(Vector3))
			{
				Vector3Field(label, (Vector3)value, newValue => setValue(newValue));
			}
			else if (fieldType == typeof(Vector2))
			{
				Vector2Field(label, (Vector2)value, newValue => setValue(newValue));
			}
			else if (fieldType == typeof(Bounds))
			{
				BoundsField(label, (Bounds)value, newValue => setValue(newValue));
			}
			else if (fieldType == typeof(Rect))
			{
				RectField(label, (Rect)value, newValue => setValue(newValue));
			}
			else if (fieldType == typeof(Color))
			{
				ColorField(label, (Color)value, newValue => setValue(newValue));
			}
			else if (typeof(Object).IsAssignableFrom(fieldType))
			{
				ObjectField(label, value as Object, fieldType, newValue => setValue(newValue));
			}
			else if (fieldType == typeof(Quaternion))
			{
				Vector3Field(label, ((Quaternion)value).eulerAngles, angle => setValue(Quaternion.Euler(angle)));
			}
			else
			{
				ColorBlock(GuiHelper.RedColorDuo.FirstColor, () =>
					HelpBox("Type `" + fieldType.FullName + "` is not supported", MessageType.Error));
			}
		}
	}
}