using UnityEngine;
using uFAction;

public class Target1 : MonoBehaviour
{
	// Now it's possible to draw delegates from inside custom editor
	// In this case We draw this delegate from TestEditor which is a custom editor for Target1
	[HideInInspector]
	public UnityAction del = new UnityAction();

	public void PrintSquare(int x)
	{
		print("The square of " + x + " is: " + x * x);
	}

	public void PrintName(GameObject go)
	{
		print(go.name);
	}

	private void NoneOfYourBusiness()
	{
		print("how did you find me?! :O");
	}

	public void SayHi()
	{
		Say("Hi");
	}

	public void Say(string what)
	{
		print(what);
	}

	public void Attack(GameObject go, float damage)
	{
		print("Attacking " + go.name + " with " + damage + " amount of damage");
	}
}