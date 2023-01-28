using UnityEngine;
using uFAction;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/// <summary>
/// A small test for SysObjAction show how delegates could easily solve some design issues (in this case circular dependency)
/// without introducing a new level of complexity.
/// 
/// The following is an example where we have a Barn,
/// which has a list of Cows and an Owner which has a Barn,
/// and a list of food he could feed his cows with.
/// 
/// You could order the cows to move, which will increase their number of heart beats,
/// if the number of heart beats reaches 'hungerThreshold' the cow becomes hungry,
/// now the person who feeds them is the owner, so when a cow is hungry, she has to notify her owner,
/// a lot of people would solve this by letting the cow reference the owner but then you'd fall into circular dependency (not direct in this case)
/// Owner --> Barn --> Cow
///     <---------------
/// With delegates, this is easily solved, a cow has an onHungry delegate to invoke when its hungry passing itself,
/// when we initialize, we let the owner subscribe to that delegate/event - so when a cow gets hungry,
/// all she does is fire the event and the owner will get notified. Notice now the cow is not referencing the owner,
/// she does not even know that she has an owner :)
/// 
/// I have used this pattern to solve many similar problems, like:
/// - Inventory-Slots relationship, where an inventory has a list of slots.
///   When a slot has been clicked it needs to notify the inventory for it take do whatever necessary
///   (like hold item, discard item, show some context menu, etc) because that is not the responsibility of the slot.
///   So how can the slot notify the inventory in a way that avoids circular dependency? Simple, we give the slot an OnClick(Slot).
///   Whenever the inventory creates a new slot, it subscribes to the slot`s OnClick.
///   Now when a slot is clicked, it just fires OnClick passing itself.
///   And so the inventory got notified indirectly of what slot has been clicked on.
/// 
/// If you are not familiar with circular dep http://en.wikipedia.org/wiki/Circular_dependency
/// </summary>
public class HowToEasilySolveCircularDependency : MonoBehaviour
{
	/// <summary>
	/// Just to show that the delegate do serialize and survives assembly reloads,
	/// we use this init flag, whose sole purpose is to make sure our initialization runs once.
	/// Now, after everything's been hooked up, try trigger an assembly reload by modifying the script
	/// add an "int x;" somewhere... After the recompilation is complete, the delegate value persists.
	/// </summary>
	[SerializeField, HideInInspector]
	private bool hasInit;

	[SerializeField, HideInInspector]
	private Owner owner;

	private void Awake()
	{
		if (!hasInit) {
			Debug.Log("Initializing...");
			hasInit = true;
			var betsy = new Cow("Betsy");
			var sarah = new Cow("Sarah");
			var barn = new Barn { cows = new List<Cow> { betsy, sarah } };
			var foods = new List<string> { "Pizza", "CheezeBurger", "Chips", "Gum", "Stuff" };
			owner = new Owner { barn = barn, foods = foods };

			foreach (var cow in barn.cows) {
				// Doing "cow.OnHungry += owner.FeedCow;" will yeild a casting error
				// That's because the +/- operators are overloaded in SysObjAction<T>
				// and not in CowDelegate. To fix this, we either have to overload the operators
				// again in CowDelegate, or do:
				// cow.onHungry = (CowDelegate)(cow.OnHungry + owner.FeedCow);
				// Or simply just use the add method:
				cow.OnHungry.Add(owner.FeedCow);
			}
		}
	}

	private void OnGUI()
	{
		foreach (var cow in owner.barn.cows) {
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Cow: " + cow.Name + " HeartBeats: " + cow.HeartBeats);
				if (GUILayout.Button("Move"))
					cow.Move();
			}
			GUILayout.EndHorizontal();
		}
	}
}

[Serializable]
public class Barn
{
	public List<Cow> cows;
}

[Serializable]
public class Owner
{
	public Barn barn;
	public List<string> foods;

	private string RandomFood { get { return foods[Random.Range(0, foods.Count)]; } }

	public void FeedCow(Cow c)
	{
		string food = RandomFood;
		Debug.Log(string.Format("Cow {0} was hungry... feeding her {1}", c.Name, food));
		c.Eat(food);
	}
}

[Serializable]
public class Cow
{
	[SerializeField]
	private int nHeartBeats; // for the sake of simplicity, 0 is the normal number of heart beats

	[SerializeField]
	private string name;

	[SerializeField]
	private CowAction onHungry = new CowAction();

	private int hungerThreshold;

	public CowAction OnHungry { get { return onHungry; } }
	public string Name { get { return name; } }
	public int HeartBeats { get { return nHeartBeats; } }
	public bool IsHungry { get { return nHeartBeats % hungerThreshold == 0; } }

	public Cow(string name)
	{
		this.name = name;
		RegenerateHunger();
	}

	private void RegenerateHunger()
	{
		hungerThreshold = UnityEngine.Random.Range(5, 10);
	}

	public void Move()
	{
		nHeartBeats++;
		if (IsHungry)
			OnHungry.Invoke(this);
	}

	public void Eat(string food)
	{
		Debug.Log(name + " is eating " + food);

		// if a cow eats, she relaxes thus nHeartBeats decreases
		nHeartBeats = Mathf.Max(0, nHeartBeats - food.Length);
		if (nHeartBeats == 0)
			RegenerateHunger();
	}
}

[Serializable]
public class CowAction : SysObjAction<Cow> { }