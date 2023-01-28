using System;
using System.Reflection;
using Object = UnityEngine.Object;
using System.Linq;
using Vexe.Runtime.Extensions;

namespace uFAction
{
	/// <summary>
	/// A serialized delegate (Action) that could target any UnityEngine.Object
	/// and hook up methods of no return value (void) and no parameters
	/// It supports read-only editor integration (for full read/write, use ComponentAction)
	/// </summary>
	[Serializable]
	public class UnityAction : UnityDelegate<Action>, IInvokableWithNoReturn
	{
		/// <summary>
		/// Returns an array of types representing the types of parameters the delegate takes
		/// Since this delegate is parameterless, Type.EmptyTypes is returned
		/// </summary>
		public override Type[] ParamTypes { get { return Type.EmptyTypes; } }

		/// <summary>
		/// The delegate's return type - In this case it's void, since we're dealing with Actions
		/// </summary>
		public override Type ReturnType { get { return typeof(void); } }

		/// <summary>
		/// An internal add method used when rebuilding the invocation list
		/// </summary>
		protected override Action DirectAdd(Action handler)
		{
			return _delegate + handler;
		}

		/// <summary>
		/// An internal add method used when adding a handler to the delegate
		/// </summary>
		protected override Action InternalAdd(Action handler)
		{
			return GetDelegate() + handler;
		}

		/// <summary>
		/// An internal remove method used when removing a handler from the delegate
		/// </summary>
		protected override Action InternalRemove(Action handler)
		{
			return GetDelegate() - handler;
		}

		/// <summary>
		/// Returns the Target object of the specified handler - used for internal purposes
		/// </summary>
		protected override Object GetHandlerTarget(Action handler)
		{
			return handler.Target as Object;
		}

		/// <summary>
		/// Returns the MethodInfo of the specified handler - used for internal purposes
		/// </summary>
		protected override MethodInfo GetHandlerMethod(Action handler)
		{
			return handler.Method;
		}

		/// <summary>
		/// Overloads the '+' operator to make it possible to do 'myDel += myHandler;'
		/// </summary>
		public static UnityAction operator +(UnityAction sd, Action handler)
		{
			sd.Add(handler);
			return sd;
		}

		/// <summary>
		/// Overloads the '-' operator to make it possible to do 'myDel -= myHandler;'
		/// </summary>
		public static UnityAction operator -(UnityAction sd, Action handler)
		{
			sd.Remove(handler);
			return sd;
		}

		/// <summary>
		/// invokes (executes) the delegate - In this case with no parameters, since we're dealing with Actions
		/// </summary>
		public void Invoke()
		{
			var del = GetDelegate();
			if (del != null)
				del();
		}

		/// <summary>
		/// Since this delegate is just an `Action` (has no params) InvokeWithEdtiorArgs is the same as invoking the delegate immediately
		/// so why not save time and do it that way?
		/// </summary>
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
	/// A parameter version of UnityAction that takes one arg of type TArg
	/// </summary>
	public class UnityAction<TArg> : UnityDelegate<Action<TArg>>, IInvokableWithNoReturn<TArg>
	{
		public override Type[] ParamTypes { get { return new[] { typeof(TArg) }; } }

		public override Type ReturnType { get { return typeof(void); } }

		protected override Action<TArg> DirectAdd(Action<TArg> handler)
		{
			return _delegate + handler;
		}

		protected override Action<TArg> InternalAdd(Action<TArg> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Action<TArg> InternalRemove(Action<TArg> handler)
		{
			return GetDelegate() - handler;
		}

		protected override Object GetHandlerTarget(Action<TArg> handler)
		{
			return handler.Target as Object;
		}

		protected override System.Reflection.MethodInfo GetHandlerMethod(Action<TArg> handler)
		{
			return handler.Method;
		}

		public static UnityAction<TArg> operator +(UnityAction<TArg> sd, Action<TArg> handler)
		{
			sd.Add(handler);
			return sd;
		}

		public static UnityAction<TArg> operator -(UnityAction<TArg> sd, Action<TArg> handler)
		{
			sd.Remove(handler);
			return sd;
		}

		public void Invoke(TArg arg)
		{
			var del = GetDelegate();
			if (del != null)
				del(arg);
		}

		protected override void CachedInvokeWithEditorArgs(Delegate handler, object[] args)
		{
			((Action<TArg>)handler).Invoke((TArg)args[0]);
		}
	}

	/// <summary>
	/// A parameter version of UnityAction that takes two args of types TArg1 and TArg2
	/// </summary>
	public class UnityAction<TArg1, TArg2> : UnityDelegate<Action<TArg1, TArg2>>, IInvokableWithNoReturn<TArg1, TArg2>
	{
		public override Type[] ParamTypes { get { return new[] { typeof(TArg1), typeof(TArg2) }; } }

		public override Type ReturnType { get { return typeof(void); } }

		protected override Action<TArg1, TArg2> DirectAdd(Action<TArg1, TArg2> handler)
		{
			return _delegate + handler;
		}

		protected override Action<TArg1, TArg2> InternalAdd(Action<TArg1, TArg2> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Action<TArg1, TArg2> InternalRemove(Action<TArg1, TArg2> handler)
		{
			return GetDelegate() - handler;
		}

		protected override Object GetHandlerTarget(Action<TArg1, TArg2> handler)
		{
			return handler.Target as Object;
		}

		protected override System.Reflection.MethodInfo GetHandlerMethod(Action<TArg1, TArg2> handler)
		{
			return handler.Method;
		}

		public static UnityAction<TArg1, TArg2> operator +(UnityAction<TArg1, TArg2> sd, Action<TArg1, TArg2> handler)
		{
			sd.Add(handler);
			return sd;
		}

		public static UnityAction<TArg1, TArg2> operator -(UnityAction<TArg1, TArg2> sd, Action<TArg1, TArg2> handler)
		{
			sd.Remove(handler);
			return sd;
		}

		public void Invoke(TArg1 arg1, TArg2 arg2)
		{
			var del = GetDelegate();
			if (del != null)
				del(arg1, arg2);
		}

		protected override void CachedInvokeWithEditorArgs(Delegate handler, object[] args)
		{
			((Action<TArg1, TArg2>)handler).Invoke((TArg1)args[0], (TArg2)args[1]);
		}
	}

	/// <summary>
	/// A parameter version of UnityAction that takes three args of types TArg1, TArg2 and TArg3
	/// </summary>
	public class UnityAction<TArg1, TArg2, TArg3> : UnityDelegate<Action<TArg1, TArg2, TArg3>>, IInvokableWithNoReturn<TArg1, TArg2, TArg3>
	{
		public override Type[] ParamTypes { get { return new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) }; } }

		public override Type ReturnType { get { return typeof(void); } }

		protected override Action<TArg1, TArg2, TArg3> DirectAdd(Action<TArg1, TArg2, TArg3> handler)
		{
			return _delegate + handler;
		}

		protected override Action<TArg1, TArg2, TArg3> InternalAdd(Action<TArg1, TArg2, TArg3> handler)
		{
			return GetDelegate() + handler;
		}

		protected override Action<TArg1, TArg2, TArg3> InternalRemove(Action<TArg1, TArg2, TArg3> handler)
		{
			return GetDelegate() - handler;
		}

		protected override Object GetHandlerTarget(Action<TArg1, TArg2, TArg3> handler)
		{
			return handler.Target as Object;
		}

		protected override System.Reflection.MethodInfo GetHandlerMethod(Action<TArg1, TArg2, TArg3> handler)
		{
			return handler.Method;
		}

		public static UnityAction<TArg1, TArg2, TArg3> operator +(UnityAction<TArg1, TArg2, TArg3> sd, Action<TArg1, TArg2, TArg3> handler)
		{
			sd.Add(handler);
			return sd;
		}

		public static UnityAction<TArg1, TArg2, TArg3> operator -(UnityAction<TArg1, TArg2, TArg3> sd, Action<TArg1, TArg2, TArg3> handler)
		{
			sd.Remove(handler);
			return sd;
		}

		public void Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3)
		{
			var del = GetDelegate();
			if (del != null)
				del(arg1, arg2, arg3);
		}

		protected override void CachedInvokeWithEditorArgs(Delegate handler, object[] args)
		{
			((Action<TArg1, TArg2, TArg3>)handler).Invoke((TArg1)args[0], (TArg2)args[1], (TArg3)args[2]);
		}
	}
}