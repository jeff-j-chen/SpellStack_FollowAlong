using UnityEngine;
using UnityEditor;

namespace EditorGUIFramework
{
	public class GUIHelpBox : GUIControl
	{
		public string Message { get; set; }
		public MessageType Type { get; set; }
		public override void Draw()
		{
			EditorGUI.HelpBox(Rect, Message, Type);
			base.Draw();
		}
	}
}