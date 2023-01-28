using UnityEngine;

namespace EditorGUIFramework
{
	public class GUIBox : GUIControl
	{
		public override void Draw()
		{
			GUI.Box(Rect, Content, Style);
			base.Draw();
		}
	}
}