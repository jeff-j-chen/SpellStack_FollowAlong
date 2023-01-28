using System.Collections.Generic;
using UnityEngine;

public class Target2 : MonoBehaviour
{
	// Used from TestEditor2
	public List<string> strings = new List<string>();

	private void TellMeASecret(string secret)
	{
		print("Hush! don't tell anybody: " + secret);
	}

	public void InversePosition()
	{
		transform.position = -transform.position;
	}

	public void MoveTo(Vector3 pos)
	{
		transform.position = pos;
	}

	public void ReceiveMessage(GameObject sender, string msg)
	{
		print(string.Format("Receieved a message from `{0}` saying: \"{1}\"", sender.name, msg));
	}
}