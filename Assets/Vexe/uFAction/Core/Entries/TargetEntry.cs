using Vexe.Runtime.Extensions;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Reflection;
using Vexe.Runtime.Helpers;

namespace uFAction
{
	/// <summary>
	/// A container class for a target Object to hook up methods from, and a list of MethodEntry
	/// </summary>
	[Serializable]
	public class TargetEntry : FoldableEntry
	{
		[SerializeField]
		protected Object target;

		[SerializeField]
		protected List<MethodEntry> methods = new List<MethodEntry>();

		public Object Target { get { return target; } set { target = value; } }
		public List<MethodEntry> MethodEntries { get { return methods; } set { methods = value; } }
		public MethodInfo[] MethodInfos { get { return methods.Select(m => m.Info).ToArray(); } }
		public Component ComponentTarget { get { return GetTypedTarget<Component>(); } }
		public GameObject GameObject
		{
			get
			{
				if (target == null) return null;
				var comp = ComponentTarget;
				if (comp != null) return comp.gameObject;
				return target as GameObject ?? GOHelper.EmptyGO;
			}
		}

		public TargetEntry() { }
		public TargetEntry(Object target)
		{
			this.target = target;
		}
		public TargetEntry(Object target, MethodEntry method)
			: this(target)
		{
			methods.Add(method);
		}
		public TargetEntry(Object target, MethodInfo minfo) :
			this(target, new MethodEntry(minfo)) { }

		public T GetTypedTarget<T>() where T : Object
		{
			return Target as T;
		}

		public void Invoke()
		{
			foreach (var method in MethodEntries) {
				var info = method.Info;
				if (info == null) continue;
				object[] args;
				if (info.GetParameters().IsEmpty()) {
					args = null;
				}
				else {
					var values = method.ArgsEntries.Select(arg => arg.value).ToList();
					if (info.IsExtensionMethod()) {
						values.Insert(0, target);
					}
					args = values.ToArray();
				}
				info.Invoke(target, args);
			}
		}
	}
}