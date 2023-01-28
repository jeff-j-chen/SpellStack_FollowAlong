using Vexe.Runtime.Extensions;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

namespace uFAction
{
	public abstract class BaseUnityDelegate<TDelegate> : BaseDelegate<TDelegate, Object>, IBasicDelegateOps<TDelegate>, IInvokableFromEditor
#if UNITY_EDITOR
, IViewableDelegate
#endif
 where TDelegate : class
	{
		// Member fields
		#region
		[SerializeField]
		protected List<GOEntry> goEntries = new List<GOEntry>();

		[SerializeField]
		protected bool cacheHasBeenUpdated;

#if UNITY_EDITOR
		[SerializeField]
		private int _styleIndex;

		[SerializeField]
		protected bool hasBeenModified;

		[SerializeField]
		private bool headerToggle;

		[SerializeField]
		protected bool advancedMode;
#endif
		#endregion

		protected TargetEntry[] TargetEntries { get { return goEntries.SelectMany(e => e.TargetEntries).ToArray(); } }

		/// <summary>
		/// Returns an array of all the target objects that are hooked into the delegate (the objects that the delegate is targeting)
		/// </summary>
		public override Object[] Targets { get { return TargetEntries.Select(e => e.Target).ToArray(); } }

		// IBasicDelegateOps implementation
		#region
		public abstract void Add(TDelegate handler);
		public abstract void Remove(TDelegate handler);
		public abstract void Clear();

		/// <summary>
		/// Returns true if the specified handler method is contained in the delegate's invocation list
		/// </summary>
		public virtual bool Contains(TDelegate handler)
		{

			AssertHandlerValidity(handler);
			return DelegateOpsHelper.Contains(
				GetHandlerMethod(handler),
				TargetEntries
			);
		}
		#endregion

		// IViewableDelegate implementation
		#region
#if UNITY_EDITOR
		public bool HeaderToggle { get { return headerToggle; } set { headerToggle = value; } }

		private int styleIndex { get { return _styleIndex % PossibleViewStyles.Length; } set { _styleIndex = value; } }

		/// <summary>
		/// [S, G]ets the header title toggle (foldout)
		/// </summary>
		//public bool HeaderToggle { get { return headerToggle; } set { headerToggle = value; } }

		/// <summary>
		/// Returns all possible editor view styles for this delegate - in this case, ReadOnly.
		/// </summary>
		public EditorViewStyle[] PossibleViewStyles { get { return new[] { EditorViewStyle.Mini, EditorViewStyle.Advanced }; } }

		/// <summary>
		/// The current view style the delegate is using
		/// </summary>
		public EditorViewStyle CurrentViewStyle { get { return PossibleViewStyles[styleIndex]; } }

		/// <summary>
		/// Cycle view styles from all the possible view styles
		/// </summary>
		public void CycleViewStyles()
		{
			styleIndex = (styleIndex + 1) % PossibleViewStyles.Length;
		}

		/// <summary>
		/// Returns true if the delegate has been modified (used by the editor to know whether or not to re-layout the gui)
		/// </summary>
		public bool HasBeenModifiedFromCode { get { return hasBeenModified; } }
#endif
		#endregion

		// IInvokableFromEditor implementation
		#region
		/// <summary>
		/// Invokes the delgate using the args set from the editor
		/// </summary>
		public virtual void InvokeWithEditorArgs()
		{
			foreach (var t in TargetEntries)
				t.Invoke();
		}
		#endregion

		protected void ClearGOs()
		{
#if UNITY_EDITOR
			if (!goEntries.IsEmpty())
				hasBeenModified = true;
#endif
			goEntries.Clear();
		}
	}
}