using UnityEngine;

namespace EditorGUIFramework
{
	public class GUISplitter : GUIControl
	{
		public override void Draw()
		{
			GUI.Box(Rect, "");
			base.Draw();
		}
	}
}