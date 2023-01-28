using System.Collections.Generic;

namespace EditorGUIFramework
{
	public interface IBlock
	{
		List<GUIControlEntry> Entries { get; }
		void AddControl(GUIControl control, GUIOption option);
		void AddEntry(GUIControlEntry e);
	}
}