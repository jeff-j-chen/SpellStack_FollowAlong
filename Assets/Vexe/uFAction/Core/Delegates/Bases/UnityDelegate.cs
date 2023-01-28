using System;
using UnityEngine;
using System.Linq;
using Vexe.Runtime.Extensions;
using Object = UnityEngine.Object;

namespace uFAction
{
	/// <summary>
	/// The base class for seialized delegates that target `UnityEngine.Object`s
	/// </summary>
	public abstract class UnityDelegate<TDelegate> :
		BaseUnityDelegate<TDelegate>, IRebuildableDelegate, IExtraDelegateOps<TDelegate>
		where TDelegate : class
	{
		protected TDelegate _delegate;
		private Func<Delegate, object[]> _getArgs;
		private Delegate[] _cachedInvocationList;

		private Delegate[] cachedInvocationList
		{
			get
			{
				if (_cachedInvocationList == null)
					RebuildInvocationList();
				return _cachedInvocationList;
			}
		}

		private Func<Delegate, object[]> getArgs
		{
			get
			{
				if (!cacheHasBeenUpdated)
				{
					cacheHasBeenUpdated = true;
					_getArgs = GetMemoizedArguments();
				}
				return _getArgs;
			}
		}

		// IExtraDelegateOps implementation
		#region
		/// <summary>
		/// Sets the delegate to the value specified (will clear all previous target entries)
		/// </summary>
		public void Set(TDelegate handler)
		{
			AssertHandlerValidity(handler);
			AssertHandlerAndSettingsArePlayingNice("Setting the delegate", GetHandlerTarget(handler), GetHandlerMethod(handler));
			_delegate = handler;
			ClearGOs();
			goEntries.Add(new GOEntry(new TargetEntry(GetHandlerTarget(handler), GetHandlerMethod(handler))));
		}

		/// <summary>
		/// Returns the delegate value - Rebuilds the invocation list if the delegate is null
		/// </summary>
		public TDelegate GetDelegate()
		{
			if (_delegate == null) RebuildInvocationList();
			return _delegate;
		}
		#endregion

		// IBasicDelegateOps overrides
		#region
		/// <summary>
		/// Adds the specified handler to the delegate
		/// </summary>
		public override void Add(TDelegate handler)
		{
			AssertHandlerValidity(handler);
			var target = GetHandlerTarget(handler);
			var method = GetHandlerMethod(handler);
			AssertHandlerAndSettingsArePlayingNice("Adding", target, method);

			DelegateOpsHelper.Add(
				target,
				method,
				goEntries,
				() => _delegate = InternalAdd(handler)
			);
#if UNITY_EDITOR
			hasBeenModified = true;
#endif
		}

		/// <summary>
		/// Removes the specified handler (if exists) from the delegate
		/// </summary>
		public override void Remove(TDelegate handler)
		{
			AssertHandlerValidity(handler);

			DelegateOpsHelper.Remove(
				GetHandlerTarget(handler),
				GetHandlerMethod(handler),
				goEntries,
				() =>
				{
					_delegate = InternalRemove(handler);
#if UNITY_EDITOR
					hasBeenModified = true;
#endif
				}
			);
		}

		/// <summary>
		/// Clears out the delegate (removes all handlers/subscribers)
		/// </summary>
		public override void Clear()
		{
			_delegate = null;
			ClearGOs();
		}
		#endregion

		// IRebuildableDelegate implementation
		#region
		/// <summary>
		/// Rebuilds the invocation list of the delegate from its targets and method entries
		/// If the delegate is still null afterwards, An empty delegate is returned (delegate{ }) in the case of Actions
		/// and null in the case of Funcs.
		/// An exception is thrown in case of bind failure
		/// </summary>
		public void RebuildInvocationList()
		{
			// Set the delegate to null if it's not - that's cause if it's not, then we're not rebuilding
			// but actually adding more to it... unwanted result.
			if (_delegate != null) _delegate = null;

			var tEntries = TargetEntries;
			for (int i = 0; i < tEntries.Length; i++)
			{
				var entry = tEntries[i];
				var target = entry.Target;
				if (target == null) continue;
				foreach (var methodEntry in entry.MethodEntries)
				{
					var info = methodEntry.Info;
					if (info == null) continue;
					try
					{
						_delegate = DirectAdd(Delegate.CreateDelegate(typeof(TDelegate), target, info) as TDelegate);
					}
					catch (Exception e)
					{
						Debug.LogError(string.Format("Couldn't re-bind method `{0}` from `{1}` to the invocation list. Reason: {2}",
							info.Name,
							target,
							e.Message));
					}
				}
			}
			_cachedInvocationList = (_delegate as Delegate).GetInvocationList();
		}
		#endregion

		// Internals
		#region
		protected abstract TDelegate DirectAdd(TDelegate handler);
		protected abstract TDelegate InternalAdd(TDelegate handler);
		protected abstract TDelegate InternalRemove(TDelegate handler);
		protected override string InvalidHandlerMessage { get { return base.InvalidHandlerMessage + "Also make sure that the target object is a UnityEngine.Object"; } }
		#endregion

		public override void InvokeWithEditorArgs()
		{
			for (int i = 0; i < cachedInvocationList.Length; i++)
			{
				var handler = cachedInvocationList[i];
				CachedInvokeWithEditorArgs(handler, getArgs(handler));
			}
		}

		public void InvokeWithEditorArgsNotMemoized()
		{
			base.InvokeWithEditorArgs();
		}

		private Func<Delegate, object[]> GetMemoizedArguments()
		{
			return new Func<Delegate, object[]>(handler =>
			{
				return TargetEntries.Where(t => t.Target == (Object)handler.Target)
									.SelectMany(t => t.MethodEntries)
									.FirstOrDefault(m => m.Info.AreMethodsEqualForDeclaringType(handler.Method))
									.ArgsEntries.Select(a => a.value)
									.ToArray();
			}).Memoize();
		}

		protected abstract void CachedInvokeWithEditorArgs(Delegate handler, object[] args);
	}
}