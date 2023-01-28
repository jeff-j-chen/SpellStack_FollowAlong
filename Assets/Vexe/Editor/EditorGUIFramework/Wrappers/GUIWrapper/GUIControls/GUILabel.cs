using UnityEngine;

namespace EditorGUIFramework
{
	public class GUILabel : GUIControl
	{
		public override void Draw()
		{
			GUI.Label(Rect, Content, Style);
			base.Draw();
		}
	}
}