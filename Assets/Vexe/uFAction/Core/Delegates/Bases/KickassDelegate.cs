using Vexe.Runtime.Extensions;
using System;
using UnityEngine;
using System.Reflection;
using Object = UnityEngine.Object;

namespace uFAction
{
	/// <summary>
	/// A serialized delegate that targets any `UnityEngine.Object`
	/// It uses reflection to hook up to any method (of void return) with any parameter defintion
	/// It's a delegate that's really meant to be used from the editor
	/// Although adding/removing delegates from code is still possible, but a bit trickier than regular delegates
	/// As with anything reflection-related, it's better to avoid invoking the delegate 'frequently' in high-performance sensitive areas
	/// </summary>
	[Serializable]
	public class KickassDelegate : BaseUnityDelegate<Delegate>
	{
		/* <<< IBasicDelegateOps overrides, and other ops >>> */
		#region
		/// <summary>
		/// Adds the specified handler to the delegate after asserting that it's valid, and has no parameters
		/// </summary>
		public override void Add(Delegate handler)
		{
			AssertHandlerValidity(handler);
			AssertArgsNumberValidity(handler, 0);
			InternalAdd(handler);
		}

		/// <summary>
		/// Adds to the delegate the specified handler (if it's valid) to be invoked later using the passed direct values
		/// An ArgumentNumberMisMatch gets thrown if the number of direct values is not equal to the number of handler's method parameters
		/// An ArgumentTypeMisMatch gets thrown if one of the directValues types cannot be assigned to its corresponding type in the handler's method parameters
		/// (Like passing an int to a parameter of type Transform, etc)
		/// If it's something trivial and could be implictly casted, then you should explictly cast it
		/// (like passing an int to a parameter of type Single (float))
		/// </summary>
		public void AddUsingValues(Delegate handler, params object[] directValues)
		{
			AssertHandlerValidity(handler);
			int nArgs = directValues.Length;
			var pinfos = AssertArgsNumberValidity(handler, nArgs);
			AssertParamTypesWithValuesValidity(handler, pinfos, directValues);
			var mUsed = InternalAdd(handler);
			mUsed.ReinitArgs(nArgs);
			for (int i = 0; i < nArgs; i++)
			{
				var arg = mUsed.ArgsEntries[i];
				arg.isUsingSource = false;
				arg.directValue.value = directValues[i];
			}
		}

		/// <summary>
		/// Adds to the delegate the specified handler (if it's valid) to be invoked later using the passed source sets
		/// An ArgumentNumberMisMatch gets thrown if the number of sets is not equal to the number of handler's method parameters
		/// An ArgumentTypeMisMatch gets thrown if one of the sets' field types cannot be assigned to its corresponding type in the handler's method parameters
		/// (Like passing an int to a parameter of type Transform, etc)
		/// An ArgumentException gets thrown if a set field doesn't exist in the same set's source
		/// </summary>
		public void AddUsingSource(Delegate handler, params SourceSet[] sets)
		{
			AssertHandlerValidity(handler);
			int nArgs = sets.Length;
			var pinfos = AssertArgsNumberValidity(handler, nArgs);
			AssertSourceValidity(handler, pinfos, sets);
			var mUsed = InternalAdd(handler);
			mUsed.ReinitArgs(nArgs);
			for (int i = 0; i < nArgs; i++)
			{
				var arg = mUsed.ArgsEntries[i];
				var set = sets[i];
				arg.isUsingSource = true;
				arg.sourceTarget = set.source;
				arg.memberName = set.member;
			}
		}

		/// <summary>
		/// Adds to the delegate the specified handler (if it's valid) to be invoked later using the passed field from the passed source
		/// An ArgumentNumberMisMatch gets thrown in this case if the number of handler's method parameters is not equal to one (because we're passing one field)
		/// An ArgumentTypeMisMatch gets thrown if the field's type cannot be assigned to its corresponding type in the handler's method parameters
		/// (Like passing an int to a parameter of type Transform, etc)
		/// An ArgumentException gets thrown if the field doesn't exist in 'source'
		/// </summary>
		public void AddUsingSource(Delegate handler, Component source, string field)
		{
			AddUsingSource(handler, new SourceSet(source, field));
		}

		/// <summary>
		/// Removes the specified handler (if exists) from the delegate
		/// </summary>
		public override void Remove(Delegate handler)
		{
			AssertHandlerValidity(handler);
			DelegateOpsHelper.Remove(
				GetHandlerTarget(handler),
				GetHandlerMethod(handler),
				goEntries,
				null
			);
#if UNITY_EDITOR
			hasBeenModified = true;
#endif
		}

		/// <summary>
		/// Clears out the delegate (removes all handlers/subscribers)
		/// </summary>
		public override void Clear()
		{
			ClearGOs();
		}
		#endregion

		/* <<< Internals >>> */
		#region
		private MethodEntry InternalAdd(Delegate handler)
		{
#if UNITY_EDITOR
			hasBeenModified = true;
#endif
			var target = GetHandlerTarget(handler);
			var method = GetHandlerMethod(handler);
			AssertHandlerAndSettingsArePlayingNice("Adding", target, method);
			return DelegateOpsHelper.Add(target, method, goEntries, null);
		}

		/// <summary>
		/// Asserts that the source sets are valid - that is, the field used does exist, throws an ArgumentException if that's not the case
		/// Also asserts that the types of the handler's method parameters is assignable from the types of fields in the sets
		/// </summary>
		private void AssertSourceValidity(Delegate handler, ParameterInfo[] pinfos, SourceSet[] sets)
		{
			var sBindings = Settings.sSourceBindingFlags;
			for (int i = 0; i < sets.Length; i++)
			{
				var paramType = pinfos[i].ParameterType;
				var set = sets[i];
				var source = set.source;
				var member = set.member;

				Type memberType;
				var pinfo = source.GetProperty(member, sBindings);
				if (pinfo == null)
				{
					var finfo = source.GetField(member, sBindings);
					if (finfo == null)
					{
						var minfo = source.GetMethod(member, sBindings);
						if (minfo == null)
						{
							throw new ArgumentException(string.Format(string.Concat(
								"Couldn't find the member `{0}` in `{1}`. ",
								"Could it be that you need to adjust the source bindings in the settings? ",
								"Maybe you're adding a private member, but `NonPublic` is not ticked? ",
								"Or maybe you're adding a member that's declared in a higher hierarchy but `DeclaredOnly` is ticked?"),
								member, source.GetType().Name));
						}
						memberType = minfo.ReturnType;
					}
					else memberType = finfo.FieldType;
				}
				else memberType = pinfo.PropertyType;

				if (!paramType.IsAssignableFrom(memberType))
					ThrowArgumentMismatch(handler, paramType, memberType, "The return type of `" + member + "` is not valid");
			}
		}

		/// <summary>
		/// Asserts that the types of the handler's method parameters is assignable from the types of `directValues`
		/// </summary>
		private void AssertParamTypesWithValuesValidity(Delegate handler, ParameterInfo[] pinfos, object[] directValues)
		{
			for (int i = 0; i < pinfos.Length; i++)
			{
				var pType = pinfos[i].ParameterType;
				var vType = directValues[i].GetType();
				if (!pType.IsAssignableFrom(vType))
					ThrowArgumentMismatch(handler, pType, vType);
			}
		}

		private static void ThrowArgumentMismatch(Delegate handler, Type pType, Type vType, string extra = "")
		{
			throw new ArgumentTypeMismatch(string.Format(string.Concat(new[]
					{ extra,
					  "Can't add handler `{0}` from target object `{1}` - ", 
					  "There's been a type mis-match between one of the values passed ", 
					  "and the actual handler method parameters. ",
					  "Type `{2}` is not assignable from type `{3}`. ",
					  "(If you're trying to assign an int to a float or vise-versa, then you should explictly cast the value)"
					}), handler.Method.GetFullName(), handler.Target.GetType().Name, pType.FullName, vType.FullName));
		}

		private class ArgumentTypeMismatch : Exception
		{
			public ArgumentTypeMismatch(string msg)
				: base(msg) { }
		}

		private class ArgumentNumberMismatch : Exception
		{
			public ArgumentNumberMismatch(string msg)
				: base(msg) { }
		}

		/// <summary>
		/// Asserts that the number of the handler's method parameters are equal to nReceivedArgs
		/// </summary>
		private ParameterInfo[] AssertArgsNumberValidity(Delegate handler, int nReceivedArgs)
		{
			var pinfos = handler.Method.GetParameters();
			int nRequiredArgs = pinfos.Length;
			if (nReceivedArgs != nRequiredArgs)
			{
				throw new ArgumentNumberMismatch(string.Format(string.Concat(
					"Can't add handler `{0}` from target object `{1}`. ",
					"Arguments number mis-match. Received {2}, expected {3}"),
					handler.Method.Name, handler.Target.GetType().Name, nReceivedArgs, nRequiredArgs));
			}
			return pinfos;
		}

		protected override Object GetHandlerTarget(Delegate handler)
		{
			return handler.Target as Object;
		}

		protected override MethodInfo GetHandlerMethod(Delegate handler)
		{
			return handler.Method;
		}
		#endregion

		/* <<< ITypedDelegate overrides >>> */
		#region
		/// <summary>
		/// The parameter types the delegate takes to invoke - returns null which indicates it could take any type of params
		/// </summary>
		public override Type[] ParamTypes { get { return null; } }

		/// <summary>
		/// The return type of the delegate - returns typeof(void)
		/// </summary>
		public override Type ReturnType { get { return typeof(void); } }
		#endregion
	}
}