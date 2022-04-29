using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Homeworlds.View
{
	public class PersistentObject : MonoBehaviour
	{
		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}
	}
}