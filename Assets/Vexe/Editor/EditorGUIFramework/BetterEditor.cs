using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Exceptions;
using Vexe.Runtime.Helpers;
using Object = UnityEngine.Object;

namespace EditorGUIFramework
{
	/// <summary>
	/// A better editor class that has a GLWrapper that uses the same API that GUIWrapper does to render GUI Controls
	/// with a bunch of most-often used properties and methods for convenience
	/// </summary>
	public class BetterEditor<T> : Editor where T : Object
	{
		protected GLWrapper gui = new GLWrapper();

		/// <summary>
		/// Returns the typed target object (casts 'target' to the type of component the editor's for)
		/// </summary>
		public T TypedTarget { get { return target as T; } }

		/// <summary>
		/// A reference to the static globally-available BetterPrefs instance
		/// </summary>
		public BetterPrefs Prefs { get { return BetterPrefs.Instance; } }

		/// <summary>
		/// A key that's used in foldout values - override to define a more specific key
		/// </summary>
		protected virtual string key { get { return RTHelper.GetTargetID(target); } }

		/// <summary>
		/// [G, S]ets the boolean flag whose key is specified by the key property
		/// </summary>
		protected bool Foldout
		{
			get { return Prefs.GetSafeBool(key); }
			set { Prefs.SetBool(key, value); }
		}

		/// <summary>
		/// Performs a safe-cast to the editor's target to the specified generic argument and returns the result
		/// </summary>
		public TTarget GetCustomTypedTarget<TTarget>() where TTarget : T
		{
			return target as TTarget;
		}

		/// <summary>
		/// Returns a FieldInfo object for the specified field name
		/// The field has to be public, or marked up with SerializeField
		/// null is returned if not found
		/// </summary>
		public FieldInfo GetFieldInfo(string name)
		{
			return target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
								   .FirstOrDefault(f => f.Name == name && f.IsPublic ||
												   f.IsDefined<SerializeField>());
		}

		/// <summary>
		/// Gets the untyped value of the field whose name is specified by the input argument 'name'
		/// Throws a MemberNotFoundException if the field wasn't found by GetFieldInfo
		/// </summary>
		public object GetFieldValue(string name)
		{
			var field = AssertMemberFound(name);
			return field.GetValue(target);
		}

		/// <summary>
		/// Gets the typed value of the field whose name is specified by the input argument 'name'
		/// Throws a MemberNotFoundException if the field wasn't found by GetFieldInfo
		/// </summary>
		public TValueType GetFieldValue<TValueType>(string name)
		{
			return (TValueType)GetFieldValue(name);
		}

		/// <summary>
		/// Sets the field whose name is specified by the input string to the specified object value
		/// Throws a MemberNotFoundException if the field wasn't found by GetFieldInfo
		/// </summary>
		public void SetFieldValue(string name, object value)
		{
			var field = AssertMemberFound(name);
			field.SetValue(target, value);
		}

		/// <summary>
		/// Convenience method to quickly log messages
		/// </summary>
		protected void log(object msg)
		{
			Debug.Log(msg);
		}

		//SerializedObject mSerializedObject;
		//protected new SerializedObject serializedObject
		//{
		//	set { mSerializedObject = value; }
		//	get
		//	{
		//		if (mSerializedObject == null || mSerializedObject.targetObject == null)
		//			mSerializedObject = new SerializedObject(target);
		//		return mSerializedObject;
		//	}
		//}

		/// <summary>
		/// A convenience block for Update-Code-Apply on the serializedObject.
		/// Makes it less often to forget about Applying
		/// </summary>
		public void SerializedObjectBlock(Action block)
		{
			//if (serializedObject.targetObject == null)
			//{
			//	Repaint();
			//	return;
			//}
			serializedObject.Update();
			block();
			//if (serializedObject.targetObject == null)
			//{
			//	Repaint();
			//	return;
			//}
			serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// Returns a FieldInfo of the specified field if it exists, throws MemberNotFoundException otherwise
		/// </summary>
		private FieldInfo AssertMemberFound(string name)
		{
			var field = GetFieldInfo(name);
			if (field == null)
				throw new MemberNotFoundException(name);
			return field;
		}
	}
}