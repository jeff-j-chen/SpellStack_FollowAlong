using System;
using UnityEditor;
using UnityEngine;
using EditorGUIFramework;
using ShowEmAll.DrawMates;

namespace ShowEmAll.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(EnumMaskAttribute))]
	public class EnumMaskAttributeDrawer : BetterPropertyDrawer<EnumMaskAttribute>
	{
		private EnumMaskDrawer<GUIWrapper, GUIOption> enumDrawer;

		protected override void Init(SerializedProperty property, GUIContent label)
		{
			base.Init(property, label);
			enumDrawer = new EnumMaskDrawer<GUIWrapper, GUIOption>(gui)
			{
				Text = NiceFieldName,
				GetValue = () => fieldInfo.GetValue(target) as Enum,
				SetValue = newValue => fieldInfo.SetValue(target, newValue)
			};
		}

		protected override void Code()
		{
			enumDrawer.Draw();
		}
	}
}