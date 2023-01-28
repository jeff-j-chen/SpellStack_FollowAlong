using System;
namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void Indent(float indentLevel)
		{
			Space(indentLevel * INDENT_AMOUNT);
		}
		public void Indent()
		{
			Indent(1f);
		}

		public abstract void Space(float pixels);
		public abstract void FlexibleSpace();
		public void FlexibleSandwich(Action block)
		{
			FlexibleSpace();
			block();
			FlexibleSpace();
		}
	}
}