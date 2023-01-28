using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

#pragma warning disable 0649

namespace EditorGUIFramework.Tests.Editor
{
	/// <summary>
	/// A test drawer demonstrating GUIWrapper's API
	/// The API is very similar to your standard GUILayout.XXX
	/// but the main difference is that most methods take a delegate at the end
	/// so instead of:
	/// if (GUILayout.Button(...)) {
	///		// code ...
	/// }
	/// you say:
	/// gui.Button(..., () => {
	///		// code ...
	/// });
	/// 
	/// instead of:
	/// x = EditorGUILayout.IntField(label, value);
	/// you say:
	/// gui.IntField(label, value, newValue => x = newValue);
	/// 
	/// Another very important thing, is that if you're using GUIWrapper, you should notify it of any change
	/// in height via gui.HeightHasChanged(). Usually the change of height is because of a foldout.
	/// If you don't do it, you'll notice a slight delay when folding/unfolding.
	/// Please see example below.
	/// </summary>
	[CustomPropertyDrawer(typeof(DrawMeInACustomWay))]
	public class TestDrawer : BetterPropertyDrawer<DrawMeInACustomWay>
	{
		private string[] options = { "Option1", "Option2", "Option3" };
		private int selectionIndex;
		private Bounds boundsValue;
		private Rect rectValue;
		private Color colorValue1;
		private Color colorValue2;
		private int intValue;
		private float floatValue;
		private bool toggle;

		string stringObject = "New value";
		int intObject = 123;
		Transform transform = null;

		protected override void Code()
		{
			gui.BoundsField(boundsValue, newBounds => boundsValue = newBounds);
			gui.RectField(rectValue, newRect => rectValue = newRect);

			gui.HorizontalBlock(() =>
			{
				gui.ColorField(colorValue1, newColor => colorValue1 = newColor);
				gui.ColorField(colorValue2, newColor => colorValue2 = newColor);
			});

			gui.PropertyField(property.FindPropertyRelative("name"));


			gui.ChangeBlock(
			() =>
			{
				gui.FloatField("FloatValue", floatValue, newValue => floatValue = newValue);
				gui.IntField("IntValue", intValue, newValue => intValue = newValue);
			},
			() => Debug.Log("Something changed"));

			gui.HorizontalBlock(() =>
			{
				gui.EnabledBlock(toggle, () => gui.Button("BUTTON"));
				gui.FlexibleSpace();
				gui.AddButton("something", () => Debug.Log("Adding stuff..."));
				gui.ClearButton("everything", () => Debug.Log("Clearing, you sure about this?"));
				gui.RemoveButton("Stuff", MiniButtonStyle.Right, () => Debug.Log("Removing stuff..."));
			});

			gui.Button("toggle", () =>
			{
				toggle = !toggle;
				Debug.Log("Toggled to: " + toggle);
			});

			gui.EnabledBlock(toggle, () =>
				gui.HorizontalBlock(() =>
				{
					gui.Button(toggle ? "on" : "off", () => Debug.Log("I'm on"));
					gui.Button(toggle ? "On" : "Off", () => Debug.Log("I'm On"));
					gui.Button(toggle ? "ON" : "OFF", () => Debug.Log("I'm ON"));
				}));

			gui.IndentedBlock(GUI.skin.box, 1, () =>
			{
				var spFlag1 = property.FindPropertyRelative("flag1");
				gui.FoldoutBold(spFlag1, "You'll notice a slight delay expanding this");
				if (spFlag1.boolValue)
				{
					gui.LabelWidthBlock(80f, () =>
					{
						gui.FieldOfType(stringObject, "String Field", value => stringObject = value);
						gui.FieldOfType(intObject, "Int Field", value => intObject = value);
						gui.FieldOfType(transform, "Transform", t => transform = t);
					});
				}

				gui.Splitter();

				var spFlag2 = property.FindPropertyRelative("flag2");
				gui.HorizontalBlock(() =>
				{
					gui.Space(10f);
					gui.Foldout(spFlag2.boolValue, "No delay here", value =>
					{
						if (spFlag2.boolValue != value)
						{
							spFlag2.boolValue = value;

							// Notify the gui that there was a height change
							// This is what eliminates the delay
							// Usually height changes in a foldout
							gui.HeightHasChanged();
						}
					});
				});

				if (spFlag2.boolValue)
				{
					gui.Splitter();
					gui.HorizontalBlock(() =>
					{
						gui.Label(">");
						Rect foldRect = new Rect();
						gui.GetLastRect(lastRect => foldRect = lastRect);
						gui.Label("Turn " + (toggle ? "off" : "on"));
						gui.GetLastRect(lastRect =>
						{
							if (GUI.Button(CombineRects(foldRect, lastRect), GUIContent.none, GUIStyle.none))
							{
								toggle = !toggle;
							}
						});
						gui.FlexibleSpace();
						gui.EnabledBlock(toggle, () =>
							gui.ColorBlock(toggle ? Color.green : Color.red, () =>
								gui.Button(toggle ? "on" : "off", () => Debug.Log("I'm on"))));
					});

					gui.NumericTextFieldLabel(1, "this is cool! hueheu");
					gui.PropertyField(property.FindPropertyRelative("name"));
					gui.Popup("Method", selectionIndex, options, newIndex => selectionIndex = newIndex);

					gui.DragDropArea<GameObject>(
						label: "Drag-drop GameObject",
						labelSize: 13,
						style: GUI.skin.textArea,
						canSetVisualModeToCopy: dragObjects => dragObjects.All(o => o is GameObject),
						cursor: MouseCursor.Link,
						onDrop: log,
						onMouseUp: () => log("Click"),
						preSpace: 20f
					);
				}
			});
		}

		public static Rect CombineRects(Rect a, Rect b)
		{
			return new Rect(Math.Min(a.x, b.x), Math.Min(a.y, b.y), a.width + b.width, Math.Max(a.height, b.height));
		}
	}
}