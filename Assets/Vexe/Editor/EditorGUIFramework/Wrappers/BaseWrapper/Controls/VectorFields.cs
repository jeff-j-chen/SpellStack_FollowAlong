using UnityEngine;
using System;
using TOption = EditorGUIFramework.GUIOption;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		public void Vector3Field(Vector3 value, Action<Vector3> setValue)
		{
			Vector3Field("", value, setValue);
		}
		public void Vector3Field(string label, Vector3 value, Action<Vector3> setValue)
		{
			Vector3Field(label, value, null, setValue);
		}
		public void Vector3Field(string label, string tooltip, Vector3 value, Action<Vector3> setValue)
		{
			Vector3Field(label, tooltip, value, null, setValue);
		}
		public void Vector3Field(string label, Vector3 value, TOption option, Action<Vector3> setValue)
		{
			Vector3Field(label, "", value, option, setValue);
		}
		public void Vector3Field(string label, string tooltip, Vector3 value, TOption option, Action<Vector3> setValue)
		{
			Vector3Field(new GUIContent(label, tooltip), value, option, setValue);
		}
		public abstract void Vector3Field(GUIContent content, Vector3 value, TOption option, Action<Vector3> setValue);

		public void Vector2Field(Vector2 value, Action<Vector2> setValue)
		{
			Vector2Field("", value, setValue);
		}
		public void Vector2Field(string label, Vector2 value, Action<Vector2> setValue)
		{
			Vector2Field(label, value, null, setValue);
		}
		public void Vector2Field(string label, string tooltip, Vector2 value, Action<Vector2> setValue)
		{
			Vector2Field(label, tooltip, value, null, setValue);
		}
		public void Vector2Field(string label, Vector2 value, TOption option, Action<Vector2> setValue)
		{
			Vector2Field(label, "", value, option, setValue);
		}
		public void Vector2Field(string label, string tooltip, Vector2 value, TOption option, Action<Vector2> setValue)
		{
			Vector2Field(new GUIContent(label, tooltip), value, option, setValue);
		}
		public abstract void Vector2Field(GUIContent content, Vector2 value, TOption option, Action<Vector2> setValue);
	}
}