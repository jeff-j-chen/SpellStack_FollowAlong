using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using EditorGUIFramework.Helpers;
using System;

namespace EditorGUIFramework
{
	public abstract class GUIControlBlock : GUIControl, IPositionableBlock, IChangableGUIControl
	{
		private Action onChange;
		public Action OnChange
		{
			get { return onChange; }
			set
			{
				var changables = (from e in entries
								  let c = e.control as IChangableGUIControl
								  where c != null
								  select c);

				foreach (var c in changables)
					c.OnChange += value;

				onChange = value;
			}
		}

		protected List<GUIControlEntry> entries = new List<GUIControlEntry>();
		protected Rectangle start;

		protected GUIControl[] Controls { get { return entries.Select(e => e.control).ToArray(); } }
		public List<GUIControlEntry> Entries { get { return entries; } }
		public Rectangle Start { get { return start; } set { start = value; } }
		public GUIControl LastControl { get { return Controls.Last(); } }
		public override bool State
		{
			get { return base.State; }
			set
			{
				ControlsLoop(c => c.State = value);
				base.State = value;
			}
		}
		public override Color Color
		{
			get { return base.Color; }
			set
			{
				ControlsLoop(c => c.Color = value);
				base.Color = value;
			}
		}
		public override float LabelWidth
		{
			get { return base.LabelWidth; }
			set
			{
				ControlsLoop(c => c.LabelWidth = value);
				base.LabelWidth = value;
			}
		}

		protected void ControlsLoop(Action<GUIControl> code)
		{
			for (int i = 0; i < Controls.Length; i++)
			{
				code(Controls[i]);
			}
		}

		public void AddEntry(GUIControlEntry entry)
		{
			entries.Add(entry);
		}

		public void AddControl(GUIControl control, GUIOption option)
		{
			AddEntry(new GUIControlEntry(control, option));
		}

		protected Rect GetGroupRect()
		{
			var rect = new Rect(Start);
			rect.height = Height.Value;
			if (width.HasValue) rect.width = width.Value;
			return rect;
		}

		protected void DrawGroupBox()
		{
			GUI.Box(GetGroupRect(), "", Style);
		}

		public override void Draw(float x, float y)
		{
			start.x = x;
			start.y = y;
			base.Draw(x, y);
		}

		protected void DrawControl(GUIControl c, float x, float y)
		{
			Blocks.StateBlock(c.State, () =>
				Blocks.ColorBlock(c.Color, () =>
					Blocks.LabelWidthBlock(c.LabelWidth, () =>
						c.Draw(x, y)
					)
				)
			);
		}

		public abstract EmptyControl CreateSpace(float pixels);
	}
}