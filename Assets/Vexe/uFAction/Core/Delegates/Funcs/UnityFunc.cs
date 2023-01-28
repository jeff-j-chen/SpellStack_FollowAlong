using System;
using System.Reflection;
using Object = UnityEngine.Object;

namespace uFAction
{
	/// <summary>
	/// A serialized delegate (Func) that could target any `UnityEngine.Object` 
	/// and hook up methods that has a return type of TReturn and no parameters
	/// </summary>
	public class UnityFunc<TReturn> : UnityDelegate<Func<TReturn>>, IInvokableWithReturn<TReturn>
	{
		public override Type[] ParamTypes { get { return Type.EmptyTypes; } }

		public override Type ReturnType { get { return typeof(TReturn); } }

		/// <summary>
		/// Invokes the delegate (in this case with no params)
		/// If the delegate was null, a NullReferenceException gets thrown instead of returning null, cause that would mean that
		/// the delegate got invoked but the return value that got back was null
		/// </summary>
		public TReturn Invoke()
		{
			var del = GetDelegate();
			if (del == null)
				DelegateOpsHelper.ThrowDelegateNull("UnityFunc", "Delegate is null - Can't invoke");
			return del();
		}

		protected override Func<TReturn> DirectAdd(Func<TReturn> handler)
		{
			return _delegate + handler;
		}

		protected override Func<TReturn> InternalAdd(Func<TReturn> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Func<TReturn> InternalRemove(Func<TReturn> handler)
		{
			return GetDelegate() - handler;
		}

		protected override Object GetHandlerTarget(Func<TReturn> handler)
		{
			return handler.Target as Object;
		}

		protected override MethodInfo GetHandlerMethod(Func<TReturn> handler)
		{
			return handler.Method;
		}


		public static UnityFunc<TReturn> operator +(UnityFunc<TReturn> del, Func<TReturn> handler)
		{
			del.Add(handler);
			return del;
		}

		public static UnityFunc<TReturn> operator -(UnityFunc<TReturn> del, Func<TReturn> handler)
		{
			del.Remove(handler);
			return del;
		}

		public static implicit operator UnityFunc<TReturn>(Func<TReturn> handler)
		{
			var func = new UnityFunc<TReturn>();
			func += handler;
			return func;
		}

		public override void InvokeWithEditorArgs()
		{
			Invoke();
		}

		protected override void CachedInvokeWithEditorArgs(Delegate handler, object[] args)
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// A parameter version of UnityFunc`TReturn` - Could hook up methods that take one arg of type TArg
	/// </summary>
	public class UnityFunc<TArg, TReturn> : UnityDelegate<Func<TArg, TReturn>>, IInvokableWithReturn<TArg, TReturn>
	{
		public override Type[] ParamTypes { get { return new[] { typeof(TArg) }; } }

		public override Type ReturnType { get { return typeof(TReturn); } }

		/// <summary>
		/// Invokes the delegate with 'arg'
		/// If the delegate was null, a NullReferenceException gets thrown instead of returning null, cause that would mean that
		/// the delegate got invoked but the return value that got back was null
		/// </summary>
		public TReturn Invoke(TArg arg)
		{
			var del = GetDelegate();
			if (del == null)
				DelegateOpsHelper.ThrowDelegateNull("UnityFunc", "Can't invoke");
			return del(arg);
		}

		protected override Func<TArg, TReturn> DirectAdd(Func<TArg, TReturn> handler)
		{
			return _delegate + handler;
		}

		protected override Func<TArg, TReturn> InternalAdd(Func<TArg, TReturn> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Func<TArg, TReturn> InternalRemove(Func<TArg, TReturn> handler)
		{
			return GetDelegate() - handler;
		}

		protected override Object GetHandlerTarget(Func<TArg, TReturn> handler)
		{
			return handler.Target as Object;
		}

		protected override System.Reflection.MethodInfo GetHandlerMethod(Func<TArg, TReturn> handler)
		{
			return handler.Method;
		}


		public static UnityFunc<TArg, TReturn> operator +(UnityFunc<TArg, TReturn> del, Func<TArg, TReturn> handler)
		{
			del.Add(handler);
			return del;
		}

		public static UnityFunc<TArg, TReturn> operator -(UnityFunc<TArg, TReturn> del, Func<TArg, TReturn> handler)
		{
			del.Remove(handler);
			return del;
		}

		protected override void CachedInvokeWithEditorArgs(Delegate handler, object[] args)
		{
			((Func<TArg, TReturn>)handler).Invoke((TArg)args[0]);
		}
	}

	/// <summary>
	/// A parameter version of UnityFunc`TReturn` - Could hook up methods that take two args of types TArg1 and TArg2
	/// </summary>
	public class UnityFunc<TArg1, TArg2, TReturn> : UnityDelegate<Func<TArg1, TArg2, TReturn>>, IInvokableWithReturn<TArg1, TArg2, TReturn>
	{
		public override Type[] ParamTypes { get { return new[] { typeof(TArg1), typeof(TArg2) }; } }

		public override Type ReturnType { get { return typeof(TReturn); } }

		/// <summary>
		/// Invokes the delegate with the arguments 'arg1' and 'arg2'
		/// If the delegate was null, a NullReferenceException gets thrown instead of returning null, cause that would mean that
		/// the delegate got invoked but the return value that got back was null
		/// </summary>
		public TReturn Invoke(TArg1 arg1, TArg2 arg2)
		{
			var del = GetDelegate();
			if (del == null)
				DelegateOpsHelper.ThrowDelegateNull("UnityFunc", "Can't invoke");
			return del(arg1, arg2);
		}

		protected override Func<TArg1, TArg2, TReturn> DirectAdd(Func<TArg1, TArg2, TReturn> handler)
		{
			return _delegate + handler;
		}

		protected override Func<TArg1, TArg2, TReturn> InternalAdd(Func<TArg1, TArg2, TReturn> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Func<TArg1, TArg2, TReturn> InternalRemove(Func<TArg1, TArg2, TReturn> handler)
		{
			return GetDelegate() - handler;
		}
		protected override Object GetHandlerTarget(Func<TArg1, TArg2, TReturn> handler)
		{
			return handler.Target as Object;
		}

		protected override MethodInfo GetHandlerMethod(Func<TArg1, TArg2, TReturn> handler)
		{
			return handler.Method;
		}


		public static UnityFunc<TArg1, TArg2, TReturn> operator +(UnityFunc<TArg1, TArg2, TReturn> del, Func<TArg1, TArg2, TReturn> handler)
		{
			del.Add(handler);
			return del;
		}

		public static UnityFunc<TArg1, TArg2, TReturn> operator -(UnityFunc<TArg1, TArg2, TReturn> del, Func<TArg1, TArg2, TReturn> handler)
		{
			del.Remove(handler);
			return del;
		}

		protected override void CachedInvokeWithEditorArgs(Delegate handler, object[] args)
		{
			((Func<TArg1, TArg2, TReturn>)handler).Invoke((TArg1)args[0], (TArg2)args[1]);
		}
	}

	/// <summary>
	/// A parameter version of UnityFunc`TReturn` - Could hook up methods that take three args of types TArg1, TArg2 and TArg3
	/// </summary>
	public class UnityFunc<TArg1, TArg2, TArg3, TReturn> : UnityDelegate<Func<TArg1, TArg2, TArg3, TReturn>>, IInvokableWithReturn<TArg1, TArg2, TArg3, TReturn>
	{
		public override Type[] ParamTypes { get { return new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }; } }

		public override Type ReturnType { get { return typeof(TReturn); } }

		/// <summary>
		/// Invokes the delegate with the arguments 'arg1', 'arg2' and 'arg3'
		/// If the delegate was null, a NullReferenceException gets thrown instead of returning null, cause that would mean that
		/// the delegate got invoked but the return value that got back was null
		/// </summary>
		public TReturn Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3)
		{
			var del = GetDelegate();
			if (del == null)
				DelegateOpsHelper.ThrowDelegateNull("UnityFunc", "Can't invoke");
			return del(arg1, arg2, arg3);
		}

		protected override Func<TArg1, TArg2, TArg3, TReturn> DirectAdd(Func<TArg1, TArg2, TArg3, TReturn> handler)
		{
			return _delegate + handler;
		}

		protected override Func<TArg1, TArg2, TArg3, TReturn> InternalAdd(Func<TArg1, TArg2, TArg3, TReturn> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Func<TArg1, TArg2, TArg3, TReturn> InternalRemove(Func<TArg1, TArg2, TArg3, TReturn> handler)
		{
			return GetDelegate() - handler;
		}

		protected override Object GetHandlerTarget(Func<TArg1, TArg2, TArg3, TReturn> handler)
		{
			return handler.Target as Object;
		}

		protected override System.Reflection.MethodInfo GetHandlerMethod(Func<TArg1, TArg2, TArg3, TReturn> handler)
		{
			return handler.Method;
		}

		public static UnityFunc<TArg1, TArg2, TArg3, TReturn> operator +(UnityFunc<TArg1, TArg2, TArg3, TReturn> del, Func<TArg1, TArg2, TArg3, TReturn> handler)
		{
			del.Add(handler);
			return del;
		}

		public static UnityFunc<TArg1, TArg2, TArg3, TReturn> operator -(UnityFunc<TArg1, TArg2, TArg3, TReturn> del, Func<TArg1, TArg2, TArg3, TReturn> handler)
		{
			del.Remove(handler);
			return del;
		}

		protected override void CachedInvokeWithEditorArgs(Delegate handler, object[] args)
		{
			((Func<TArg1, TArg2, TArg3, TReturn>)handler).Invoke((TArg1)args[0], (TArg2)args[1], (TArg3)args[2]);
		}
	}
}