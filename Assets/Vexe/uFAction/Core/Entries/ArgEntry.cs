using UnityEngine;
using System;
using System.Reflection;
using Object = UnityEngine.Object;
using System.Text.RegularExpressions;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Helpers;
using Vexe.Runtime.Types;

namespace uFAction
{
	/// <summary>
	/// An argument entry class that makes it possible to set method arguments via editor directly or from a source
	/// </summary>
	[Serializable]
	public class ArgEntry
	{
		public bool isUsingSource;

		[SerializeField]
		private DirectValue _directValue;
		[SerializeField]
		private SourceValue _sourceValue;

		public ArgEntry() { }
		public ArgEntry(ArgEntry from)
		{
			isUsingSource = from.isUsingSource;
			_sourceValue = from.sourceValue;
			_directValue = from.directValue;
		}

		public override string ToString()
		{
			return base.ToString() + " source: " + sourceValue + " direct: " + directValue;
		}

		public SourceValue sourceValue { get { return RTHelper.LazyValue(() => _sourceValue, sv => _sourceValue = sv); } }
		public DirectValue directValue { get { return RTHelper.LazyValue(() => _directValue, dv => _directValue = dv); } }
		public Type TypeUsed { get { return directValue.TypeUsed; } }
		public string memberName { get { return sourceValue.memberName; } set { sourceValue.memberName = value; } }
		public GameObject sourceGo { get { return sourceValue.sourceGo; } set { sourceValue.sourceGo = value; } }
		public Object sourceTarget { get { return sourceValue.sourceTarget; } set { sourceValue.sourceTarget = value; } }
		public object value { get { return isUsingSource ? sourceValue.value : directValue.value; } }

		[Serializable]
		public class SourceValue
		{
			public GameObject sourceGo { get { return _sourceGo; } set { _sourceGo = value; } }
			public Object sourceTarget
			{
				get { return _sourceTarget; }
				set
				{
					if (_sourceTarget != value)
					{
						_sourceTarget = value;
						if (value == null) return;
						var comp = value as Component;
						if (comp) _sourceGo = comp.gameObject;
						else _sourceGo = value as GameObject ?? GOHelper.EmptyGO;
					}
				}
			}

			[SerializeField]
			private GameObject _sourceGo;

			[SerializeField]
			private Object _sourceTarget;

			[SerializeField]
			private string _memberName;

			private FieldInfo finfo;
			private PropertyInfo pinfo;
			private MethodInfo minfo;

			public string memberName
			{
				get { return _memberName; }
				set
				{
					if (_memberName != value)
					{
						_memberName = value;
						pinfo = null;
						finfo = null;
						minfo = null;
					}
				}
			}

			public object value
			{
				get
				{
					if (string.IsNullOrEmpty(_memberName))
						throw new ArgumentNullException("ArgEntry: can't fetch value - field name is null or empty");

					if (sourceTarget == null)
						throw new ArgumentNullException("ArgEntry: can't fetch value - target is null");

					Type t = sourceTarget.GetType();
					var flags = Settings.sSourceBindingFlags;

					if (finfo == null) finfo = t.GetField(_memberName, flags);
					if (finfo != null) return finfo.GetValue(sourceTarget);
					if (pinfo == null) pinfo = t.GetProperty(_memberName, flags);
					if (pinfo != null) return pinfo.GetValue(sourceTarget, null);
					if (minfo == null) minfo = t.GetMethod(Regex.Replace(_memberName, "[()]", ""), Type.EmptyTypes, flags);
					if (minfo != null) return minfo.Invoke(sourceTarget, null);
					return null;
				}
			}
		}

		[Serializable]
		public class DirectValue // Couldn't have been more ugly! -___-
		{
			[SerializeField]
			private SerializedType typeUsed = new SerializedType();

			public Type TypeUsed { get { return typeUsed.Value; } }

			public object value
			{
				get
				{
					var uType = typeUsed.Value;
					if (uType == typeof(int))
					{
						return _intValue;
					}
					if (uType == typeof(float))
					{
						return _floatValue;
					}
					if (uType == typeof(bool))
					{
						return _boolValue;
					}
					if (uType == typeof(string))
					{
						return _stringValue;
					}
					if (uType == typeof(Vector3))
					{
						return _vector3Value;
					}
					if (uType == typeof(Vector2))
					{
						return _vector2Value;
					}
					if (uType == typeof(Bounds))
					{
						return _boundsValue;
					}
					if (uType == typeof(Rect))
					{
						return _rectValue;
					}
					if (uType == typeof(Color))
					{
						return _colorValue;
					}
					if (typeof(Enum).IsAssignableFrom(uType))
					{
						return Enum.Parse(uType, _enumOptions[_enumIndex]);
					}
					if (typeof(Object).IsAssignableFrom(uType))
					{
						return _objectRefValue;
					}
					if (uType == typeof(Quaternion))
					{
						return _quaternionValue;
					}
					return null;
				}
				set
				{
					var type = value == null ? typeof(Object) : value.GetType();
					typeUsed.Value = type;

					if (type == typeof(int))
					{
						_intValue = (int)value;
					}
					else if (type == typeof(float))
					{
						_floatValue = (float)value;
					}
					else if (type == typeof(bool))
					{
						_boolValue = (bool)value;
					}
					else if (type == typeof(string))
					{
						_stringValue = (string)value;
					}
					else if (type == typeof(Vector3))
					{
						_vector3Value = (Vector3)value;
					}
					else if (type == typeof(Vector2))
					{
						_vector2Value = (Vector2)value;
					}
					else if (type == typeof(Bounds))
					{
						_boundsValue = (Bounds)value;
					}
					else if (type == typeof(Rect))
					{
						_rectValue = (Rect)value;
					}
					else if (type == typeof(Color))
					{
						_colorValue = (Color)value;
					}
					else if (type == typeof(Quaternion))
					{
						_quaternionValue = (Quaternion)value;
					}
					else if (type.IsEnum)
					{
						SetEnums(type);
						_enumIndex = 0;
					}
					else if (typeof(Object).IsAssignableFrom(type))
					{
						_objectRefValue = value as Object;
					}
					else throw new InvalidOperationException(string.Format("Can't set value - Type `{0}` is not supported", type));
				}
			}

			[SerializeField]
			private int _intValue;
			[SerializeField]
			private float _floatValue;
			[SerializeField]
			private bool _boolValue;
			[SerializeField]
			private string _stringValue = string.Empty;
			[SerializeField]
			private Vector3 _vector3Value;
			[SerializeField]
			private Vector2 _vector2Value;
			[SerializeField]
			private Object _objectRefValue;
			[SerializeField]
			private Bounds _boundsValue;
			[SerializeField]
			private Rect _rectValue;
			[SerializeField]
			private Color _colorValue;
			[SerializeField]
			private Quaternion _quaternionValue;
			[SerializeField]
			private int _enumIndex;
			[SerializeField]
			private string[] _enumOptions;

			public int intValue { get { return _intValue; } }
			public float floatValue { get { return _floatValue; } }
			public bool boolValue { get { return _boolValue; } }
			public string stringValue { get { return _stringValue; } }
			public Vector2 vector2Value { get { return _vector2Value; } }
			public Vector3 vector3Value { get { return _vector3Value; } }
			public Bounds boundsValue { get { return _boundsValue; } }
			public Object objectRefValue { get { return _objectRefValue; } }
			public Rect rectValue { get { return _rectValue; } }
			public Color colorValue { get { return _colorValue; } }
			public Quaternion quaternionValue { get { return _quaternionValue; } }
			public string[] enumOptions { get { return _enumOptions; } }
			public int enumIndex { get { return _enumIndex; } set { _enumIndex = value; } }

			public void SetQuaternionFromVector3(Vector3 v)
			{
				var newQuat = new Quaternion();
				newQuat.eulerAngles = v;
				_quaternionValue = newQuat;
			}

			public void SetEnums(Type type)
			{
				typeUsed.Value = type;
				_enumOptions = Enum.GetNames(type);
			}
		}
	}
}