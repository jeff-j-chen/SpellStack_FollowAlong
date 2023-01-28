using UnityEngine;
using System;
using Vexe.Runtime.Types.GUI;

namespace uFAction
{
	[Serializable]
	public class ColorTheme : ScriptableObject
	{
		[SerializeField]
		private ColorDuo gameObjectsColors;
		[SerializeField]
		private ColorDuo targetsColors;
		[SerializeField]
		private ColorDuo methodsColors;

		public ColorDuo GameObjectsColors { get { return gameObjectsColors; } }
		public ColorDuo TargetColors { get { return targetsColors; } }
		public ColorDuo MethodsColors { get { return methodsColors; } }

		public void Init()
		{
			gameObjectsColors = new ColorDuo("GameObjectsColors");
			targetsColors = new ColorDuo("TargetsColors");
			methodsColors = new ColorDuo("MethodsColors");
		}
	}
}