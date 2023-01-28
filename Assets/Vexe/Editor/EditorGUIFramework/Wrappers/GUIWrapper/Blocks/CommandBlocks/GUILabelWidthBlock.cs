namespace EditorGUIFramework
{
	public class GUILabelWidthBlock : CommandBlock
	{
		public float LabelWidth { get; set; }
		public override void Execute()
		{
			foreach (var e in entries) {
				e.control.LabelWidth = LabelWidth;
				LastBlock.AddEntry(e);
			}
		}
	}
}