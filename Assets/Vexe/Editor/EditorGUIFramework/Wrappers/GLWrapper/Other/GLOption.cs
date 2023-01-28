using UnityEngine;
using System.Collections.Generic;

namespace EditorGUIFramework
{
	public class GLOption : LayoutOption
	{
		public GUILayoutOption[] Value
		{
			get
			{
				var options = new List<GUILayoutOption>();
				if (Width.HasValue)
					options.Add(GUILayout.Width(Width.Value));
				if (Height.HasValue)
					options.Add(GUILayout.Height(Height.Value));
				if (MinHeight.HasValue)
					options.Add(GUILayout.MinHeight(MinHeight.Value));
				if (MinWidth.HasValue)
					options.Add(GUILayout.MinWidth(MinWidth.Value));
				if (ExpandHeight.HasValue)
					options.Add(GUILayout.ExpandHeight(ExpandHeight.Value));
				if (ExpandWidth.HasValue)
					options.Add(GUILayout.ExpandWidth(ExpandWidth.Value));
				return options.ToArray();
			}
		}

		public static implicit operator GUILayoutOption[](GLOption option)
		{
			return option == null ? null : option.Value;
		}
	}
}