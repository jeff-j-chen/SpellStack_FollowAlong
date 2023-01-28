using UnityEngine;
using System.Collections.Generic;
using System;
using Vexe.Runtime.Types;
using UnityEditor;
using Vexe.Editor.Helpers;

/// <summary>
/// A BetterPrefs class used to store boolean values at dictionaries with string/int keys
/// Could be instantiated (via CreateInstance) or treated globally (via Instance)
/// If treated a globally, a ScriptableObject is created and stored as an asset at "*/ScriptableAssets/BetterPrefs.aseet"
/// </summary>
[Serializable]
public class BetterPrefs : ScriptableObject
{
	private static readonly string Path = EditorHelper.ScriptableAssetsPath + "/BetterPrefs.asset";
	private const string MenuPath = "Tools/Vexe/BetterPrefs";

	[SerializeField, HideInInspector]
	private SerializedStringBoolDictionary mStrs;

	[SerializeField, HideInInspector]
	private SerializedIntBoolDictionary mInts;

	private SerializedDictionary<int, bool> ints { get { return SerializedIntBoolDictionary.GetSerializedValue(ref mInts); } }
	private SerializedDictionary<string, bool> strs { get { return SerializedStringBoolDictionary.GetSerializedValue(ref mStrs); } }

	[MenuItem(MenuPath + "/Clear")]
	public static void Clear()
	{
		Instance.strs.Value.Clear();
		Instance.ints.Value.Clear();
	}

	/// <summary>
	/// Creates and returns a new BetterPrefs instance
	/// </summary>
	public static BetterPrefs CreateInstance()
	{
		return ScriptableObject.CreateInstance<BetterPrefs>();
	}

	/// <summary>
	/// Sets the boolean dictionary at the specified key to the specified boolean value
	/// </summary>
	public void SetBool(string key, bool? value)
	{
		if (!value.HasValue)
			throw new ArgumentNullException("Nullable boolean must have a value");
		SetBool(key, value.Value);
	}

	/// <summary>
	/// Sets the boolean dictionary at the specified key to the specified boolean value
	/// </summary>
	public void SetBool(string key, bool value)
	{
		strs[key] = value;
		EditorUtility.SetDirty(this); // wish there's some magic not to forget doing this after wasting hours of trying to figure what the hell is wrong with the class not serializing
	}

	/// <summary>
	/// Returns the boolean value from the dictionary at the specified key
	/// </summary>
	public bool? GetBool(string key)
	{
		bool value;
		return strs.TryGetValue(key, out value) ? value : (bool?)null;
	}

	/// <summary>
	/// Returns the boolean value from the dictionary at the specified key
	/// If the boolean doesn't have a value (null) false is returned instead
	/// </summary>
	public bool GetSafeBool(string key)
	{
		var ret = GetBool(key);
		return ret.HasValue ? ret.Value : false;
	}

	/// <summary>
	/// Sets the boolean dictionary at the specified key to the specified boolean value
	/// </summary>
	public void SetBool(int key, bool? value)
	{
		if (!value.HasValue)
			throw new ArgumentNullException("Nullable boolean must have a value");
		SetBool(key, value.Value);
	}

	/// <summary>
	/// Sets the boolean dictionary at the specified key to the specified boolean value
	/// </summary>
	public void SetBool(int key, bool value)
	{
		ints[key] = value;
		EditorUtility.SetDirty(this);
	}

	/// <summary>
	/// Returns the boolean value from the dictionary at the specified key
	/// </summary>
	public bool? GetBool(int key)
	{
		bool value;
		return ints.TryGetValue(key, out value) ? value : (bool?)null;
	}

	/// <summary>
	/// Returns the boolean value from the dictionary at the specified key
	/// If the boolean doesn't have a value (null) false is returned instead
	/// </summary>
	public bool GetSafeBool(int key)
	{
		var ret = GetBool(key);
		return ret.HasValue ? ret.Value : false;
	}

	private static BetterPrefs instance;

	/// <summary>
	/// Returns the BetterPrefs singleton instance
	/// </summary>
	public static BetterPrefs Instance
	{
		get { return EditorHelper.LazyLoadScriptableAsset<BetterPrefs>(ref instance, Path, true); }
	}

	/// <summary>
	/// Returns the boolean value from the dictionary at the specified key
	/// If the boolean doesn't have a value (null) false is returned instead
	/// </summary>
	public static bool sGetSafeBool(string key)
	{
		return Instance.GetSafeBool(key);
	}

	/// <summary>
	/// Sets the boolean dictionary at the specified key to the specified boolean value
	/// </summary>
	public static void sSetBool(string key, bool value)
	{
		Instance.SetBool(key, value);
	}
}