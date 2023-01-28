using System;
using System.Linq;

namespace EditorGUIFramework
{
	public class GUIChangedBlock : CommandBlock
	{
		public Action OnChangeHandler { get; set; }
		public override void Execute()
		{
			var changables = entries
				.Select(e => new { Entry = e, Control = e.control as IChangableGUIControl })
				.Where(x => x.Control != null);

			foreach (var c in changables) {
				c.Control.OnChange += OnChangeHandler;
				LastBlock.AddEntry(c.Entry);
			}
		}
	}
}