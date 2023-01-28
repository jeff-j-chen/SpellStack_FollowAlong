using System.Linq;
using Vexe.Editor.Extensions;
using Vexe.Runtime.Extensions;
using UnityEditor;
using UnityEngine;

namespace Assets.Vexe.uFAction.Scripts.Examples.Editor
{
	[CustomEditor(typeof(ComponentStateChanger))]
	public class ComponentStateChangerEditor : ComponentModifierEditor
	{
		protected override string[] GetValues()
		{
			return gameObject.GetComponentsNames().Where(n => n != "Transform").ToArray();
		}

		protected override GUIContent Content
		{
			get { return new GUIContent("Change state", "Change the selected component's state to the chose state"); }
		}

		protected override bool IsComponentNameValid
		{
			get { return base.IsComponentNameValid && spTarget.gameObject().GetComponent(spComponent.stringValue) != null; }
		}

		public override bool CanSetArgs
		{
			get { return false; }
		}

		public override string DelegateName
		{
			get { return "onStateChanged"; }
		}
	}
}