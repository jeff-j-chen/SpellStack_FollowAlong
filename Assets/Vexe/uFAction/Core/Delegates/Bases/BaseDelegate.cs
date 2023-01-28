using System;
using System.Reflection;
using System.Linq;
using Vexe.Runtime.Extensions;
using UnityEngine;

namespace uFAction
{
	/// <summary>
	/// Base class for creating delegates - derive to create your own custom delegates
	/// </summary>
	/// <typeparam name="TDelegate">The type of delegate</typeparam>
	/// <typeparam name="TTarget">The type of target the delegate targets</typeparam>
	[Serializable]
	public abstract class BaseDelegate<TDelegate, TTarget> : ITypedDelegate
		where TDelegate : class
		where TTarget : class
	{
		public T[] GetTypedTargets<T>() where T : TTarget
		{
			return Targets.Cast<T>().ToArray();
		}

		/// <summary>
		/// Returns an array of all targets subscribed (hooked) to this delegate
		/// </summary>
		public abstract TTarget[] Targets { get; }

		/// <summary>
		/// Returns true if the delegate doesn't take any parameters to invoke
		/// </summary>
		public bool IsParameterlessDelegate { get { return ParamTypes == Type.EmptyTypes; } }

		/// <summary>
		/// Returns an array of types for the parameters the delegate takes to invoke
		/// </summary>
		public abstract Type[] ParamTypes { get; }

		/// <summary>
		/// The return type of the delegate
		/// </summary>
		public abstract Type ReturnType { get; }

		/// <summary>
		/// Returns true if the passed delegate is valid - override to implement your custom validity check
		/// By default a valid handler is handler that's niether it nor its target are nulls
		/// </summary>
		/// <param name="handler">The handler to check for its validity</param>
		protected virtual bool IsValidHandler(TDelegate handler)
		{
			return handler != null && GetHandlerTarget(handler) != null;
		}

		/// <summary>
		/// Message used when throwing the invalid handler exception
		/// </summary>
		protected virtual string InvalidHandlerMessage
		{
			get
			{
				return string.Concat(new[] {
					"Make sure neither the handler nor the target are null. ", 
					"If the target was null, could it be that you're trying to add a static method? ", 
					"If so, static methods are not supported. "
				});
			}
		}

		/// <summary>
		/// Checks to see if the passed handler is valid, if not, throw an InvalidOperationException
		/// </summary>
		/// <param name="handler">Handler to assert</param>
		protected void AssertHandlerValidity(TDelegate handler)
		{
			if (!IsValidHandler(handler)) ThrowInvalidHandler(handler);
		}

		/// <summary>
		/// Throws an InvalidOperationException when:
		/// 1- The handler is non-public and the NonPublic binding in the settings is not ticked
		/// 2- Opposite of 1
		/// 3- The handler is not declared in target but yet DeclaredOnly is ticked in the settings
		/// 4- The handler is one of the excluded methods (defined in Settings.MethodsToExclude)
		/// </summary>
		protected void AssertHandlerAndSettingsArePlayingNice(string op, object target, MethodInfo method)
		{
			Action<string[]> throwInvalid = msgs =>
			{
				throw new InvalidOperationException(string.Format(
					string.Concat(msgs),
					op, method.GetFullName(), target.GetType().Name));
			};
			if (Settings.sMethodsToExclude.Contains(method.Name)) {
				throwInvalid(new[] {
					"{0} failed - Method `{1}` in `{2}` is one of the methods listed in the MethodsToExclude list. ", 
					"If you want the operation to succeed, the method must be removed from the exclusion list" 
				});
			}
			var flags = Settings.sMethodBindingFlags;
			if (!method.IsPublic) {
				if ((flags & BindingFlags.NonPublic) == 0) {
					throwInvalid(new[] {
						"{0} failed - Method `{1}` in `{2}` is Non-Public. ", 
						"Yet the `NonPublic` method binding is not ticked in the delegate's settings, ", 
						"if you want the operation to succeed, make sure it is. " 
					});
				}
			}
			else {
				if ((flags & BindingFlags.Public) == 0) {
					throwInvalid(new[] {
						"{0} failed - Method `{1}` in `{2}` is Public. ", 
						"Yet the `Public` method binding is not ticked in the delegate's settings, ", 
						"if you want the operation to succeed, make sure it is. "
					});
				}
			}
			if (!method.IsDeclaredIn(target) && (flags & BindingFlags.DeclaredOnly) > 0) {
				throwInvalid(new[] {
					"{0} failed - Method `{1}` is not declared in `{2}`. ", 
					"Yet `DeclaredOnly` method binding is ticked in the delegate's settings, ",
					"if you want the operation to succeed, make sure it's not ticked. "
				});
			}
		}

		/// <summary>
		/// Throws an InvalidOperationException for the passed handler
		/// </summary>
		protected void ThrowInvalidHandler(TDelegate handler)
		{
			var target = GetHandlerTarget(handler);
			throw new InvalidOperationException(
				string.Format("The target: `{0}` of the handler method: `{1}` is invalid. " + InvalidHandlerMessage,
					target == null ? "null" : target.GetType().Name,
					GetHandlerMethod(handler).Name));
		}

		/// <summary>
		/// An internal getter method that returns the Target object of the specified handler
		/// </summary>
		protected abstract TTarget GetHandlerTarget(TDelegate handler);

		/// <summary>
		/// An internal getter method that returns the MethodInfo of the specified handler
		/// </summary>
		protected abstract MethodInfo GetHandlerMethod(TDelegate handler);
	}
}