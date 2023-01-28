using uFAction;
using Vexe.Runtime.Extensions;
using UnityEngine;

/// <summary>
/// An example class that gives the ability to change the state ([en, dis]abled) of a component on a specified target
/// Very useful when used in combine with delegates to remotely change the state of the component
/// </summary>
public class ComponentStateChanger : ComponentModifier
{
	/// <summary>
	/// The delegate to invoke when the component's state has been changed
	/// The new state is passed to all the delegate's subscribers (handlers)
	/// NOTE: _Not_ all components supports a change of state (ex Transform)
	/// so if you try to change the state of a component that doesn't support the change of state,
	/// an InvalidOperationException will be thrown
	/// </summary>
	[HideInInspector] // drawn from a custom inspector instead
	public BooleanAction onStateChanged = new BooleanAction();

	/// <summary>
	/// The state to change our component's state to
	/// </summary>
	public bool toState;

	/// <summary>
	/// Change the specified component's state to the specified 'newState'
	/// </summary>
	public void ChangeComponentState(string componentName, bool newState)
	{
		Component comp;
		if (IsComponentValid(out comp))
			DoChange(comp, newState);
	}

	/// <summary>
	/// Changes the state of the component selected from the inspector to the specified state
	/// </summary>
	public void ChangeSelectedComponentState(bool newState)
	{
		ChangeComponentState(component, newState);
	}

	/// <summary>
	/// Changes the state of the component selected from the inspector to the state that's set from the inspector too
	/// </summary>
	public void ChangeSelectedComponentState()
	{
		ChangeSelectedComponentState(toState);
	}

	/// <summary>
	/// Changes the state of the component selected from the inspector to the state that's set from the inspector too
	/// </summary>
	public override void Modify()
	{
		ChangeSelectedComponentState();
	}


	/// <summary>
	/// Toggles the selected component state
	/// </summary>
	public void ToggleState()
	{
		Component comp;
		if (IsComponentValid(out comp))
			DoChange(comp, !comp.GetState());
	}

	private void DoChange(Component comp, bool newState)
	{
		if (comp.GetState() != newState) {
			comp.ChangeState(newState);
			onStateChanged.Invoke(newState);
		}
	}
}