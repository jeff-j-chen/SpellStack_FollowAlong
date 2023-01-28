namespace EditorGUIFramework
{
	public class GUIEnabledBlock : CommandBlock
	{
		public bool State { get; set; }
		public override void Execute()
		{
			foreach (var e in entries) {
				e.control.State &= State;
				LastBlock.AddEntry(e);
			}
		}
	}
}