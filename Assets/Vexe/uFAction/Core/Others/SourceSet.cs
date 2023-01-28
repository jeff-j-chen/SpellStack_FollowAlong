using UnityEngine;

namespace uFAction
{
	public struct SourceSet
	{
		public readonly Component source;
		public readonly string member;

		public SourceSet(Component source, string member)
		{
			this.source = source;
			this.member = member;
		}
	}
}