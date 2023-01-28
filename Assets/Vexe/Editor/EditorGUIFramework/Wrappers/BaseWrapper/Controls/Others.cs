namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public abstract void Splitter(float thickness);
		public void Splitter()
		{
			Splitter(1f);
		}
	}
}