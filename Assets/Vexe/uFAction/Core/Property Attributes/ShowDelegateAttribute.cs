using System;
using UnityEngine;

namespace uFAction
{
	/// <summary>
	/// The attribute used to decorate SerializedMBDelegates with to integrate them to the editor
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ShowDelegate : PropertyAttribute
	{
		/// <summary>
		/// The title of the delegate that will appear in the editor It's optional.
		/// If you don't pass a title the delegate field name will be used instead
		/// after converting it to Pascal casing and splitting it.
		/// ex: if your delegate field was 'onPlayerSpotted' the title will be 'On Player Spotted'
		/// </summary>
		public readonly string title;

		/// <summary>
		/// If true, the delegate header will always be unfolded (collapsed)
		/// </summary>
		public readonly bool forceExpand;

		/// <summary>
		/// If true, you can set the delegate's arguments (if any) from the editor
		/// </summary>
		public readonly bool canSetArgsFromEditor;

		/// <summary>
		/// If true, the delegate can't be modified from the editor (can't remove/add/set handlers)
		/// </summary>
		//public readonly bool readonlyDelegate;

		public ShowDelegate(string title = "", bool canSetArgsFromEditor = true, bool forceExpand = false)//, bool readonlyDelegate = false)
		{
			this.title = title;
			this.canSetArgsFromEditor = canSetArgsFromEditor;
			this.forceExpand = forceExpand;
			//this.readonlyDelegate = readonlyDelegate;
		}
	}
}