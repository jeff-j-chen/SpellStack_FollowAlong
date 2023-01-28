public interface IExtraDelegateOps<TDelegate>
{
	/// <summary>
	/// Sets the delegate to the specified value
	/// </summary>
	void Set(TDelegate value);

	/// <summary>
	/// An internal delegate getter method
	/// </summary>
	TDelegate GetDelegate();
}