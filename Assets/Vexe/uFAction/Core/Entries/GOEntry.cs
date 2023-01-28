using Vexe.Runtime.Extensions;
using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Linq;
using Vexe.Runtime.Helpers;

namespace uFAction
{
	/// <summary>
	/// A container class for a gameObject to hook up targets from, and a list of ComponentEntries
	/// </summary>
	[Serializable]
	public class GOEntry : FoldableEntry
	{
		[SerializeField]
		private GameObject go;

		[SerializeField]
		private List<TargetEntry> targetEntries;

		public List<TargetEntry> TargetEntries { get { return targetEntries; } set { targetEntries = value; } }
		public GameObject GO { get { return go; } set { go = value; } }
		public Object[] Targets
		{
			get
			{
				return IsNa ?
					targetEntries.Select(e => e.Target).ToArray() :
					GO.GetAllComponentsIncludingSelf();
			}
		}
		public bool IsNa { get { return go == GOHelper.EmptyGO; } }

		public GOEntry()
		{
			targetEntries = new List<TargetEntry>();
		}

		public GOEntry(GameObject go)
			: this()
		{ this.go = go; }

		public GOEntry(TargetEntry target)
			: this(target.GameObject)
		{ targetEntries.Add(target); }


		public GOEntry(Object target, MethodInfo minfo) :
			this(new TargetEntry(target, minfo))
		{ }
	}
}