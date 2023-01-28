using System;

namespace EditorGUIFramework
{
	public interface IChangableGUIControl
	{
		Action OnChange { get; set; }
	}
}