using Object = UnityEngine.Object;
using Vexe.Runtime.Extensions;
using UnityEngine;
using System;
using System.Collections.Generic;
using Vexe.Runtime.Helpers;
using EditorGUIFramework;

namespace uFAction.Editors
{
	public class MiniEditor<TWrapper, TOption> : BaseEditor<TWrapper, TOption>, ISubHeadedEditor
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		protected override void InternalDraw()
		{
			gui.IndentedBlock(GUI.skin.box, HeaderIndent, () =>
			{
				// Sub-header
				DrawSubHeader(
					@showInvoke: IsParameterlessDelegate || CanSetArgsFromEditor,
					@enableInvoke: true,
					@showClear: true,
					@enableClear: true,
					@clear: ClearGOs
				);

				// Line splitter
				gui.Splitter();

				// Entries
				Theme.GameObjectsColors.Reset();
				Theme.TargetColors.Reset();
				Theme.MethodsColors.Reset();
				//Measure("MiniEditor:Main for loop", () =>
				//{
				for (int iLoop = 0; iLoop < goEntries.Count; )
				{
					int i = iLoop;
					var goEntry = goEntries[i];
					var go = goEntry.GO;
					if (go == null) { iLoop++; continue; }
					bool isParam = !IsParameterlessDelegate;
					hasRemovedGo = false;

					Object[] targets = goEntry.Targets;

					if (goEntry.TargetEntries.IsEmpty())
					{
						Log("TargetEntries are empty, adding new");
						goEntry.TargetEntries.Add(new TargetEntry(go));
					}

					var tEntry = goEntry.TargetEntries[0];
					var mEntries = tEntry.MethodEntries;

					if (mEntries.IsEmpty())
					{
						mEntries.Add(new MethodEntry());
					}

					var mEntry = mEntries[0];

					// in case of a Kickass del
					if (paramTypes == null && mEntry.Info != null)
					{
						isParam &= !mEntry.Info.GetActualParams().IsEmpty();
					}

					Action field = () =>
					{
						// GO field
						DoGOField(goEntry, newGo => SetGoEntry(goEntry, i, newGo));

						// Targets popup
						int tIndex = targets.IndexOf(tEntry.Target);
						DoTargetsField(
							@currentIndex: tIndex,
							@targets: targets,
							@getCurrent: () => goEntry.TargetEntries[0].Target,
							@useLabelField: tEntry.GameObject == GOHelper.EmptyGO,
							@color: Theme.TargetColors.NextColor,
							@setTarget: newValue => SetTargetEntry(tEntry, newTarget => goEntry.TargetEntries[0] = newTarget, newValue),
							@setNewIndex: newIndex => tIndex = newIndex
						);

						mEntry = tEntry.MethodEntries[0];

						// Method popup
						gui.ColorBlock(Theme.MethodsColors.NextColor, () =>
							DoMethodsField(
								@mEntry: mEntry,
								@getCurrentInfo: () => tEntry.MethodEntries[0].Info,
								@setInfo: mInfo => SetMethodEntry(mEntry, newValue => tEntry.MethodEntries[0] = newValue, mInfo),
								@methods: getMethods(targets[tIndex].GetType())
							)
						);
					};

					bool isInfoValid = mEntry.Info != null;
					bool showFoldout = isParam && CanSetArgsFromEditor && isInfoValid;

					//Measure("MiniEditor.DrawField()", () => {
					DrawField(
						@color: null,
						@index: i,
						@field: field,
						@moveDown: () => goEntries.MoveElementDown(i),
						@moveUp: () => goEntries.MoveElementUp(i),
						@showFoldout: showFoldout,
						@enableFoldout: isInfoValid,
						@foldout: () => Foldout(goEntry.foldout, f => goEntry.foldout = f),
						@enableRemove: true,
						@whatToRemove: "game object",
						@remove: () => RemoveGo(i)
					);
					//});

					if (showFoldout && goEntry.foldout)
					{
						DoArgEntries(tEntry, 0);
					}

					if (!hasRemovedGo)
						iLoop++;
				}
				gui.Space(1.5f);
			});
			//});
		}

		public void DrawSubHeader(bool showInvoke, bool enableInvoke, bool showClear, bool enableClear, Action clear)
		{
			gui.HorizontalBlock(() =>
			{
				gui.FlexibleSpace();
				gui.BoldLabel("GOs/Targets/Methods");
				gui.FlexibleSpace();

				InvokeAndClearButtons(showInvoke, enableInvoke, showClear, enableClear, clear);

				gui.CheckButton(
					AdvancedMode,
					value => AdvancedMode = value,
					"advancedMode", MiniButtonStyle.ModRight);
			});
		}

		protected override void IntegrateDataToEditor()
		{
			base.IntegrateDataToEditor();

			goEntries = new List<GOEntry>();
			var filtered = FilterGOEntries();

			foreach (var g in filtered)
				foreach (var t in g.TargetEntries)
					foreach (var m in t.MethodEntries)
					{
						var goEntry = new GOEntry(new TargetEntry(t.Target, m));
						goEntry.foldout = g.foldout;
						goEntries.Add(goEntry);
					}
		}
	}
}