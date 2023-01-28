using System;
using UnityEngine;

namespace ShowEmAll
{
	/// <summary>
	/// Markup an enum field with this attribute to take into consideration any custom values set for the enums for correct bit-wise operation
	/// Credits to Bunny82: http://answers.unity3d.com/questions/393992/custom-inspector-multi-select-enum-dropdown.html?sort=oldest
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class EnumMaskAttribute : PropertyAttribute
	{
	}
}