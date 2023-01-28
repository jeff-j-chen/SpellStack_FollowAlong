using System;

/// <summary>
/// Credits: https://gist.github.com/michaelbartnett/5652076 
/// </summary>
public class Tuple<T1, T2>
{
	private readonly T1 first;
	private readonly T2 second;

	/// <summary>
	/// Retyurns the first element of the tuple
	/// </summary>
	public T1 First
	{
		get { return first; }
	}

	/// <summary>
	/// Returns the second element of the tuple
	/// </summary>
	public T2 Second
	{
		get { return second; }
	}

	/// <summary>
	/// Create a new tuple value
	/// </summary>
	/// <param name="item1">First element of the tuple</param>
	/// <param name="second">Second element of the tuple</param>
	public Tuple(T1 item1, T2 item2)
	{
		this.first = item1;
		this.second = item2;
	}

	public override string ToString()
	{
		return string.Format("Tuple({0}, {1})", First, Second);
	}

	public override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 23 + first.GetHashCode();
		hash = hash * 23 + second.GetHashCode();
		return hash;
	}

	public override bool Equals(object o)
	{
		var other = o as Tuple<T1, T2>;
		return other == null ? false : first.Equals(other.first) && second.Equals(other.second);
	}

	public static bool operator ==(Tuple<T1, T2> a, Tuple<T1, T2> b)
	{
		Func<object, object, bool> equals = object.ReferenceEquals;
		if (equals(a, null) && equals(b, null)) return true;
		if (equals(a, null) || equals(b, null)) return false;
		return a.Equals(b);
	}

	public static bool operator !=(Tuple<T1, T2> a, Tuple<T1, T2> b)
	{
		return !(a == b);
	}
}