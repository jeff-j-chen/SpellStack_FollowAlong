using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using ShowEmAll;

#if UNITY_EDITOR
using UnityEditor;
#endif

#pragma warning disable 0649

namespace uFAction
{
	[Serializable]
	public class Settings : ScriptableObject
	{
		[SerializeField]
		private bool debugMode;

		[SerializeField]
		private int maxValuesCountInPopup = 30;

		[SerializeField]
		private int maxValueLengthInPopup = 40;

		[SerializeField]
		private bool keepWndFocusedAfterSelection;

		[EnumMask, SerializeField]
		private SimpleBindingFlags methodBindingFlags = SimpleBindingFlags.Public;

		[EnumMask, SerializeField]
		private SimpleBindingFlags sourceBindingFlags = SimpleBindingFlags.Public | SimpleBindingFlags.DeclaredOnly;

		[SerializeField]
		private bool showExtensionMethods;

		[SerializeField]
		private ColorTheme colorTheme;

		[SerializeField]
		private List<string> methodsToExclude = new List<string> { "CancelInvoke", "StopAllCoroutines" };

		/// <summary>
		/// Used mainly to trigger logs - internal cooking - useful when hunting for bugs
		/// </summary>
		public bool DebugMode { get { return debugMode; } }

		/// <summary>
		/// Whether or not to keep the selection window active (focused) after selecting a value (a target, method or when adding a new go)
		/// </summary>
		public bool KeepWindowFocusedAfterSelection { get { return keepWndFocusedAfterSelection; } }

		/// <summary>
		/// Returns the maximum number of characters a value could have in a popup - if the number exceeds, a window is used instead
		/// </summary>
		public int MaxValueLengthInPopup { get { return maxValueLengthInPopup; } }

		/// <summary>
		/// Returns the maximum number of values to be used in a popup - if the number exceeds, a window is used instead
		/// </summary>
		public int MaxValuesCountInPopup { get { return maxValuesCountInPopup; } }

		/// <summary>
		/// Methods to exclude from appearing
		/// </summary>
		public List<string> MethodsToExclude { get { return methodsToExclude; } }

		/// <summary>
		/// The binding flags that's used to bind to methods
		/// </summary>
		public BindingFlags MethodBindingFlags { get { return (BindingFlags)methodBindingFlags | BindingFlags.Instance; } }

		/// <summary>
		/// The binding flags that's used to bind to source values
		/// </summary>
		public BindingFlags SourceBindingFlags { get { return (BindingFlags)sourceBindingFlags | BindingFlags.Instance; } }

		/// <summary>
		/// Wether or not to show extension methods
		/// </summary>
		public bool ShowExtensionMethods { get { return showExtensionMethods; } }

		/// <summary>
		/// The color theme used by the delegate
		/// </summary>
		public ColorTheme ColorTheme { get { return colorTheme; } }

		public static bool sDebugMode { get { return Instance.DebugMode; } }
		public static int sMaxValuesCountInPopup { get { return Instance.MaxValuesCountInPopup; } }
		public static int sMaxValueLengthInPopup { get { return Instance.MaxValueLengthInPopup; } }
		public static bool sKeepSelectionWindowActiveAfterAdd { get { return Instance.KeepWindowFocusedAfterSelection; } }
		public static BindingFlags sMethodBindingFlags { get { return Instance.MethodBindingFlags; } }
		public static BindingFlags sSourceBindingFlags { get { return Instance.SourceBindingFlags; } }
		public static bool sShowExtensionMethods { get { return Instance.ShowExtensionMethods; } }
		public static List<string> sMethodsToExclude { get { return Instance.MethodsToExclude; } }
		public static ColorTheme sColorTheme
		{
			get
			{
#if UNITY_EDITOR
				return LazyLoadScriptableAsset(ref Instance.colorTheme, themesPath, true);
#else
				return instance.colorTheme;
#endif
			}
		}

		private static Settings instance;
		public static Settings Instance
		{
			get
			{
#if UNITY_EDITOR
				return LazyLoadScriptableAsset(ref instance, settingsPath, true);
#else
				return instance;
#endif
			}
		}

#if UNITY_EDITOR
		public const string uFAction = "uFAction";
		public const string MenuPath = "Tools/Vexe/" + uFAction;
		private static string packagePath;

		private static void SelectObject(Object obj)
		{
			Selection.activeObject = obj;
		}

		[MenuItem(MenuPath + "/Settings")]
		public static void GoToSettings()
		{
			SelectObject(Instance);
		}

		[MenuItem(MenuPath + "/CreateTheme")]
		public static void CreateTheme()
		{
			var theme = CreateScriptableObjectAsset<ColorTheme>(GetPackagePath() + "/Themes/NewTheme");
			theme.Init();
			SelectObject(theme);
		}

		public static string GetPackagePath()
		{
			return DirectoryHelper.LazyGetDirectoryPath(ref packagePath, uFAction);
		}

		public static T LazyLoadScriptableAsset<T>(ref T value, string path, bool log) where T : ScriptableObject
		{
			if (value == null)
			{
				path = path.NormalizePath();
				value = AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T;
				if (value == null)
				{
					if (log) Debug.Log("No asset of type `" + typeof(T) + "` was found - creating new");
					value = CreateScriptableObjectAsset<T>(path);
				}
			}
			return value;
		}
		private static T CreateScriptableObjectAsset<T>(string path) where T : ScriptableObject
		{
			T so = CreateInstance<T>();
			if (Path.GetExtension(path) != ".asset")
				path += ".asset";
			AssetDatabase.CreateAsset(so, path);
			return so;
		}
		private static string settingsPath { get { return GetPackagePath() + "/Settings.asset"; } }
		private static string themesPath { get { return GetPackagePath() + "/Themes"; } }
		//private static string settingsPath { get { return uFActionEditorUtils.GetPackagePath() + "/Settings.asset"; } }
		//private static string themesPath { get { return uFActionEditorUtils.GetPackagePath() + "/Themes"; } }
#endif
	}
}