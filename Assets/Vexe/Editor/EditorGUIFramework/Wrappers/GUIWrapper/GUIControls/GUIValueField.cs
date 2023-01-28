using System;

namespace EditorGUIFramework
{
	public abstract class GUIValueField<T> : GUIControl, IChangableGUIControl
	{
		public Action OnChange { get; set; }
		public Action<T> SetValue { get; set; }
		public T Value { get; set; }
	}
}