using System;

namespace uFAction
{
	public interface ITypedDelegate
	{
		/// <summary>
		/// Returns true if the delegate doesn't take any parameters to invoke
		/// </summary>
		bool IsParameterlessDelegate { get; }

		/// <summary>
		/// Returns an array of types for the parameters the delegate takes to invoke - null for no (void) params
		/// </summary>
		Type[] ParamTypes { get; }

		/// <summary>
		/// Returns the return type of the delegate
		/// </summary>
		Type ReturnType { get; }
	}
}