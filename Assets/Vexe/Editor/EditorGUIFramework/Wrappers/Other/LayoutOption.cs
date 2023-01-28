using UnityEngine;
using UnityEditor;

namespace EditorGUIFramework
{
	public abstract class LayoutOption
	{
		public float? Height { get; set; }
		public float? Width { get; set; }
		public bool? ExpandHeight { get; set; }
		public bool? ExpandWidth { get; set; }
		public float? MinHeight { get; set; }
		public float? MinWidth { get; set; }
	}
}