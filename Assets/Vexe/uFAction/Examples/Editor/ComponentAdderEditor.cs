using System.Linq;
using Vexe.Runtime.Helpers;
using UnityEditor;
using UnityEngine;

namespace Assets.Vexe.uFAction.Scripts.Examples.Editor
{
	[CustomEditor(typeof(ComponentAdder))]
	public class ComponentAdderEditor : ComponentModifierEditor
	{
		protected override GUIContent Content
		{
			get { return new GUIContent("Add", "Add selected component"); }
		}

		protected override string[] GetValues()
		{
			return ReflectionHelper.GetAllTypesOf(typeof(Component)).Select(t => t.Name).ToArray();
		}

		public override string DelegateName
		{
			get { return "onAdd"; }
		}
	}
}