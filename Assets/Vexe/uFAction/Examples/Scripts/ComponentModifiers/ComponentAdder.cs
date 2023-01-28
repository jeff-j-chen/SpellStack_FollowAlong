using System;
using uFAction;
using Vexe.Runtime.Helpers;
using UnityEngine;

/// <summary>
/// An example class that gives the ability to add components to a specified target
/// Very useful when used in combine with delegates to remotely add the component
/// </summary>
public class ComponentAdder : ComponentModifier
{
	/// <summary>
	/// The delegate to invoke when the component's been added to the target
	/// You could take this one step further and use a ComponentAction (UnityAction<Component>)
	/// to pass in the added component to all the delegate's subscribers to let them know about the added component
	/// </summary>
	[HideInInspector] // We hide it and draw it from our custom inspector
	public UnityAction onAdd = new UnityAction();

	/// <summary>
	/// Adds a component with the specified name to the target
	/// Throws an InvalidOperationException if the component's name wasn't valid (ex "RigidBody" instead of "Rigidbody")
	/// </summary>
	public void AddComponent(string name)
	{
		Type type = null;
		if (ReflectionHelper.IsValidTypeName(name, ref type)) {
			target.AddComponent(type);
			onAdd.Invoke();
		}
		else throw new InvalidOperationException(string.Format("Component `{0}` is not valid to add", name));
	}

	/// <summary>
	/// Adds the component selected in the inspector to the target
	/// </summary>
	public void AddSelectedComponent()
	{
		AddComponent(component);
	}

	/// <summary>
	/// Adds the component selected in the inspector to the target
	/// </summary>
	public override void Modify()
	{
		AddSelectedComponent();
	}
}