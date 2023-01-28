using EditorGUIFramework;
using UnityEditor;
using UnityEngine;
using DEVBUS;
using System.Collections.Generic;
using System.Linq;
using Vexe.Runtime.Extensions;

namespace Assets.Vexe.uFAction.Scripts.Examples.Editor
{
	/// <summary>
	/// A test editor demonstrating BetterUndo
	/// </summary>
	[CustomEditor(typeof(Target2))]
	public class TestEditor2 : BetterEditor<Target2>
	{
		/// <summary>
		/// Initialize (declare & assign) our BetterUndo instance
		/// </summary>
		private BetterUndo _undo = new BetterUndo();

		/// <summary>
		/// Make our BetterUndo instance the current static globally-available instance
		/// That way we could Undo/Redo via menu items (Undo: Ctrl+Alt+u, Redo: Ctrl+Alt+r)
		/// From this point on, if we wanted to perform any undo operation, we use this getter
		/// instead of _undo
		/// </summary>
		private BetterUndo undo { get { return BetterUndo.MakeCurrent(ref _undo); } }

		/// <summary>
		/// A list to use for demonstration
		/// </summary>
		private List<string> strings;
		private int atIndex;
		private string toValue;

		void OnEnable()
		{
			strings = TypedTarget.strings;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			gui.HorizontalBlock(() =>
			{
				// Basically, RecordXXX = RegisterXXX + PerformXXX
				gui.Button("Add", () =>
					undo.RecordAddToList(() => strings, "New string")
				);
				gui.Button("Remove last", () =>
					undo.RecordRemoveFromList(() => strings, strings.Count - 1)
				);
				gui.Button("Clear from middel till end", () =>
					undo.RecordClearRangeFromTillEnd(() => strings, strings.Count / 2)
				);
				gui.Button("Clear", () =>
					undo.RecordClearList(() => strings)
				);
			});

			gui.Button("Set", () =>
				undo.RecordSetVariable(
					strings[atIndex],							// the current variable value
					newValue => strings[atIndex] = newValue,	// the variable setter
					toValue										// the value to set to
				)
			);

			gui.IntField("Index", atIndex, i => atIndex = i);
			gui.TextField("Value", toValue, s => toValue = s);

			gui.EnabledBlock(strings.Count > 2, () =>
			{
				// You could also create ops and then perform/undo them later on
				// Here we perform a special op, say: set the first and last elements to special values,
				// and remove the middle element. Why? Just because.
				gui.Button("Special Op (Count must be greater than 2)", () =>
				{
					var setFirst = new SetVariable<string>
					{
						GetCurrent = () => strings[0],
						SetValue = s => strings[0] = s,
						ToValue = "Special first"
					};
					var setLast = new SetVariable<string>
					{
						GetCurrent = () => strings.Last(),
						SetValue = s => strings.SetLast(s),
						ToValue = "Special last"
					};
					var removeMiddle = new RemoveFromList<string>
					{
						GetList = () => strings,
						Index = strings.Count / 2
					};

					// For special/custom ops, you could use RecordBasicOps which all it has is an OnPerformed and OnUndone delegates
					// put whatever special code you want inside
					undo.RecordBasicOp(
					() => // This code gets executed when the operation is performed (also when redone)
					{
						Debug.Log("Performed special operation...");
						setFirst.Perform();
						setLast.Perform();
						removeMiddle.Perform();
					},
					() => // This code gets executed when the operation is undone
					{
						removeMiddle.Undo();
						setLast.Undo();
						setFirst.Undo();
						Debug.Log("Undid special operation...");
					});
				});
			});
		}
	}
}