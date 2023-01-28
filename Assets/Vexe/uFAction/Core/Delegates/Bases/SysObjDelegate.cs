using UnityEngine;
using System;
using System.Linq;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Extensions;
using System.Runtime.Serialization;

namespace uFAction
{
	/// <summary>
	/// The base class for serialized delegates that target any `System.Object` that is _not_ a UnityEngine.Object (nor contain any)
	/// (in other words - don't use this to target Components - use UnityAction/Func for that instead)
	/// Serializtion is done after each delegate change (removal/addition of handlers).
	/// The delegate serializes to a string - since a string is something Unity could serialize effectively.
	/// Deserialization is done if the delegate was null when trying to get its value.
	/// We deserialize from the data string that we serialized to.
	/// </summary>
	/// <typeparam name="TDelegate">The type of delegate</typeparam>
	[Serializable]
	public abstract class SysObjDelegate<TDelegate> :
		BaseDelegate<TDelegate, System.Object>, IExtraDelegateOps<TDelegate>, IBasicDelegateOps<TDelegate>
#if UNITY_EDITOR
, IViewableDelegate
#endif
 where TDelegate : class
	{
		/// <summary>
		/// The serialized string data for this delegate
		/// </summary>
		[SerializeField]
		protected string serializedData = string.Empty;

		protected TDelegate _delegate;

#if UNITY_EDITOR
		[SerializeField]
		private bool headerToggle;

		[SerializeField]
		protected bool hasBeenModified;
#endif

		/// <summary>
		/// Returns an array of all targets hooked to this delegate
		/// </summary>
		public override object[] Targets { get { return (GetDelegate() as Delegate).GetInvocationList().Select(d => d.Target).ToArray(); } }

		/// <summary>
		/// Removes (unsubscribes) a handler method from the delegate
		/// </summary>
		/// <param name="handler">The handler to be removed</param>
		public void Remove(TDelegate handler)
		{
			AssertHandlerValidity(handler);
			Change(() => _delegate = InternalRemove(handler));
		}

		/// <summary>
		/// Adds (subscribes) a handler method to the delegate
		/// </summary>
		/// <param name="handler">The handler to be added</param>
		public void Add(TDelegate handler)
		{
			AssertHandlerValidity(handler);
			AssertHandlerAndSettingsArePlayingNice("Adding", GetHandlerTarget(handler), GetHandlerMethod(handler));
			try
			{
				Change(() => _delegate = InternalAdd(handler));
			}
			catch { }
		}

		/// <summary>
		/// Returns the delegate value - checks first to see if the delegate is null, if so we try to deserialize it
		/// if we fail (like, in the first Get() call - cause we haven't seialized anything yet) we empty the delegate.
		/// </summary>
		public TDelegate GetDelegate()
		{
			if (_delegate == null && !string.IsNullOrEmpty(serializedData))
				_delegate = SerializationHelper.DeserializeFromString<TDelegate>(serializedData);
			return _delegate;
		}

		/// <summary>
		/// Sets the delegate to the speicifed handler
		/// </summary>
		public void Set(TDelegate handler)
		{
			AssertHandlerValidity(handler);
			Change(() => _delegate = handler);
		}

		/// <summary>
		/// Returns true if the specified handler method is contained in the delegate's invocation list
		/// </summary>
		public bool Contains(TDelegate handler)
		{
			AssertHandlerValidity(handler);
			var del = GetDelegate() as Delegate;
			return del == null ? false :
				del.GetInvocationList()
				   .Contains(d => d.Method == GetHandlerMethod(handler));
		}

		/// <summary>
		/// Clears out the delegate (removes all handlers/subscribers)
		/// </summary>
		public void Clear()
		{
			Change(() => _delegate = null);
		}

		// IViewableDelegate implementation
		#region
#if UNITY_EDITOR
		/// <summary>
		/// [S, G]ets the title header toggle
		/// </summary>
		public bool HeaderToggle { get { return headerToggle; } set { headerToggle = value; } }

		/// <summary>
		/// All the possible views for the delegate - in this case it's ReadOnly
		/// </summary>
		public EditorViewStyle[] PossibleViewStyles { get { return new[] { EditorViewStyle.Readonly }; } }

		/// <summary>
		/// Retruns the current view style
		/// </summary>
		public EditorViewStyle CurrentViewStyle { get { return PossibleViewStyles[0]; } }

		/// <summary>
		/// Since we only have a Readonly-view, cycling doesn't do anything
		/// </summary>
		public void CycleViewStyles()
		{

		}

		/// <summary>
		/// Returns true if the delegate has been modified (used by the editor to re-layout the gui)
		/// </summary>
		public bool HasBeenModifiedFromCode { get { return hasBeenModified; } }
#endif
		#endregion

		/// <summary>
		/// An internal method to change the state of the delegate (add/remove/set).
		/// A serialization is made after the change.
		/// </summary>
		protected void Change(Action change)
		{
#if UNITY_EDITOR
			hasBeenModified = true;
#endif
			change();
			serializedData = _delegate == null ? string.Empty : SerializationHelper.SerializeToString(_delegate);
		}

		/// <summary>
		/// Returns true if the passed delegate is valid.
		/// A valid handler is handler that's niether it nor its target are nulls,
		/// and its target is _not_ a UnityEngine.Object (nor a deriviate)
		/// </summary>
		/// <param name="handler">The handler to check for its validity</param>
		protected override bool IsValidHandler(TDelegate handler)
		{
			bool baseValid = base.IsValidHandler(handler);
			return baseValid && !typeof(UnityEngine.Object).IsAssignableFrom(GetHandlerTarget(handler).GetType());
		}

		/// <summary>
		/// Message thrown when we have an invalid handler
		/// </summary>
		protected override string InvalidHandlerMessage { get { return base.InvalidHandlerMessage + "Also, target must _not_ be a UnityEngine.Object (nor a derivative of it nor contain any)"; } }
		protected abstract TDelegate InternalAdd(TDelegate handler);
		protected abstract TDelegate InternalRemove(TDelegate handler);
	}
}