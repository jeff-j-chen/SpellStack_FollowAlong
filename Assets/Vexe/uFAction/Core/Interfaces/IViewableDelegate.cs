namespace uFAction
{

#if UNITY_EDITOR
	public interface IViewableDelegate
	{
		EditorViewStyle CurrentViewStyle { get; }
		EditorViewStyle[] PossibleViewStyles { get; }
		bool HeaderToggle { get; set; }
		void CycleViewStyles();
		bool HasBeenModifiedFromCode { get; }
	}
#endif
}