using UnityEngine;
using EditorGUIFramework;
using DEVBUS;
using Vexe.Runtime.Helpers;

namespace ShowEmAll.DrawMates
{
	public abstract class BaseDrawer<TWrapper, TOption>
		where TWrapper : BaseWrapper<TOption>
		where TOption : LayoutOption, new()
	{
		public TWrapper gui { get; set; }
		public Object target { get; set; }

		private BetterUndo _undo = new BetterUndo();
		protected BetterUndo undo { get { return BetterUndo.MakeCurrent(ref _undo); } }

		public BaseDrawer()
		{
		}

		public BaseDrawer(TWrapper gui, Object target)
		{
			this.gui = gui;
			this.target = target;
		}

		protected void HeightHasChanged()
		{
			var gw = gui as IMustBeNotifiedOnHeightChange;
			if (gw != null)
				gw.HeightHasChanged();
		}

		public abstract void Draw();

		/// <summary>
		/// Used for foldouts - override to be more specific thus have unique keys
		/// </summary>
		protected virtual string key { get { return RTHelper.GetTargetID(target); } }

		protected BetterPrefs Prefs { get { return BetterPrefs.Instance; } }

		protected bool foldout
		{
			get { return Prefs.GetSafeBool(key); }
			set { Prefs.SetBool(key, value); }
		}

		protected void SetFoldout(bool value)
		{
			Prefs.SetBool(key, value);
		}

		protected bool GetFoldout()
		{
			return Prefs.GetSafeBool(key);
		}

		protected void log(object msg)
		{
			Debug.Log(msg);
		}
	}
}