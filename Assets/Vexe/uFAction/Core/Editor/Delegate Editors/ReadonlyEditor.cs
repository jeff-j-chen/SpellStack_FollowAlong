using Vexe.Editor.Helpers;
using System;
using System.Linq;
using UnityEditor;
using Vexe.Runtime.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Reflection;
using Vexe.Runtime.Helpers;
using EditorGUIFramework;

namespace uFAction.Editors
{
	public class ReadonlyEditor<TWrapper, TOption> : BaseEditor<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		protected override void InternalDraw()
		{
			var del = delegateObject.GetMethod("GetDelegate").Invoke(delegateObject, null) as Delegate;

			if (del.IsEmpty())
			{
				gui.HorizontalBlock(() =>
				{
					gui.Space(5f);
					gui.ColorBlock(GuiHelper.RedColorDuo.SecondColor, () =>
						gui.HelpBox("Delegate is empty", MessageType.Info));
					gui.Space(5f);
				});
				return;
			}

			DoTargets(del.GetGroupedInvocationList().ToList());
		}

		private void DoTargets(List<KeyValuePair<object, MethodInfo[]>> targetMethodsPairs)
		{
			gui.IndentedBlock(GUI.skin.box,
				@indentLevel: .2f,
				@beginningVerticalSpace: 0f,
				@endingVerticalSpace: 5f,
				@block: () =>
				{
					gui.BoldLabel("Targets");
					gui.Splitter();
					Theme.TargetColors.Reset();

					for (int i = 0; i < targetMethodsPairs.Count; )
					{
						var pair = targetMethodsPairs[i];
						var target = pair.Key;

						if (target == null)
						{
							targetMethodsPairs.RemoveAt(i);
							continue;
						}

						DoTarget(target, i);
						gui.IndentedBlock(GUI.skin.box, 1.3f, () => DoMethods(pair));
						i++;
					}
				});
		}

		private void DoTarget(object target, int index)
		{
			gui.HorizontalBlock(() =>
			{
				gui.NumericLabel(index + 1);
				gui.ColorBlock(Theme.TargetColors.NextColor, () =>
					gui.TextFieldLabel(target.GetType().Name));
			});

			if (typeof(Object).IsAssignableFrom(target.GetType()))
			{
				var obj = target as Object;
				gui.GetLastRect(lastRect =>
				{
					GuiHelper.PingField(lastRect, obj, obj != null, MouseCursor.Link);
					GuiHelper.SelectField(lastRect, obj, EventsHelper.MouseEvents.IsRMB_MouseDown());
				});
			}
		}

		private void DoMethods(KeyValuePair<object, MethodInfo[]> pair)
		{
			gui.BoldLabel("Methods");
			gui.Splitter();
			Theme.MethodsColors.Reset();
			for (int j = 0; j < pair.Value.Length; j++)
			{
				var method = pair.Value[j];
				gui.HorizontalBlock(() =>
				{
					gui.NumericLabel(j + 1);
					gui.ColorBlock(Theme.MethodsColors.NextColor, () =>
						gui.TextFieldLabel(method.Name));
				});
			}
		}
	}
}