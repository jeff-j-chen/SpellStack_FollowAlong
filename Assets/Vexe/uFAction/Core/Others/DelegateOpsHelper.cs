using Vexe.Runtime.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Vexe.Runtime.Helpers;
using Object = UnityEngine.Object;

namespace uFAction
{
	public static class DelegateOpsHelper
	{
		public static void ThrowDelegateNull(string del, string msg)
		{
			throw new NullReferenceException(del + ": delegate is empty - " + msg);
		}

		public static bool Contains(MethodInfo method, IEnumerable<TargetEntry> tEntries)
		{
			return tEntries
				.SelectMany(te => te.MethodEntries)
				.Contains(me => me.Info == method);
		}

		public static void Remove(Object target, MethodInfo method, List<GOEntry> goEntries, Action internalRemove)
		{
			for (int i = goEntries.Count - 1; i > -1; i--)
			{
				var ge = goEntries[i];
				var tEntries = ge.TargetEntries;
				for (int j = tEntries.Count - 1; j > -1; j--)
				{
					var te = tEntries[j];
					int mIndex;
					if ((mIndex = te.MethodEntries.IndexOf(m => m.Info == method)) != -1)
					{
						if (internalRemove != null)
							internalRemove();

						te.MethodEntries.RemoveAt(mIndex);

						// if there are no more methods for this target, remove it
						if (te.MethodEntries.IsEmpty())
							tEntries.RemoveAt(j);

						// no targets for this go? remove it
						if (tEntries.IsEmpty())
							goEntries.RemoveAt(i);
						return;
					}
				}
			}
		}

		public static MethodEntry Add(Object target, MethodInfo method, List<GOEntry> goEntries, Action internalAdd)
		{
			if (internalAdd != null)
				internalAdd();

			MethodEntry mUsed;

			var comp = target as Component;
			var go = comp == null ? GOHelper.EmptyGO : comp.gameObject;
			var goEntry = goEntries.FirstOrDefault(e => e.GO == go);
			if (goEntry != null)
			{ // do we have the target's go?
				int tIndex;
				var tEntries = goEntry.TargetEntries;

				// if we're targeting a new target
				if ((tIndex = tEntries.IndexOf(e => e.Target == target)) == -1)
				{
					var toAdd = new TargetEntry(target, method);
					tEntries.Add(toAdd); // add a new target entry to that go
					mUsed = toAdd.MethodEntries[0];
				}
				else
				{ // otherwise just add the method/handler
					mUsed = new MethodEntry(method);
					tEntries[tIndex].MethodEntries.Add(mUsed);
				}
			}
			else
			{ // otherwise, add a new go entry with that target and handler
				goEntries.Add(new GOEntry(target, method));
				mUsed = goEntries.Last().TargetEntries[0].MethodEntries[0];
			}

			return mUsed;
		}
	}
}