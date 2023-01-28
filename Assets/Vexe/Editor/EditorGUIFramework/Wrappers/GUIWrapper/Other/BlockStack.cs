using System.Collections.Generic;
using System.Linq;
using Vexe.Runtime.Extensions;

namespace EditorGUIFramework
{
	public class BlockStack
	{
		private List<IBlock> blocks = new List<IBlock>();

		public IBlock LastBlock { get { return blocks.IsEmpty() ? null : blocks.First(); } }
		public GUIControlBlock LastGUIBlock { get { return blocks.IsEmpty() ? null : blocks.FirstOrDefault(b => b is GUIControlBlock) as GUIControlBlock; } }
		public int Length { get { return blocks.Count; } }

		public void Push(IBlock b)
		{
			blocks.Insert(0, b);
		}
		public IBlock Peek()
		{
			return blocks.First();
		}
		public void Pop()
		{
			blocks.RemoveAt(0);
		}
	}
}