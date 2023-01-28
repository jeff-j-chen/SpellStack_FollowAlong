using Vexe.Editor.Extensions;
using EditorGUIFramework;
using Vexe.Editor.Helpers;
using Vexe.Runtime.Extensions;
using uFAction;
using UnityEditor;
using UnityEngine;

namespace Assets.Vexe.uFAction.Scripts.Examples.Editor
{
	public abstract class ComponentModifierEditor : BetterEditor<ComponentModifier>
	{
		protected SerializedProperty spTarget;
		protected SerializedProperty spComponent;
		protected DelegateDrawer<GLWrapper, GLOption> delegateDrawer;
		protected GameObject gameObject { get { return spTarget.gameObject(); } }

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			serializedObject.Update();

			spTarget = serializedObject.FindProperty("target");
			spComponent = serializedObject.FindProperty("component");

			gui.ChangeBlock(
				() => spTarget.DrawProperty(),
				() => spComponent.stringValue = string.Empty
				);

			gui.EnabledBlock(spTarget.objectReferenceValue != null, () =>
			{
				gui.HorizontalBlock(() =>
				{
					EditorGUILayout.PrefixLabel("Component");

					gui.Label(IsComponentNameValid ? spComponent.stringValue : "Select component -> ", GUI.skin.textField);

					gui.SelectionButton("component", () =>
						SelectionWindow.Show(
							getValues: GetValues,
							getTarget: () => spComponent.stringValue,
							setTarget: c =>
							{
								spComponent.stringValue = c;
								serializedObject.ApplyModifiedProperties();
							},
							getValueName: c => c,
							label: "Components"));
				});

				gui.EnabledBlock(IsComponentNameValid, () =>
					gui.Button(Content, TypedTarget.Modify)
					);

				delegateDrawer.Draw();
			});
			serializedObject.ApplyModifiedProperties();
		}

		private void OnEnable()
		{
			delegateDrawer = new DelegateDrawer<GLWrapper, GLOption>(gui)
			{
				spDelegate = serializedObject.FindProperty(DelegateName),
				delegateObject = GetFieldValue(DelegateName),
				title = DelegateTitle,
				canSetArgsFromEditor = CanSetArgs
			};
		}

		protected abstract string[] GetValues();
		protected abstract GUIContent Content { get; }
		protected virtual bool IsComponentNameValid { get { return !string.IsNullOrEmpty(spComponent.stringValue); } }

		public virtual string DelegateTitle { get { return DelegateName.SplitPascalCase(); } }
		public virtual bool CanSetArgs { get { return true; } }
		public abstract string DelegateName { get; }
	}
}