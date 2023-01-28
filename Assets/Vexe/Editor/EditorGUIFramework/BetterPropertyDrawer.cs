using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;

namespace EditorGUIFramework
{
	/// <summary>
	/// A better property drawer that uses GUIWrapper to automatically figure out heights, position of Controls and rects
	/// with a bunch of convenience properties and methods for most-often used things.
	/// </summary>
	/// <typeparam name="T">What the drawer is for. Could be Unity object or an attribute</typeparam>
	public abstract class BetterPropertyDrawer<T> : PropertyDrawer where T : class
	{
		protected GUIWrapper gui = new GUIWrapper();
		protected SerializedProperty property;
		protected GUIContent label;
		private bool hasInit;

		/// <summary>
		/// A key that's used in foldout values - override to define a more specific key
		/// </summary>
		protected virtual string key { get { return RTHelper.GetTargetID(target); } }

		/// <summary>
		/// [G, S]ets the boolean flag whose key is specified by the key property
		/// </summary>
		protected bool Foldout
		{
			get { return prefs.GetSafeBool(key); }
			set { prefs.SetBool(key, value); }
		}

		/// <summary>
		/// A reference to the static globally-available BetterPrefs instance
		/// </summary>
		public BetterPrefs prefs { get { return BetterPrefs.Instance; } }

		/// <summary>
		/// Returns the gameObject the target component is attached to
		/// </summary>
		public GameObject gameObject { get { return (target as Component).gameObject; } }

		/// <summary>
		/// Returns a nicified string for the field's name
		/// ex: "myStringField" -> "My String Field"
		/// </summary>
		public string NiceFieldName { get { return fieldInfo.Name.SplitPascalCase(); } }

		/// <summary>
		/// Returns the typed attribute is this drawer was for an attribute, or the typed target object if it was for a specific Unity object
		/// </summary>
		public T TypedValue { get { return typeof(T).IsA<PropertyAttribute>() ? GetTypedAttribute<T>() : fieldInfo.GetValue(target) as T; } }

		/// <summary>
		/// The SerializedObject of the SerializedProperty that this drawer is targeting
		/// </summary>
		public SerializedObject serializedObject { get { return property.serializedObject; } }

		/// <summary>
		/// The target object that the SerializedProperty that drawer is targeting resides in
		/// ex: You have a MyScript MonoBehaviour with MyField that has a custom drawer,
		/// then the target is MyScript
		/// </summary>
		public Object target { get { return serializedObject.targetObject; } }

		/// <summary>
		/// The target's type object
		/// </summary>
		public Type targetType { get { return target.GetType(); } }

		/// <summary>
		/// Returns the System.Type of the drawer's field
		/// </summary>
		public Type fieldType { get { return fieldInfo.FieldType; } }

		/// <summary>
		/// Returns the property's height - uses GUIWrapper's automatic layout to determine the height
		/// </summary>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!hasInit || serializedObject != property.serializedObject)
			{
				hasInit = true;
				Init(property, label);
			}

			float height = gui.Layout(Code);
			if (property.isExpanded)
			{
				int count = property.CountRemaining();
				height += ((count) * GLWrapper.DEFAULT_HEIGHT);
				height += (count * 2f);
			}

			return height;
		}

		/// <summary>
		/// The property's GUI - Do *not* override this if you want to implement your custom GUI, override Code instead
		/// </summary>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			this.property = property;
			gui.Draw(position, Code);
		}

		/// <summary>
		/// Override to provide your custom initialization - good place for assertions too
		/// </summary>
		protected virtual void Init(SerializedProperty property, GUIContent label)
		{
			this.property = property;
			this.label = label;
		}

		/// <summary>
		/// Applies changes to the serializedObject is the specified code block changes (change of an int field value, property field etc)
		/// </summary>
		protected void ApplyAfterChange(Action change)
		{
			gui.ApplyAfterChange(serializedObject, change);
		}

		/// <summary>
		/// Performs a safe-cast on the drawer's attribute to the specific generic argument and returns the result
		/// </summary>
		protected TAttribute GetTypedAttribute<TAttribute>() where TAttribute : class, T
		{
			return attribute as TAttribute;
		}

		/// <summary>
		/// A convenience block for Update-Code-Apply on the serializedObject.
		/// Makes it less often to forget about Applying
		/// </summary>
		protected void SerializedObjectBlock(Action block)
		{
			serializedObject.Update();
			block();
			serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// Convenience method to quickly log messages
		/// </summary>
		protected void log(object msg)
		{
			Debug.Log(msg);
		}

		/// <summary>
		/// Override to provide your custom GUI code
		/// </summary>
		protected abstract void Code();
	}
}