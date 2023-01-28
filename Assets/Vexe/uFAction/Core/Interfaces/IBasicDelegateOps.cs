public interface IBasicDelegateOps<TDelegate>
{
	/// <summary>
	/// Removes (unsubscribes) a handler method from the delegate
	/// </summary>
	/// <param name="handler">The handler method to remove</param>
	void Remove(TDelegate handler);

	/// <summary>
	/// Adds (subscribes) a handler method to the delegate.
	/// The target (the object that the handler is applied on) has to be of type TTarget
	/// otherwise an InvalidOperationException is thrown
	/// </summary>
	/// <param name="handler">The handler method to add</param>
	void Add(TDelegate handler);

	/// <summary>
	/// Returns true if the specified handler method is contained in the delegate's invocation list
	/// </summary>
	bool Contains(TDelegate handler);

	/// <summary>
	/// Clears out the delegate from all subscribers/handlers
	/// </summary>
	void Clear();
}