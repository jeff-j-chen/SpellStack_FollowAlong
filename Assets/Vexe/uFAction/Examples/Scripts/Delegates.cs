using System;
using uFAction;
using UnityEngine;

/// <summary>
/// Just a file that holds delegates - I just thought it'd be nice to have them in one place
/// Add more delegates as and when needed
/// You might consider splitting them into two files if this gets too big
/// </summary>
namespace uFAction
{
	[Serializable]
	public class GameObjectAction : UnityAction<GameObject> { }

	[Serializable]
	public class ColliderAction : UnityAction<Collider> { }

	[Serializable]
	public class CollisionAction : UnityAction<Collision> { }

	[Serializable]
	public class BooleanAction : UnityAction<bool> { }

	[Serializable]
	public class ComponentAction : UnityAction<Component> { }

	[Serializable]
	public class Vector3Action : UnityAction<Vector3> { }

	[Serializable]
	public class StringAction : UnityAction<string> { }
}