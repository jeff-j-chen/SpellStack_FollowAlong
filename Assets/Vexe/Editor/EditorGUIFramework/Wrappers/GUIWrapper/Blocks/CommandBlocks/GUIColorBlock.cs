using UnityEngine;

namespace EditorGUIFramework
{
	public class GUIColorBlock : CommandBlock
	{
		public Color Color { get; set; }
		public override void Execute()
		{
			foreach (var e in entries) {
				e.control.Color = Color;
				LastBlock.AddEntry(e);
			}
		}
	}
}