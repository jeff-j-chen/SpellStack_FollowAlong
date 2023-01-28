using System.Collections.Generic;

namespace EditorGUIFramework
{
	public abstract class CommandBlock : IBlock
	{
		protected List<GUIControlEntry> entries = new List<GUIControlEntry>();
		public List<GUIControlEntry> Entries { get { return entries; } }
		public IBlock LastBlock { get; set; }

		public void AddControl(GUIControl control, GUIOption option)
		{
			entries.Add(new GUIControlEntry(control, option));
		}
		public void AddEntry(GUIControlEntry e)
		{
			entries.Add(e);
		}
		public abstract void Execute();
	}
}