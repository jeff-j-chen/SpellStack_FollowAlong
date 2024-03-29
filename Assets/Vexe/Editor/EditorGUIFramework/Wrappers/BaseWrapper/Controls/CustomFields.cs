﻿using UnityEngine;
using Object = UnityEngine.Object;
using Option = EditorGUIFramework.GUIOption;
using UnityEditor;
using System;
using Vexe.Editor.Helpers;
using Vexe.Editor.Extensions;
using Vexe.Runtime.Helpers;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		/// <summary>
		/// Creates a draggable label field (a label using a custom style making it resemble an object field)
		/// What if you wanted drop support? then you wouldn't be looking at this method, but one of the others above,
		/// like DraggablePropertyObjectField, and DragObjectField (they support dropping by default)
		/// or manually register your field for drop support via RegisterFieldForDrop, or even use DragAndDropObjectField.
		/// </summary>
		public void DraggableLabelField(string label, string field, Object value, float labelWidth, GUIStyle style, MouseCursor cursor)
		{
			Label(label, labelWidth == 0 ? null : new TOption { Width = labelWidth - 3.5f });
			Label(field, style ?? GUI.skin.textField);
			GetLastRect(lastRect => GuiHelper.RegisterFieldForDrag(lastRect, value));
		}
		public void DraggableLabelField(string label, string field, Object value, float labelWidth, GUIStyle style)
		{
			DraggableLabelField(label, field, value, labelWidth, style, MouseCursor.Link);
		}
		public void DraggableLabelField(string label, string field, Object value, float labelWidth)
		{
			DraggableLabelField(label, field, value, labelWidth, null);
		}
		public void DraggableLabelField(string label, string field, Object value)
		{
			DraggableLabelField(label, field, value, 0);
		}
		public void DraggableLabelField(string label, Object value, float labelWidth)
		{
			DraggableLabelField(label, value == null ? "null" : value.name, value, labelWidth);
		}
		public void DraggableLabelField(string label, Object value)
		{
			DraggableLabelField(label, value, 0);
		}
		public void DraggableLabelField(Object value, float labelWidth = 0)
		{
			DraggableLabelField("", value, labelWidth);
		}
		public void DragDropArea<T>(
			string label, int labelSize, GUIStyle style,
			Predicate<Object[]> canSetVisualModeToCopy, MouseCursor cursor,
			Action<T> onDrop, Action onMouseUp,
			float preSpace, float postSpace = 0f, float height = 0f) where T : UnityEngine.Object
		{
			HorizontalBlock(() =>
			{
				Space(preSpace);
				IndentedBlock(style, () =>
				{
					Space(height / 5);
					HorizontalBlock(() => FlexibleSandwich(() => Label(label, GuiHelper.CreateLabel(labelSize))));
					Space(height / 5);
				});

				GetLastRect(dropArea =>
				{
					// cache the current event
					Event currentEvent = Event.current;

					// if our mouse isn't contained within that box area, exit out
					if (!dropArea.Contains(currentEvent.mousePosition)) return;

					GuiHelper.AddCursorRect(dropArea, cursor);

					if (onMouseUp != null)
						if (currentEvent.type == EventType.MouseUp)
							onMouseUp();

					if (onDrop != null)
					{
						if (currentEvent.type == EventType.DragUpdated ||
							currentEvent.type == EventType.DragPerform)
						{

							// set the visual mode to copy
							DragAndDrop.visualMode = canSetVisualModeToCopy(DragAndDrop.objectReferences) ?
								DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

							// if we dropped something
							if (currentEvent.type == EventType.DragPerform)
							{

								// register that this drag-drop event has been handled by this control
								DragAndDrop.AcceptDrag();

								// loop over the dropped items
								foreach (var item in DragAndDrop.objectReferences)
								{
									onDrop(item as T);
								}
							}
							// since we've used the DragPerform event, we'll mark it as used
							// (its type will change to EventType.Used)
							// so that other Controls ignore it
							Event.current.Use();
						}
					}
				});
				Space(postSpace);
			});
		}

		public void MutablePropertyObjectField(
			SerializedProperty sp,
			bool isObjectField,
			Action<bool> setIsObjectField,
			Object undoObject,
			string label,
			Object dragObject,
			string textFieldValue,
			Action<string> setTextField,
			Action onToggle,
			float labelWidth,
			Predicate predicate = null
			)
		{
			predicate = predicate ?? DefaultMutableFieldTogglePredicate;
			ChangeBlock(() =>
			{
				if (isObjectField)
				{
					DraggableLabelField(label, dragObject, labelWidth);
				}
				else
				{
					Undo.RecordObject(undoObject, "Renamed " + textFieldValue);
					LabelWidthBlock(labelWidth, () =>
					{
						TextField(label, textFieldValue, setTextField);
					});
				}

				GetLastRect(lastRect =>
				{
					if (lastRect.Contains(Event.current.mousePosition))
					{
						if (predicate())
						{
							if (onToggle != null) onToggle();
							setIsObjectField(!isObjectField);
						}
					}
				});
			},
			() => sp.serializedObject.ApplyModifiedProperties());
		}

		/// <summary>
		/// A mutable object field that mutates between being a DraggableLabeledObjectField and a text field for a serialized property.
		/// </summary>
		public void MutablePropertyObjectField(
			SerializedProperty sp,
			SerializedProperty spIsObjectField,
			Object undoObject,
			string label,
			Object dragObject,
			string textFieldValue,
			Action<string> setTextField,
			Action onToggle,
			float labelWidth,
			Predicate predicate = null
			)
		{
			MutablePropertyObjectField(sp, spIsObjectField.boolValue, foldout => spIsObjectField.boolValue = foldout,
				undoObject, label, dragObject, textFieldValue, setTextField, onToggle, labelWidth, predicate);
		}

		public void MutablePropertyObjectField(
			SerializedProperty sp,
			bool isObjectField,
			Action<bool> setIsObjectField,
			string label,
			Action onToggle,
			float labelWidth)
		{
			bool isNull = sp.objectReferenceValue == null;
			Action<string> setter;
			if (isNull) setter = s => { }; // or simply null, but then we have to check if the setter is not null, before we invoke it
			else setter = n => sp.objectReferenceValue.name = n;
			MutablePropertyObjectField(sp, isObjectField, setIsObjectField, sp.gameObject(), label, sp.gameObject(), isNull ? "" : sp.objectReferenceValue.name, setter, onToggle, labelWidth);
		}

		public void MutablePropertyObjectField(
			SerializedProperty sp,
			SerializedProperty spIsObjectField,
			string label,
			Action onToggle,
			float labelWidth)
		{
			MutablePropertyObjectField(sp, spIsObjectField.boolValue, foldout => spIsObjectField.boolValue = foldout, label, onToggle, labelWidth);
		}


		/// <summary>
		/// Default predicate for toggling mutable fields (Ctrl + MiddleMouse Down)
		/// </summary>
		public static readonly Predicate DefaultMutableFieldTogglePredicate = () =>
		{
			return Event.current.control && EventsHelper.MouseEvents.IsMMB_MouseDown();
		};
	}
}