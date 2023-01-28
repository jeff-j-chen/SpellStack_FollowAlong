using UnityEngine;
using System;

namespace EditorGUIFramework
{
	public abstract class GUIControl : IGUIControl
	{
		private Action<Rect> onDrawn;
		protected float? width, height;
		protected bool state;
		protected Color color;
		protected float labelWidth;

		public Action<Rect> OnDrawn { get { return onDrawn; } set { onDrawn = value; } }
		public float x { get; set; }
		public float y { get; set; }
		public virtual float? Height { get { return height; } set { height = value; } }
		public virtual float? Width { get { return width; } set { width = value; } }
		public Vector2 Position { get { return new Vector2(x, y); } set { x = value.x; y = value.y; } }
		public Rect Rect { get { return new Rect(x, y, Width.Value, Height.Value); } }
		public GUIContent Content { get; set; }
		public GUIStyle Style { get; set; }
		public virtual bool State { get { return state; } set { state = value; } }
		public virtual Color Color { get { return color; } set { color = value; } }
		public virtual float LabelWidth { get { return labelWidth; } set { labelWidth = value; } }
		public virtual float VerticalOffset { get { return GUIWrapper.DefaultVerticalOffset; } }
		public virtual float HorizontalOffset { get { return GUIWrapper.DefaultHorizontalOffset; } }

		public GUIControl()
		{
			Style = GUIStyle.none;
			Content = GUIContent.none;
			color = Color.white;
			state = true;
			onDrawn = delegate { };
		}
		public virtual void Draw()
		{
			OnDrawn(Rect);
		}
		public virtual void Draw(float x, float y)
		{
			Position = new Vector2(x, y);
			Draw();
		}
	}
}