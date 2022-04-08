using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StarNames", menuName = "ScriptableObjects/StarNames", order = 1)]
public class StarNamesSo : ScriptableObject
{
	[SerializeField]
	private List<string> names;

	public string this[int index]
	{
		get { return names[index % names.Count]; }
	}
}
