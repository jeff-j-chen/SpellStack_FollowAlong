using UnityEngine;
using System;
using Option = EditorGUIFramework.GUIOption;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void BoundsField(Bounds value, Action<Bounds> setValue)
		{
			BoundsField("", value, setValue);
		}
		public void BoundsField(string label, Bounds value, Action<Bounds> setValue)
		{
			BoundsField(label, value, DefaultMultiFieldOption, setValue);
		}
		public void BoundsField(string label, string tooltip, Bounds value, Action<Bounds> setValue)
		{
			BoundsField(label, tooltip, value, DefaultMultiFieldOption, setValue);
		}
		public void BoundsField(string label, Bounds value, TOption option, Action<Bounds> setValue)
		{
			BoundsField(label, "", value, option, setValue);
		}
		public void BoundsField(string label, string tooltip, Bounds value, TOption option, Action<Bounds> setValue)
		{
			BoundsField(new GUIContent(label, tooltip), value, option, setValue);
		}
		public abstract void BoundsField(GUIContent content, Bounds value, TOption option, Action<Bounds> setValue);
	}
}