using System;
using System.Reflection;

namespace uFAction
{
	/// <summary>
	/// A serialized delegate (Func) that could target any object that is _not_ a UnityEngine.Object nor contain any
	/// It hooks up methods of the signature: TReturn MethodName();
	/// </summary>
	public class SysObjFunc<TReturn> : SysObjDelegate<Func<TReturn>>, IInvokableWithReturn<TReturn>
	{
		public override Type[] ParamTypes { get { return Type.EmptyTypes; } }

		public override Type ReturnType { get { return typeof(TReturn); } }

		/// <summary>
		/// Invokes the delegate - in this case with no params
		/// Throws a NullReferenceException if the delegate value was null (like in the case of the delegate being empty)
		/// </summary>
		/// <returns></returns>
		public TReturn Invoke()
		{
			var del = GetDelegate();
			if (del == null)
				DelegateOpsHelper.ThrowDelegateNull("SysObjFunc", "Can't invoke");
			return del();
		}

		protected override object GetHandlerTarget(Func<TReturn> handler)
		{
			return handler.Target;
		}

		protected override MethodInfo GetHandlerMethod(Func<TReturn> handler)
		{
			return handler.Method;
		}

		public static SysObjFunc<TReturn> operator +(SysObjFunc<TReturn> del, Func<TReturn> handler)
		{
			del.Add(handler);
			return del;
		}

		public static SysObjFunc<TReturn> operator -(SysObjFunc<TReturn> del, Func<TReturn> handler)
		{
			del.Remove(handler);
			return del;
		}

		protected override Func<TReturn> InternalAdd(Func<TReturn> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Func<TReturn> InternalRemove(Func<TReturn> handler)
		{
			return GetDelegate() - handler;
		}
	}

	public class SysObjFunc<TArg, TReturn> : SysObjDelegate<Func<TArg, TReturn>>, IInvokableWithReturn<TArg, TReturn>
	{
		public override Type[] ParamTypes { get { return new[] { typeof(TArg) }; } }

		public override Type ReturnType { get { return typeof(TReturn); } }

		/// <summary>
		/// Invokes the delegate with the specified argument
		/// Throws a NullReferenceException if the delegate value was null (like in the case of the delegate being empty)
		/// </summary>
		public TReturn Invoke(TArg arg)
		{
			var del = GetDelegate();
			if (del == null)
				DelegateOpsHelper.ThrowDelegateNull("SysObjFunc", "Can't invoke");
			return del(arg);
		}

		protected override object GetHandlerTarget(Func<TArg, TReturn> handler)
		{
			return handler.Target;
		}

		protected override MethodInfo GetHandlerMethod(Func<TArg, TReturn> handler)
		{
			return handler.Method;
		}

		public static SysObjFunc<TArg, TReturn> operator +(SysObjFunc<TArg, TReturn> del, Func<TArg, TReturn> handler)
		{
			del.Add(handler);
			return del;
		}

		public static SysObjFunc<TArg, TReturn> operator -(SysObjFunc<TArg, TReturn> del, Func<TArg, TReturn> handler)
		{
			del.Remove(handler);
			return del;
		}

		protected override Func<TArg, TReturn> InternalAdd(Func<TArg, TReturn> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Func<TArg, TReturn> InternalRemove(Func<TArg, TReturn> handler)
		{
			return GetDelegate() - handler;
		}
	}

	public class SysObjFunc<TArg1, TArg2, TReturn> : SysObjDelegate<Func<TArg1, TArg2, TReturn>>, IInvokableWithReturn<TArg1, TArg2, TReturn>
	{
		public override Type[] ParamTypes { get { return new[] { typeof(TArg1), typeof(TArg2) }; } }

		public override Type ReturnType { get { return typeof(TReturn); } }

		/// <summary>
		/// Invokes the delegate with the specified arguments
		/// Throws a NullReferenceException if the delegate value was null (like in the case of the delegate being empty)
		/// </summary>
		public TReturn Invoke(TArg1 arg1, TArg2 arg2)
		{
			var del = GetDelegate();
			if (del == null)
				DelegateOpsHelper.ThrowDelegateNull("SysObjFunc", "Can't invoke");
			return del(arg1, arg2);
		}

		protected override object GetHandlerTarget(Func<TArg1, TArg2, TReturn> handler)
		{
			return handler.Target;
		}

		protected override MethodInfo GetHandlerMethod(Func<TArg1, TArg2, TReturn> handler)
		{
			return handler.Method;
		}

		public static SysObjFunc<TArg1, TArg2, TReturn> operator +(SysObjFunc<TArg1, TArg2, TReturn> del, Func<TArg1, TArg2, TReturn> handler)
		{
			del.Add(handler);
			return del;
		}

		public static SysObjFunc<TArg1, TArg2, TReturn> operator -(SysObjFunc<TArg1, TArg2, TReturn> del, Func<TArg1, TArg2, TReturn> handler)
		{
			del.Remove(handler);
			return del;
		}

		protected override Func<TArg1, TArg2, TReturn> InternalAdd(Func<TArg1, TArg2, TReturn> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Func<TArg1, TArg2, TReturn> InternalRemove(Func<TArg1, TArg2, TReturn> handler)
		{
			return GetDelegate() - handler;
		}
	}

	public class SysObjFunc<TArg1, TArg2, TArg3, TReturn> : SysObjDelegate<Func<TArg1, TArg2, TArg3, TReturn>>, IInvokableWithReturn<TArg1, TArg2, TArg3, TReturn>
	{
		public override Type[] ParamTypes { get { return new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }; } }

		public override Type ReturnType { get { return typeof(TReturn); } }

		/// <summary>
		/// Invokes the delegate with the specified arguments
		/// Throws a NullReferenceException if the delegate value was null (like in the case of the delegate being empty)
		/// </summary>
		public TReturn Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3)
		{
			var del = GetDelegate();
			if (del == null)
				DelegateOpsHelper.ThrowDelegateNull("SysObjFunc", "Can't invoke");
			return del(arg1, arg2, arg3);
		}

		protected override object GetHandlerTarget(Func<TArg1, TArg2, TArg3, TReturn> handler)
		{
			return handler.Target;
		}

		protected override MethodInfo GetHandlerMethod(Func<TArg1, TArg2, TArg3, TReturn> handler)
		{
			return handler.Method;
		}

		public static SysObjFunc<TArg1, TArg2, TArg3, TReturn> operator +(SysObjFunc<TArg1, TArg2, TArg3, TReturn> del, Func<TArg1, TArg2, TArg3, TReturn> handler)
		{
			del.Add(handler);
			return del;
		}

		public static SysObjFunc<TArg1, TArg2, TArg3, TReturn> operator -(SysObjFunc<TArg1, TArg2, TArg3, TReturn> del, Func<TArg1, TArg2, TArg3, TReturn> handler)
		{
			del.Remove(handler);
			return del;
		}

		protected override Func<TArg1, TArg2, TArg3, TReturn> InternalAdd(Func<TArg1, TArg2, TArg3, TReturn> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Func<TArg1, TArg2, TArg3, TReturn> InternalRemove(Func<TArg1, TArg2, TArg3, TReturn> handler)
		{
			return GetDelegate() - handler;
		}
	}
}