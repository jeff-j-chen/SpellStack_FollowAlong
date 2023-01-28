using System.Diagnostics;
using System.Linq;
using System;
using Vexe.Runtime.Extensions;

namespace EditorGUIFramework
{
	public class HorizontalGUIControlBlock : GUIControlBlock
	{
		public override float? Height
		{
			set { throw new InvalidOperationException("Can't set horizontal block height"); }
			get
			{
				if (entries.IsEmpty()) return 0;
				height = Controls.Select(c => c.Height.Value).Max() + Style.margin.vertical;
				return base.Height;
			}
		}

		public override void Draw()
		{
			//var watch = Stopwatch.StartNew();

			int totalControls = entries.Count;

			if (totalControls == 0)
				return;

			var margin = Style.margin;

			float totalSpace = 0;
			for (int i = 0; i < totalControls - 1; i++)
			{
				totalSpace += entries[i].control.HorizontalOffset;
			}

			var controls = Controls;
			var defWidthcontrols = controls.Where(c => c.Width.HasValue).ToArray();
			var nonDefWidthcontrols = controls.Except(defWidthcontrols);

			float totalDefinedWidth = 0;
			for (int i = 0; i < defWidthcontrols.Length; i++)
			{
				totalDefinedWidth += defWidthcontrols[i].Width.Value;
			}

			var flexibles = controls.Where(c => c is GUIFlexibleSpace).ToArray();
			int nFlexibles = flexibles.Length;
			if (nFlexibles > 0)
			{
				nonDefWidthcontrols = nonDefWidthcontrols.Except(flexibles);

				float totalWidthTaken = 0;
				foreach (var c in nonDefWidthcontrols)
				{
					float w = c.Style.CalcSize(c.Content).x;
					c.Width = w;
					totalWidthTaken += w;
				}
				float leftoverSpace = width.Value - totalSpace - margin.horizontal - totalWidthTaken - totalDefinedWidth;
				float flexibleSpace = leftoverSpace / nFlexibles;
				for (int i = 0; i < flexibles.Length; i++)
				{
					flexibles[i].Width = flexibleSpace;
				}
			}
			else
			{
				float standardWidth = (width.Value - totalDefinedWidth - totalSpace - margin.horizontal) /
									  (totalControls - defWidthcontrols.Length);
				foreach (var c in nonDefWidthcontrols)
				{
					c.Width = standardWidth;
				}
			}

			DrawGroupBox();
			float x = start.x + margin.left;
			float y = start.y + margin.top;
			for (int i = 0; i < totalControls; i++)
			{
				var c = controls[i];
				DrawControl(c, x, y);
				x += (c.Width.Value);
				x += c.HorizontalOffset;
			}

			base.Draw();

			//UnityEngine.Debug.Log("Hor: " + watch.ElapsedMilliseconds);
		}

		public override EmptyControl CreateSpace(float pixels)
		{
			return new EmptyControl { Width = pixels, Height = 0 };
		}
	}
}