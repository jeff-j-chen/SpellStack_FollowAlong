using UnityEngine;
using System;
using Object = UnityEngine.Object;
using UnityEditor;
using Vexe.Editor.Helpers;
using Vexe.Editor.Extensions;
using Vexe.Runtime.Extensions;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void DraggableObjectField<T>(T value, Action<T> setValue) where T : Object
		{
			DraggableObjectField("", value, setValue);
		}
		public void DraggableObjectField<T>(string label, T value, Action<T> setValue) where T : Object
		{
			DraggableObjectField(label, "", value, setValue);
		}
		public void DraggableObjectField<T>(string label, string tooltip, T value, Action<T> setValue) where T : UnityEngine.Object
		{
			ObjectField(label, tooltip, value, true, setValue);
			GetLastRect(lastRect => GuiHelper.RegisterFieldForDrag(lastRect, value));
		}

		public void DraggablePropertyField(SerializedProperty property)
		{
			DraggablePropertyField(property.name.SplitPascalCase(), property);
		}

		public void DraggablePropertyField(string label, SerializedProperty property)
		{
			DraggablePropertyField(label, "", property);
		}

		public void DraggablePropertyField(string label, string tooltip, SerializedProperty property)
		{
			PropertyField(property, label);
			GetLastRect(lastRect => GuiHelper.RegisterFieldForDrag(lastRect, property.gameObject()));
		}

		public void ObjectField<T>(T value, Action<T> setValue) where T : Object
		{
			ObjectField("", value, setValue);
		}
		public void ObjectField<T>(string label, T value, Action<T> setValue) where T : Object
		{
			ObjectField(label, "", value, setValue);
		}
		public void ObjectField<T>(string label, string tooltip, T value, Action<T> setValue) where T : Object
		{
			ObjectField(label, tooltip, value, true, setValue);
		}
		public void ObjectField<T>(string label, string tooltip, T value, bool allowSceneObjects, Action<T> setValue) where T : UnityEngine.Object // for some alien reason, writing only "Object" yielded an error that Object is not found even though it's defined and used everywhere in this file
		{
			ObjectField(new GUIContent(label, tooltip), value, typeof(T), allowSceneObjects, null, newValue => setValue(newValue as T));
		}

		public void ObjectField(Object value, Action<Object> setValue)
		{
			ObjectField("", value, setValue);
		}
		public void ObjectField(Object value, Type type, Action<Object> setValue)
		{
			ObjectField("", value, type, setValue);
		}
		public void ObjectField(string label, Object value, Action<Object> setValue)
		{
			ObjectField(label, value, typeof(Object), setValue);
		}
		public void ObjectField(string label, Object value, Type type, Action<Object> setValue)
		{
			ObjectField(label, value, type, null, setValue);
		}
		public void ObjectField(string label, Object value, Type type, TOption option, Action<Object> setValue)
		{
			ObjectField(label, value, type, true, option, setValue);
		}
		public void ObjectField(string label, Object value, Type type, bool allowSceneObjects, TOption option, Action<Object> setValue)
		{
			ObjectField(label, "", value, type, allowSceneObjects, option, setValue);
		}
		public void ObjectField(string label, string tooltip, Object value, Type type, TOption option, Action<Object> setValue)
		{
			ObjectField(new GUIContent(label, tooltip), value, type, true, option, setValue);
		}
		public void ObjectField(string label, string tooltip, Object value, Type type, bool allowSceneObjects, TOption option, Action<Object> setValue)
		{
			ObjectField(new GUIContent(label, tooltip), value, type, allowSceneObjects, option, setValue);
		}
		public abstract void ObjectField(GUIContent content, Object value, Type type, bool allowSceneObjects, TOption option, Action<Object> setValue);
	}
}