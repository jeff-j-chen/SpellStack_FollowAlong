using System;
using UnityEngine;

namespace EditorGUIFramework
{
	public interface IGUIControl
	{
		Vector2 Position { get; set; }
		float? Width { get; set; }
		float? Height { get; set; }
		Rect Rect { get; }
		GUIContent Content { get; set; }
		GUIStyle Style { get; set; }
		bool State { get; set; }
		Color Color { get; set; }
		void Draw();
		Action<Rect> OnDrawn { get; set; }
	}
}