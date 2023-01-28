using Vexe.Runtime.Extensions;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using sp = UnityEditor.SerializedProperty;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Reflection;
using Vexe.Runtime.Helpers;
using Vexe.Editor.Helpers;
using EditorGUIFramework;
using Debug = UnityEngine.Debug;
using DEVBUS;
using ShowEmAll.DrawMates;
using Vexe.Editor.Extensions;
using System.Diagnostics;
using Fasterflect;

namespace uFAction.Editors
{
	public interface ISubHeadedEditor
	{
		void DrawSubHeader(bool showInvoke, bool enableInvoke, bool showClear, bool enableClear, Action clear);
	}

	public abstract class BaseEditor<TWrapper, TOption> : BaseDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		// Member fields
		#region
		protected const string kGoEntries = "goEntries";
		protected const string NA = @"N\A";
		protected const float IndentLevel = 1.3f;
		protected const float HeaderIndent = .25f;

		protected object delegateObject;
		protected List<GOEntry> goEntries;
		protected bool hasRemovedGo;

		//private sp spDelegate;
		private string title;
		private bool canSetArgsFromEditor;
		private bool forceExpand;
		#endregion

		// Properties
		#region
		private IViewableDelegate ViewableDelegate { get { return delegateObject as IViewableDelegate; } }
		private ITypedDelegate TypedDelegate { get { return delegateObject as ITypedDelegate; } }
		private IInvokableFromEditor InvokableFromEditorDelegate { get { return delegateObject as IInvokableFromEditor; } }
		private IRebuildableDelegate RebuildableDelegate { get { return delegateObject as IRebuildableDelegate; } }
		private bool HeaderToggle
		{
			get { return ViewableDelegate.HeaderToggle; }
			set { ViewableDelegate.HeaderToggle = value; }
		}
		protected bool AdvancedMode
		{
			get { return (bool)getAdvancedMode(delegateObject); }
			set { setAdvancedMode(delegateObject, value); }
		}
		protected Type[] paramTypes { get { return TypedDelegate.ParamTypes; } }
		private Type delegateReturnType { get { return TypedDelegate.ReturnType; } }
		private Assembly[] extMethodsAsms { get { return new[] { typeof(Vexe.Runtime.Extensions.TypeExtensions).Assembly, typeof(UnityAction).Assembly }; } }
		protected bool IsParameterlessDelegate { get { return TypedDelegate.IsParameterlessDelegate; } }
		protected bool CanSetArgsFromEditor { get { return canSetArgsFromEditor; } }
		protected static ColorTheme Theme { get { return Settings.sColorTheme; } }
		#endregion

		//private bool profile;
		private MemberSetter setGoEntries;
		private MemberGetter getGoEntries;
		private MemberGetter getAdvancedMode;
		private MemberSetter setAdvancedMode;
		private Type delegateType;
		private Func<Object[], string[]> getTargetsNames;
		private Func<MethodInfo[], string[]> getMethodsFullNames;
		protected Func<Type, MethodInfo[]> getMethods;

		private void Cache()
		{
			delegateType = delegateObject.GetType();
			setGoEntries = delegateType.DelegateForSetFieldValue(kGoEntries);
			getGoEntries = delegateType.DelegateForGetFieldValue(kGoEntries);
			setAdvancedMode = delegateType.DelegateForSetFieldValue("advancedMode");
			getAdvancedMode = delegateType.DelegateForGetFieldValue("advancedMode");

			getMethods = new Func<Type, MethodInfo[]>(type =>
			{
				//log("Getting methods");
				var methods = type.GetMethods(delegateReturnType,
											   paramTypes,
											   Settings.sMethodBindingFlags,
											   true)
												.Concat(Settings.sShowExtensionMethods ?
													GetExtensionMethods(type) :
													Enumerable.Empty<MethodInfo>())
								 .ToList();
				var disinclude = Settings.sMethodsToExclude;
				methods.RemoveAll(m => disinclude.Contains(m.Name));
				return methods.ToArray();
			}).Memoize();

			getTargetsNames = new Func<Object[], string[]>(targets =>
			{
				//log("Getting type names");
				var names = targets.Select(t => t.GetType().Name).ToArray();
				int gIndex = names.IndexOf("GameObject");
				if (gIndex != -1)
				{
					names[gIndex] = names[gIndex].Insert(0, "[");
					names[gIndex] += "]";
				}

				// I had to do this since it seems that Popup doesn't show duplicate entries
				// So if we had two of the same Components on a GO, the popup would only show one of them
				var duplicatesIndicesLists = names.GetDuplicates().Select(d => d.Value);
				foreach (var dupList in duplicatesIndicesLists)
				{
					for (int i = 0; i < dupList.Length; i++)
					{
						names[dupList[i]] += " (" + (i + 1) + ")";
					}
				}
				return names;
			}).Memoize();

			getMethodsFullNames = new Func<MethodInfo[], string[]>(methods =>
			{
				//log("Getting full names");
				return methods.Select(m => m.GetFullName(ReflectionHelper.TypeNameGauntlet)).ToArray();
			}).Memoize();
		}

		//protected void Measure(string msg, Action code)
		//{
		//	Stopwatch w = null;
		//	if (profile)
		//		w = Stopwatch.StartNew();
		//	code();
		//	if (w != null && w.IsRunning)
		//	{
		//		w.Stop();
		//		log(msg + ": " + w.ElapsedMilliseconds);
		//	}
		//}

		public override void Draw()
		{
			//Measure("Draw()", () =>
			//{
				DrawTitleHeader();
				gui.Space(-4f);
				bool hasBeenModified = ViewableDelegate.HasBeenModifiedFromCode;
				if (!HeaderToggle && !hasBeenModified) return;

				var isEditorDel = InvokableFromEditorDelegate != null;
				var isViewable = ViewableDelegate != null;

				if (isViewable && hasBeenModified)
				{
					IntegrateDataToEditor();
					HeaderToggle = true;
					HeightHasChanged();
				}
				gui.ChangeBlock(() =>
				{
					if (isViewable && !isEditorDel || isEditorDel && !goEntries.IsNullOrEmpty())
					{
						//Measure("InternalDraw", InternalDraw);
						InternalDraw();
						gui.Space(-4f);
					}
					if (isEditorDel)
					{
						AddingArea();
						ApplyGOEntries();
					}
				}, () =>
				{
					if (RebuildableDelegate != null)
					{
						Log("rebuilding");
						RebuildableDelegate.RebuildInvocationList();
					}
				});
			//});
		}

		// [G, S]etters
		#region
		protected void SetFoldout(string key, bool from, bool to, Action onDo = null)
		{
			undo.RecordSetVariable(from, f => BetterPrefs.sSetBool(key, f), to, onDo + DelayedFocus, DelayedFocus);
		}

		private IEnumerable<MethodInfo> GetExtensionMethods(Type from)
		{
			var publicMod = Settings.sMethodBindingFlags & BindingFlags.Public;
			var privateMod = Settings.sMethodBindingFlags & BindingFlags.NonPublic;
			return GetExtensionMethods(
				@type: from,
				@asms: extMethodsAsms,
				@higherType: typeof(Object),
				@returnType: delegateReturnType,
				@paramTypes: paramTypes,
				@modifiers: publicMod | privateMod,
				@exactBinding: false
			);
		}
		private IEnumerable<MethodInfo> GetExtensionMethods(
			Type type, Assembly[] asms, Type higherType,
			Type returnType, Type[] paramTypes,
			BindingFlags modifiers, bool exactBinding)
		{
			if (!Settings.sShowExtensionMethods)
				return Enumerable.Empty<MethodInfo>();

			return type.GetExtensionMethods(asms, higherType, returnType, paramTypes, modifiers, exactBinding);
		}

		protected void ApplyGOEntries()
		{
			setGoEntries(delegateObject, goEntries);
		}

		protected void SetTargetEntry(TargetEntry current, Action<TargetEntry> set, Object newValue)
		{
			SetTargetEntry(current, set, new TargetEntry(newValue, new MethodEntry()));
		}

		protected void SetTargetEntry(TargetEntry current, Action<TargetEntry> set, TargetEntry newValue)
		{
			undo.RecordSetVariable(
				@get: () => current,
				@set: set,
				@toValue: newValue,
				@onPerformed: HeightHasChanged,
				@onUndone: HeightHasChanged
			);
		}
		protected void SetGoEntry(GOEntry current, int index, GameObject to)
		{
			SetGoEntry(current, index, new GOEntry(to));
		}
		protected void SetGoEntry(GOEntry current, int index, GOEntry to)
		{
			undo.RecordSetVariable(
				@get: () => current,
				@set: newEntry => goEntries[index] = newEntry,
				@toValue: to
			);
		}
		#endregion

		// Editor stuff
		#region
		protected void DoGOField(GameObject currentValue, bool enableField, Color? color, Action<GameObject> setNewValue)
		{
			gui.EnabledBlock(enableField, () =>
				gui.ColorBlock(color, () =>
				{
					if (enableField)
						gui.DraggableObjectField(currentValue, setNewValue);
					else
						gui.TextFieldLabel(currentValue.name);
				})
			);

			if (enableField)
				gui.GetLastRect(lastRect =>
					GuiHelper.AddCursorRect(lastRect, MouseCursor.Link)
				);
		}
		protected void DoGOField(GOEntry goEntry, Action<GameObject> set)
		{
			DoGOField(
				@currentValue: goEntry.GO,
				@enableField: !goEntry.IsNa,
				@color: Theme.GameObjectsColors.NextColor,
				@setNewValue: newValue =>
				{
					if (newValue != goEntry.GO)
					{
						set(newValue);
					}
				});
		}

		protected void InvokeAndClearButtons(bool showInvoke, bool enableInvoke, bool showClear, bool enableClear, Action clear)
		{
			if (showInvoke)
				gui.EnabledBlock(enableInvoke, () =>
					gui.MiniButton("I", "Invoke the delegate", InvokableFromEditorDelegate.InvokeWithEditorArgs));

			if (showClear)
				gui.EnabledBlock(enableClear, () =>
					gui.ClearButton("game objects", clear));
		}

		protected void RemoveGo(int i)
		{
			undo.RecordRemoveFromList(
				@getList: () => goEntries,
				@index: i,
				@onPerformed: () =>
				{
					hasRemovedGo = true;
					HeightHasChanged();
				},
				@onUndone: HeightHasChanged
			);
		}

		private void ClearListAndNotifyHeightHasChanged<T>(Func<List<T>> getList)
		{
			undo.RecordClearList(
				@getList: getList,
				@onPerformed: HeightHasChanged,
				@onUndone: HeightHasChanged
			);
		}

		protected void ClearGOs()
		{
			ClearListAndNotifyHeightHasChanged(() => goEntries);
		}

		public void Set(sp spDelegate, object delegateObject, string title, bool canSetArgsFromEditor, bool forceExpand, TWrapper gui)
		{
			this.delegateObject = delegateObject;
			Set(title, canSetArgsFromEditor, forceExpand);
			this.gui = gui;
			target = spDelegate.serializedObject.targetObject;
			try
			{
				Cache();
			}
			catch { }
			IntegrateDataToEditor();
		}

		public void Set(string title, bool canSetArgsFromEditor, bool forceExpand)
		{
			this.title = title;
			this.canSetArgsFromEditor = canSetArgsFromEditor;
			this.forceExpand = forceExpand;
		}

		public void DrawTitleHeader()
		{
			var toggle = HeaderToggle;
			gui.ColorBlock(toggle ? Color.white : GuiHelper.LightGreyColorDuo.FirstColor,
				() => gui.HorizontalBlock(EditorStyles.miniButton, () =>
				{
					string fold = toggle ? GuiHelper.Folds.DefaultFoldSymbol : GuiHelper.Folds.DefaultExpandSymbol;
					gui.Label(fold, new TOption { Width = 15 });
					var foldRect = new Rect();
					gui.GetLastRect(lastRect => foldRect = lastRect);
					gui.Label(title, GuiHelper.CreateLabel(12, new Vector2(-6, -1f)));
					gui.GetLastRect(lastRect =>
					{
						if (GUI.Button(GuiHelper.CombineRects(foldRect, lastRect), "", GUIStyle.none))
						{
							HeaderToggle = !toggle || forceExpand;
							if (toggle != HeaderToggle)
								HeightHasChanged();
						}
					});

					//gui.MiniButton("P", "Toggle profiler", () => profile = !profile);

					if (Settings.sDebugMode)
					{
						gui.MiniButton("g", "Print GO entries", PrintGOs);
						gui.MiniButton("t", "Print targets", PrintTargets);
						gui.MiniButton("m", "Print methods", PrintMethods);
					}

					gui.MiniButton("S", "Go to settings",
						() => undo.RecordSelection(Settings.Instance, target));

					gui.EnabledBlock(ViewableDelegate.PossibleViewStyles.Length > 1, () =>
						gui.MiniButton("V", "Switch view style", MiniButtonStyle.Right, () =>
						{
							Action cycle = () =>
							{
								ViewableDelegate.CycleViewStyles();
								HeightHasChanged();
							};
							undo.RecordBasicOp(cycle, cycle);
						})
					);
				})
			);
		}

		protected List<GOEntry> FilterGOEntries(List<GOEntry> source)
		{
			var filtered = source.Where(e => e != null && e.GO != null).ToList();
			for (int i = filtered.Count - 1; i > -1; i--)
			{
				var e = filtered[i];
				e.TargetEntries = FilterTargetEntries(e.TargetEntries);
				if (e.TargetEntries.IsEmpty())
				{
					filtered.RemoveAt(i);
				}
			}
			return filtered;
		}
		protected List<GOEntry> FilterGOEntries()
		{
			//return FilterGOEntries(delegateObject.GetValue<List<GOEntry>>("goEntries"));
			return FilterGOEntries((List<GOEntry>)getGoEntries(delegateObject));
		}
		private List<TargetEntry> FilterTargetEntries(IEnumerable<TargetEntry> source)
		{
			var filtered = source.Where(e => e != null && e.Target != null).ToList();
			for (int i = filtered.Count - 1; i > -1; i--)
			{
				var e = filtered[i];
				e.MethodEntries = FilterMethods(e.MethodEntries);
				if (e.MethodEntries.IsEmpty())
				{
					filtered.RemoveAt(i);
				}
			}
			return filtered;
		}

		private List<MethodEntry> FilterMethods(IEnumerable<MethodEntry> source)
		{
			return source.Where(e => e != null && !string.IsNullOrEmpty(e.Name)).ToList();
		}

		private void AddingArea()
		{
			Action<Object> add = newTarget => undo.RecordAddToList(
				@getList: () => goEntries,
				@value: new GOEntry(new TargetEntry(newTarget)),
				@onPerformed: () =>
				{
					goEntries.Last().foldout = true;
					HeaderToggle = true;
					HeightHasChanged();
				},
				@onUndone: HeightHasChanged
			);

			gui.DragDropArea(
				@label: "+",
				@labelSize: 17,
				@style: GUI.skin.textField,
				@canSetVisualModeToCopy: dragObjects => true,
				@cursor: MouseCursor.Link,
				@onDrop: add,
				@onMouseUp: () =>
					SelectionWindow.Show<GameObject>(
						@getValues: () => Object.FindObjectsOfType<GameObject>().Where(go => go != GOHelper.EmptyGO).ToArray(),
						@getTarget: () => null,
						@setTarget: go =>
						{
							add(go);

							if (!Settings.sKeepSelectionWindowActiveAfterAdd)
								EditorHelper.FocusOnWindow("Inspector");
						},
						@getValueName: go => go.name,
						@label: "GameObjects"),
				@preSpace: 10f,
				@postSpace: 5f,
				@height: 1f
			);
		}
		protected static void Log(string msg)
		{
			if (Settings.sDebugMode) Debug.Log(msg);
		}
		protected abstract void InternalDraw();
		protected virtual void IntegrateDataToEditor()
		{
			Log("Integrating data...");
			delegateObject.SetValue("hasBeenModified", false);
		}

		protected void MoveUpDown(Action moveDown, Action moveUp)
		{
			if (AdvancedMode)
			{
				Action down = () => undo.RecordBasicOp(moveDown, moveDown);
				Action up = () => undo.RecordBasicOp(moveUp, moveUp);
				gui.MoveDownButton(down);
				gui.MoveUpButton(up);
			}
		}

		protected void Foldout(bool current, Action<bool> setValue)
		{
			gui.Space(8f);
			gui.Foldout(current, newValue =>
			{
				if (newValue != current)
				{
					Action a = () =>
					{
						HeightHasChanged();
						DelayedFocus();
					};
					undo.RecordSetVariable(current, setValue, newValue, a, a);
				}
			});
			gui.Space(-3f);
		}

		protected void DelayedFocus()
		{
			EditorApplication.delayCall += () => EditorHelper.FocusOnWindow("Inspector");
		}

		// Arg entries stuff
		#region
		protected void DoArgEntries(TargetEntry tEntry, int index)
		{
			var mEntry = tEntry.MethodEntries[index];
			mEntry.CheckArgs(paramTypes);
			gui.HorizontalBlock(() =>
			{
				gui.Indent(IndentLevel);
				gui.VerticalBlock(GUI.skin.box, () =>
				{
					gui.BoldLabel("Args:");
					gui.Splitter();

					var args = mEntry.ArgsEntries;
					var pTypes = paramTypes ?? mEntry.Info
												.GetActualParams()
												.Select(p => p.ParameterType)
												.ToArray();

					for (int iLoop = 0; iLoop < args.Length; iLoop++)
					{
						int i = iLoop;
						var arg = args[i];
						var retType = pTypes[i];

						gui.HorizontalBlock(() =>
						{
							gui.Label((i + 1) + "- (" + ReflectionHelper.TypeNameGauntlet(retType) + "): ");

							gui.ChangeBlock(() =>
								gui.EnabledBlock(!arg.isUsingSource, () =>
									 DoArgDirect(arg, retType)),
								MarkCacheUpdate
							);

							Foldout(arg.isUsingSource, newValue =>
								arg.isUsingSource = newValue);
						});

						if (arg.isUsingSource)
						{
							DoArgFromSource(arg, retType);
						}
					}
				});
			});
		}

		private void MarkCacheUpdate()
		{
			delegateObject.SetValue("cacheHasBeenUpdated", false);
		}

		private void DoArgDirect(ArgEntry arg, Type type)
		{
			var direct = arg.directValue;

			if (type == typeof(int))
			{
				gui.IntField(direct.intValue, newValue => direct.value = newValue);
			}
			else if (type == typeof(float))
			{
				gui.FloatField(direct.floatValue, newValue => direct.value = newValue);
			}
			else if (type == typeof(bool))
			{
				gui.Toggle(direct.boolValue, newValue => direct.value = newValue);
			}
			else if (type == typeof(string))
			{
				gui.TextField(direct.stringValue, newValue => direct.value = newValue);
			}
			else if (type == typeof(Vector3))
			{
				gui.Vector3Field(direct.vector3Value, newValue => direct.value = newValue);
			}
			else if (type == typeof(Vector2))
			{
				gui.Vector2Field(direct.vector2Value, newValue => direct.value = newValue);
			}
			else if (type == typeof(Bounds))
			{
				gui.BoundsField(direct.boundsValue, newValue => direct.value = newValue);
			}
			else if (type == typeof(Rect))
			{
				gui.RectField(direct.rectValue, newValue => direct.value = newValue);
			}
			else if (type == typeof(Color))
			{
				gui.ColorField(direct.colorValue, newValue => direct.value = newValue);
			}
			else if (type.IsEnum)
			{
				direct.SetEnums(type);
				gui.Popup(direct.enumIndex, direct.enumOptions, newIndex => direct.enumIndex = newIndex);
			}
			else if (typeof(Object).IsAssignableFrom(type))
			{
				gui.ObjectField(direct.objectRefValue, type, newValue => direct.value = newValue);
			}
			else if (type == typeof(Quaternion))
			{
				gui.Vector3Field(direct.quaternionValue.eulerAngles, direct.SetQuaternionFromVector3);
			}
			else
			{
				gui.ColorBlock(GuiHelper.RedColorDuo.FirstColor, () =>
					gui.TextFieldLabel("Can't set value directly. Select from a source instead ->"));
			}
		}

		private void DoArgFromSource(ArgEntry arg, Type argReturnType)
		{
			gui.HorizontalBlock(() =>
			{
				gui.Label("From: ", new TOption { Width = 40f });

				// Source GO field
				var go = arg.sourceGo;
				DoGOField(
					@currentValue: go,
					@enableField: true,
					@color: null,
					@setNewValue: newValue =>
					{
						if (arg.sourceGo != newValue)
						{
							arg.sourceGo = newValue;
						}
					}
				);

				// Target popup
				var targets = go == null ? null : go.GetAllComponentsIncludingSelf();
				DoTargetsField(
					@currentIndex: go == null ? -1 : targets.IndexOf(arg.sourceTarget),
					@targets: targets,
					@getCurrent: () => arg.sourceTarget,
					@useLabelField: false,
					@color: null,
					@setTarget: newValue =>
					{
						if (arg.sourceTarget != newValue)
						{
							arg.sourceTarget = newValue;
						}
					},
					@setNewIndex: delegate { }
				);

				// Field name popup
				gui.ChangeBlock(() =>
					DoArgFieldPopup(arg, argReturnType),
					MarkCacheUpdate
				);
			});
		}
		private void DoArgFieldPopup(ArgEntry arg, Type returnType)
		{
			var target = arg.sourceTarget;
			string[] membersNames;
			int mIndex;
			if (target == null || arg.sourceGo == null)
			{
				membersNames = null;
				mIndex = -1;
			}
			else
			{
				var sBindings = Settings.sSourceBindingFlags;

				var extensionMethods = GetExtensionMethods(
					@type: target.GetType(),
					@asms: extMethodsAsms,
					@higherType: typeof(Object),
					@returnType: returnType,
					@paramTypes: Type.EmptyTypes,
					@modifiers: sBindings & BindingFlags.Public | sBindings & BindingFlags.NonPublic,
					@exactBinding: false
				);

				membersNames = target.GetFields(returnType, sBindings)
					.Concat(target.GetProperties(returnType, sBindings).Cast<MemberInfo>())
					.Concat(target.GetMethods(returnType, Type.EmptyTypes, sBindings, false)
											  .Where(m => !m.IsSpecialName)
											  .Cast<MemberInfo>())
					.Concat(extensionMethods.Cast<MemberInfo>())
					.Select(m =>
					{
						return m.MemberType == MemberTypes.Method ?
							(m as MethodInfo).GetFullName("[ext] ", ReflectionHelper.TypeNameGauntlet) :
							m.Name;
					})
					.ToArray();

				mIndex = membersNames.IndexOf(arg.memberName);
			}

			if (mIndex == -1)
			{
				Log("ArgMember: member not found - setting index to 0");
				mIndex = 0;
			}
			if (membersNames.IsNullOrEmpty())
				membersNames = new[] { NA };

			DoWindowOrPopup(
				@currentIndex: mIndex,
				@names: membersNames,
				@values: membersNames,
				@getCurrent: () => arg.memberName,
				@windowLabel: "Members",
				@popupIsNewValue: newValue => arg.memberName != newValue,
				@setToNewValue: newValue =>
				{
					if (arg.memberName != newValue)
					{
						arg.memberName = newValue;
					}
				},
				@setNewIndex: delegate { }
			);
		}
		#endregion

		// Fields / drawings
		#region
		protected void DoTargetsField(
			int currentIndex, Object[] targets, Func<Object> getCurrent,
			bool useLabelField, Color? color,
			Action<Object> setTarget, Action<int> setNewIndex)
		{
			string[] tNames;
			Object current = getCurrent();
			//tNames = targets == null ? null : getTargetsNames(targets.Select(t => t.GetType()));
			tNames = targets == null ? null : getTargetsNames(targets);

			if (currentIndex == -1)
			{
				Log("TargetsPopup: target not found - setting index to 0");
				currentIndex = 0;
				setNewIndex(0);
			}

			if (tNames.IsNullOrEmpty())
				tNames = new[] { NA };

			Action field = useLabelField ?
				(Action)(() => gui.DraggableLabelField(current)) :
				(Action)(() => DoWindowOrPopup(
					@currentIndex: currentIndex,
					@names: tNames,
					@values: targets,
					@getCurrent: getCurrent,
					@windowLabel: "Targets",
					@popupIsNewValue: newValue => newValue != current,
					@setToNewValue: setTarget,
					@setNewIndex: setNewIndex
				)
			);

			gui.ColorBlock(color, field);

			gui.GetLastRect(lastRect =>
			{
				GuiHelper.PingField(lastRect, current, current != null, EventsHelper.MouseEvents.IsRMB_MouseDown(), MouseCursor.Link);
				GuiHelper.SelectField(lastRect, current, EventsHelper.MouseEvents.IsRMB_MouseDown() && EventsHelper.MouseEvents.IsDoubleClick());
			});
		}
		protected void SetMethodEntry(MethodEntry current, Action<MethodEntry> set, MethodInfo toValue)
		{
			var newEntry = new MethodEntry(toValue);
			newEntry.ReinitArgs(paramTypes);
			SetMethodEntry(current, set, newEntry);
		}
		protected void SetMethodEntry(MethodEntry current, Action<MethodEntry> set, MethodEntry toValue)
		{
			Log("New method has been set: " + toValue);
			undo.RecordSetVariable(current, set, toValue);
		}

		protected void DoMethodsField(MethodEntry mEntry, Action<MethodInfo> setInfo, Func<MethodInfo> getCurrentInfo, MethodInfo[] methods)
		{
			var methodsFullNames = getMethodsFullNames(methods);

			if (methodsFullNames.IsEmpty())
				methodsFullNames = new[] { NA };

			var mIndex = methods.IndexOf(m => (m != null && mEntry.Info != null) &&
											   m.AreMethodsEqualForDeclaringType(mEntry.Info)
										);
			if (mIndex == -1)
			{
				Log("MethodsField: method not found - setting index to 0");
				mIndex = 0;
				if (!methods.IsEmpty())
				{
					Log("Methods array is not empty - setting method entry info to the first method in the array which is: " + methods[0]);
					mEntry.Info = methods[0];
				}
				else
				{
					Log("Methods array is empty");
					if (mEntry.Info != null)
					{
						Log("Method entry had a value: " + mEntry.Info + ". Setting it to null");
						mEntry.Info = null;
					}
				}
			}

			DoWindowOrPopup(
				@currentIndex: mIndex,
				@names: methodsFullNames,
				@values: methods,
				getCurrent: getCurrentInfo,
				@windowLabel: "Methods",
				@popupIsNewValue: newValue => newValue != mEntry.Info,
				@setToNewValue: setInfo,
				@setNewIndex: delegate { }
			);
		}
		private void DoWindowOrPopup<T>(
			int currentIndex, string[] names, T[] values,
			Func<T> getCurrent, string windowLabel, Predicate<T> popupIsNewValue,
			Action<T> setToNewValue, Action<int> setNewIndex)
		{
			if (!values.IsNullOrEmpty() && (values.Length > Settings.sMaxValuesCountInPopup ||
				names.Max(name => name.Length) > Settings.sMaxValueLengthInPopup))
			{
				DoWindow(currentIndex, names, values, getCurrent, windowLabel, setToNewValue);
			}
			else
			{
				DoPopup(currentIndex, names, values, popupIsNewValue, setToNewValue, setNewIndex);
			}
		}
		private void DoPopup<T>(int currentIndex, string[] names, T[] values,
			Predicate<T> isNewValue, Action<T> setToNewValue, Action<int> setNewIndex)
		{
			gui.Popup(currentIndex, names, newIndex =>
			{
				if (values.IsNullOrEmpty()) return;
				var newValue = values[newIndex];
				if (isNewValue(newValue))
				{
					Log("New popup value: " + newValue + " at index: " + newIndex);
					setNewIndex(newIndex);
					setToNewValue(newValue);
				}
			});
		}
		private void DoWindow<T>(int currentIndex, string[] names, T[] values, Func<T> getTarget, string label,
			Action<T> setToNewValue)
		{
			var valuesDic = RTHelper.CreateDictionary(values, names);
			gui.Button(names[currentIndex], "", EditorStyles.popup, null, () =>
				SelectionWindow.Show<T>(
					() => values,
					getTarget,
					newValue =>
					{
						setToNewValue(newValue);
						if (!Settings.sKeepSelectionWindowActiveAfterAdd)
							EditorHelper.FocusOnWindow("Inspector");
					},
					value => valuesDic[value],
					label
				)
			);
		}
		protected void DrawField(
			Color? color, int index, Action field,
			Action moveDown, Action moveUp,
			bool showFoldout, bool enableFoldout, Action foldout,
			bool enableRemove, string whatToRemove, Action remove)
		{
			gui.HorizontalBlock(() =>
			{
				// Numeric label
				gui.NumericLabel(index + 1);

				// Field
				gui.ColorBlock(color, field);

				// Foldout
				if (showFoldout)
					gui.EnabledBlock(enableFoldout, foldout);
				else if (delegateObject is KickassDelegate)
					gui.Space(17f);

				// Up/Down
				MoveUpDown(moveDown, moveUp);

				// Remove
				gui.EnabledBlock(enableRemove, () =>
					gui.RemoveButton(whatToRemove, MiniButtonStyle.ModRight, remove));
			});
		}

		#endregion
		#endregion

		// Prints
		#region
		public void PrintGOs()
		{
			if (goEntries != null)
				foreach (var entry in goEntries)
					if (entry.GO == null) Debug.Log("Null GO");
					else Debug.Log("Go: " + entry.GO.name);
		}
		public void PrintTargets()
		{
			if (goEntries != null)
			{
				foreach (var gEntry in goEntries)
					foreach (var tEntry in gEntry.TargetEntries)
						Debug.Log("Target: " + (tEntry.Target == null ? "Null" : tEntry.Target.GetType().Name));
			}
			else
			{
				var del = delegateObject.GetValue<Delegate>("_delegate");
				if (del != null)
				{
					var targets = del.GetInvocationList().Select(d => d.Target);
					foreach (var t in targets)
						Debug.Log("Target: " + (t == null ? "Null" : t.GetType().Name));
				}
			}
		}
		public void PrintMethods()
		{
			if (goEntries != null)
			{
				foreach (var gEntry in goEntries)
					foreach (var tEntry in gEntry.TargetEntries)
						foreach (var mEntry in tEntry.MethodEntries)
							Debug.Log("Method: " + mEntry.Info.GetFullName());
			}
			else
			{
				var del = delegateObject.GetValue<Delegate>("_delegate");
				if (del != null)
				{
					var methods = del.GetInvocationList().Select(d => d.Method);
					foreach (var m in methods)
						Debug.Log("Method: " + m.GetFullName());
				}
			}
		}
		#endregion
	}
}