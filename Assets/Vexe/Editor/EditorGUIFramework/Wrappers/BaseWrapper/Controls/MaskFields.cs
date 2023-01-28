using UnityEngine;
using UnityEditor;
using System;

namespace EditorGUIFramework
{
	public partial class BaseWrapper<TOption> where TOption : LayoutOption, new()
	{
		/// <summary>
		/// Credits to Bunny83: http://answers.unity3d.com/questions/393992/custom-inspector-multi-select-enum-dropdown.html?sort=oldest
		/// </summary>
		public void EnumMaskFieldThatWorks(int currentValue, int[] enumValues, string[] enumNames, GUIContent content, Action<int> setMask)
		{
			int maskVal = 0;
			for (int i = 0; i < enumValues.Length; i++)
			{
				Action or = () => maskVal |= 1 << i;
				if (enumValues[i] != 0)
				{
					if ((currentValue & enumValues[i]) == enumValues[i])
						or();
				}
				else if (currentValue == 0)
					or();
			}

			MaskField(content, maskVal, enumNames, newMaskVal =>
			{
				int changes = maskVal ^ newMaskVal;

				for (int i = 0; i < enumValues.Length; i++)
				{
					if ((changes & (1 << i)) != 0) // has this list item changed?
					{
						if ((newMaskVal & (1 << i)) != 0) // has it been set?
						{
							if (enumValues[i] == 0) // special case: if "0" is set, just set the val to 0
							{
								currentValue = 0;
								break;
							}
							else
							{
								currentValue |= enumValues[i];
							}
						}
						else
						{ // it has been reset
							currentValue &= ~enumValues[i];
						}
					}
				}
				setMask(currentValue);
			});
		}

		public void EnumMaskFieldThatWorks(int currentValue, int[] enumValues, string[] enumNames, string text, Action<int> setMask)
		{
			EnumMaskFieldThatWorks(currentValue, enumValues, enumNames, new GUIContent(text), setMask);
		}

		public void EnumMaskFieldThatWorks(Enum enumValue, GUIContent label, Action<int> setMask)
		{
			var enumType = enumValue.GetType();
			var enumNames = Enum.GetNames(enumType);
			var enumValues = Enum.GetValues(enumType) as int[];
			EnumMaskFieldThatWorks(Convert.ToInt32(enumValue), enumValues, enumNames, label, setMask);
		}

		public void EnumMaskFieldThatWorks(Enum enumValue, string label, Action<int> setMask)
		{
			EnumMaskFieldThatWorks(enumValue, new GUIContent(label), setMask);
		}

		public void MaskField(int mask, string[] displayedOptions, TOption option, Action<int> setMask)
		{
			MaskField("", mask, displayedOptions, option, setMask);
		}
		public void MaskField(GUIContent label, int mask, string[] displayedOptions, Action<int> setMask)
		{
			MaskField(label, mask, displayedOptions, (TOption)null, setMask);
		}
		public void MaskField(GUIContent label, int mask, string[] displayedOptions, TOption option, Action<int> setMask)
		{
			MaskField(label, mask, displayedOptions, EditorStyles.popup, option, setMask);
		}
		public void MaskField(int mask, string[] displayedOptions, GUIStyle style, TOption option, Action<int> setMask)
		{
			MaskField("", mask, displayedOptions, style, option, setMask);
		}
		public void MaskField(string label, int mask, string[] displayedOptions, TOption option, Action<int> setMask)
		{
			MaskField(label, mask, displayedOptions, EditorStyles.popup, option, setMask);
		}
		public void MaskField(string label, int mask, string[] displayedOptions, GUIStyle style, TOption option, Action<int> setMask)
		{
			MaskField(new GUIContent(label), mask, displayedOptions, style, option, setMask);
		}
		public abstract void MaskField(GUIContent label, int mask, string[] displayedOptions, GUIStyle style, TOption option, Action<int> setMask);
	}
}