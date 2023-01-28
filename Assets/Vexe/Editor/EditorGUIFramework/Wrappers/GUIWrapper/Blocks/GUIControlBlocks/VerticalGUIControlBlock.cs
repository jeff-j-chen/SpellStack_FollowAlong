using System;
using Vexe.Runtime.Extensions;

namespace EditorGUIFramework
{
	public class VerticalGUIControlBlock : GUIControlBlock
	{
		public override float? Height
		{
			set { throw new InvalidOperationException("Can't set vertical block height"); }
			get
			{
				if (entries.IsEmpty()) return 0;
				float total = 0;
				ControlsLoop(c => total += c.Height.Value);
				total += (Controls.Length - 2) * GUIWrapper.DefaultVerticalOffset + Style.margin.vertical;
				height = total;
				return base.Height;
			}
		}

		public override void Draw()
		{
			int nEntries = entries.Count;
			if (nEntries == 0) return;

			DrawGroupBox();
			var margin = Style.margin;
			float x = start.x + margin.left;
			float y = start.y + margin.top;

			for (int i = 0; i < nEntries; i++)
			{
				var c = entries[i].control;

				if (!c.Width.HasValue)
				{
					c.Width = width.HasValue ? width.Value : start.width;
					c.Width -= margin.horizontal;
				}

				DrawControl(c, x, y);
				y += c.Height.Value + c.VerticalOffset;
				start.y = y;
			}
		}

		public override EmptyControl CreateSpace(float pixels)
		{
			return new EmptyControl { Height = pixels, Width = 0 };
		}
	}
}