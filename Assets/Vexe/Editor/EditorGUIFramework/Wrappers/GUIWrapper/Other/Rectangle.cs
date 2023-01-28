using UnityEngine;

namespace EditorGUIFramework
{
	public class Rectangle
	{
		public Rect value;

		public Rectangle(float x, float y, float width, float height) : this(new Rect(x, y, width, height)) { }

		public Rectangle(Rect r)
		{
			value = r;
		}

		public static implicit operator Rect(Rectangle r)
		{
			return r.value;
		}

		public float x { get { return value.x; } set { this.value.x = value; } }
		public float y { get { return value.y; } set { this.value.y = value; } }
		public float width { get { return value.width; } set { this.value.width = value; } }
		public float height { get { return value.height; } set { this.value.height = value; } }
		public float xMax { get { return value.xMax; } set { this.value.xMax = value; } }
		public float yMax { get { return value.yMax; } set { this.value.yMax = value; } }
	}
}