using EditorGUIFramework;
using System;

namespace ShowEmAll.DrawMates
{
	public class EnumMaskDrawer<TWrapper, TOption> : BaseDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		public string Text { get; set; }
		public Action<Enum> SetValue { get; set; }
		public Func<Enum> GetValue { get; set; }

		public EnumMaskDrawer(TWrapper gui)
			: base(gui, null)
		{ }

		public override void Draw()
		{
			var currentValue = GetValue();
			gui.EnumMaskFieldThatWorks(currentValue, Text, newMask =>
			{
				var newValue = Enum.ToObject(currentValue.GetType(), newMask) as Enum;
				if (!Equals(newValue, currentValue))
				{
					undo.RecordSetVariable(() => currentValue, e => SetValue(e), newValue);
				}
			});
		}
	}
}