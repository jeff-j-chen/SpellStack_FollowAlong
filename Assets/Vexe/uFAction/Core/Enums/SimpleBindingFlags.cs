using System;

namespace uFAction
{
	[Flags]
	public enum SimpleBindingFlags
	{
		DeclaredOnly = 2,
		Public = 16,
		NonPublic = 32,
	}
}