using System;
using System.Reflection;

namespace uFAction
{
	/// <summary>
	/// A serialized parameterless delegate (Action) that could target any System.Object that is _not_ a Unity Object
	/// It could hook methods of no return (void) and no parameters
	/// </summary>
	[Serializable]
	public class SysObjAction : SysObjDelegate<Action>, IInvokableWithNoReturn
	{
		/// <summary>
		/// The return type of the delegate - in this case, it's void
		/// </summary>
		public override Type ReturnType { get { return typeof(void); } }

		/// <summary>
		/// Returns the type of the params the delegate takes - since this delegate is parameterless, Type.EmptyTypes is returned
		/// </summary>
		public override Type[] ParamTypes { get { return Type.EmptyTypes; } }

		/// <summary>
		/// invokes (executes) the delegate - in this case with no params
		/// </summary>
		public void Invoke()
		{
			var del = GetDelegate();
			if (del != null)
				del();
		}

		/// <summary>
		/// Returns the Target of the specified handler - used internally to identify if the target is valid or not
		/// </summary>
		protected override object GetHandlerTarget(Action handler)
		{
			return handler.Target;
		}

		/// <summary>
		/// Returns the MethodInfo of the specified handler - used internally to identify if the target is valid or not
		/// </summary>
		protected override MethodInfo GetHandlerMethod(Action handler)
		{
			return handler.Method;
		}

		protected override Action InternalAdd(Action handler)
		{
			return GetDelegate() + handler;
		}

		protected override Action InternalRemove(Action handler)
		{
			return GetDelegate() - handler;
		}

		/// <summary>
		/// Overloads the '+' operator to make it possible to do 'myDel += myHandler;'
		/// </summary>
		public static SysObjAction operator +(SysObjAction sd, Action handler)
		{
			sd.Add(handler);
			return sd;
		}

		/// <summary>
		/// Overloads the '-' operator to make it possible to do 'myDel -= myHandler;'
		/// </summary>
		public static SysObjAction operator -(SysObjAction sd, Action handler)
		{
			sd.Remove(handler);
			return sd;
		}
	}

	/// <summary>
	/// A parameter version of SysObjAction that takes one argument of type TArg
	/// </summary>
	[Serializable]
	public class SysObjAction<TArg> : SysObjDelegate<Action<TArg>>, IInvokableWithNoReturn<TArg>
	{
		public override Type ReturnType { get { return typeof(void); } }

		public override Type[] ParamTypes { get { return new[] { typeof(TArg) }; } }

		public void Invoke(TArg arg)
		{
			var del = GetDelegate();
			if (del != null)
				del(arg);
		}

		protected override object GetHandlerTarget(Action<TArg> handler)
		{
			return handler.Target;
		}

		protected override MethodInfo GetHandlerMethod(Action<TArg> handler)
		{
			return handler.Method;
		}

		protected override Action<TArg> InternalAdd(Action<TArg> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Action<TArg> InternalRemove(Action<TArg> handler)
		{
			return GetDelegate() - handler;
		}

		public static SysObjAction<TArg> operator +(SysObjAction<TArg> sd, Action<TArg> handler)
		{
			sd.Add(handler);
			return sd;
		}

		public static SysObjAction<TArg> operator -(SysObjAction<TArg> sd, Action<TArg> handler)
		{
			sd.Remove(handler);
			return sd;
		}
	}

	/// <summary>
	/// A parameter version of SysObjAction that takes two arguments of types TArg1 and TArg2
	/// </summary>
	[Serializable]
	public class SysObjAction<TArg1, TArg2> : SysObjDelegate<Action<TArg1, TArg2>>, IInvokableWithNoReturn<TArg1, TArg2>
	{
		public override Type ReturnType { get { return typeof(void); } }

		public override Type[] ParamTypes { get { return new[] { typeof(TArg1), typeof(TArg2) }; } }

		public void Invoke(TArg1 arg1, TArg2 arg2)
		{
			var del = GetDelegate();
			if (del != null)
				del(arg1, arg2);
		}

		protected override object GetHandlerTarget(Action<TArg1, TArg2> handler)
		{
			return handler.Target;
		}

		protected override MethodInfo GetHandlerMethod(Action<TArg1, TArg2> handler)
		{
			return handler.Method;
		}

		protected override Action<TArg1, TArg2> InternalAdd(Action<TArg1, TArg2> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Action<TArg1, TArg2> InternalRemove(Action<TArg1, TArg2> handler)
		{
			return GetDelegate() - handler;
		}

		public static SysObjAction<TArg1, TArg2> operator +(SysObjAction<TArg1, TArg2> sd, Action<TArg1, TArg2> handler)
		{
			sd.Add(handler);
			return sd;
		}

		public static SysObjAction<TArg1, TArg2> operator -(SysObjAction<TArg1, TArg2> sd, Action<TArg1, TArg2> handler)
		{
			sd.Remove(handler);
			return sd;
		}
	}

	/// <summary>
	/// A parameter version of SysObjAction that takes three arguments of types TArg1, TArg2 and TArg3
	/// </summary>
	[Serializable]
	public class SysObjAction<TArg1, TArg2, TArg3> : SysObjDelegate<Action<TArg1, TArg2, TArg3>>, IInvokableWithNoReturn<TArg1, TArg2, TArg3>
	{
		public override Type ReturnType { get { return typeof(void); } }

		public override Type[] ParamTypes { get { return new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }; } }

		public void Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3)
		{
			var del = GetDelegate();
			if (del != null)
				del(arg1, arg2, arg3);
		}

		protected override object GetHandlerTarget(Action<TArg1, TArg2, TArg3> handler)
		{
			return handler.Target;
		}

		protected override MethodInfo GetHandlerMethod(Action<TArg1, TArg2, TArg3> handler)
		{
			return handler.Method;
		}

		protected override Action<TArg1, TArg2, TArg3> InternalAdd(Action<TArg1, TArg2, TArg3> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Action<TArg1, TArg2, TArg3> InternalRemove(Action<TArg1, TArg2, TArg3> handler)
		{
			return GetDelegate() - handler;
		}

		public static SysObjAction<TArg1, TArg2, TArg3> operator +(SysObjAction<TArg1, TArg2, TArg3> sd, Action<TArg1, TArg2, TArg3> handler)
		{
			sd.Add(handler);
			return sd;
		}

		public static SysObjAction<TArg1, TArg2, TArg3> operator -(SysObjAction<TArg1, TArg2, TArg3> sd, Action<TArg1, TArg2, TArg3> handler)
		{
			sd.Remove(handler);
			return sd;
		}
	}
}