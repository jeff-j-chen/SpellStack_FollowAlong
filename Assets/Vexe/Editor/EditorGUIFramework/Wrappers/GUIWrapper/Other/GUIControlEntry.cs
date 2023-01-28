namespace EditorGUIFramework
{
	public class GUIControlEntry
	{
		public readonly GUIControl control;
		public readonly GUIOption option;

		public GUIControlEntry(GUIControl control, GUIOption option)
		{
			if (option == null) option = new GUIOption();

			this.control = control;
			this.option = option;

			if (!control.Width.HasValue && option.Width.HasValue)
				control.Width = option.Width.Value;
			try {
				if (!control.Height.HasValue)
					control.Height = option.Height.HasValue ? option.Height.Value : GUIWrapper.DEFAULT_HEIGHT;
			}
			catch { }
		}
	}
}