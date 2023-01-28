using System.Linq;
using Vexe.Editor.Extensions;
using Vexe.Runtime.Extensions;
using UnityEditor;
using UnityEngine;

namespace Assets.Vexe.uFAction.Scripts.Examples.Editor
{
	[CustomEditor(typeof(ComponentRemover))]
	public class ComponentRemoverEditor : ComponentModifierEditor
	{
		protected override string[] GetValues()
		{
			return gameObject.GetComponentsNames().Where(n => n != "Transform").ToArray();
		}

		protected override GUIContent Content
		{
			get { return new GUIContent("Remove", "Remove selected component from target"); }
		}

		protected override bool IsComponentNameValid
		{
			get { return base.IsComponentNameValid && spTarget.gameObject().GetComponent(spComponent.stringValue) != null; }
		}

		public override string DelegateName
		{
			get { return "onRemove"; }
		}
	}
}