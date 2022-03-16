using Homeworlds.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Homeworlds.View
{
	public class ViewBoardPrefabStore : MonoBehaviour
	{
		public GameObject shipPrefab;
		public GameObject starPrefab;
		public List<GameObject> pipPrefabs;
		public List<Material> materials;

		public GameObject FromPipSize(ePipSize i_Size)
		{
			return pipPrefabs[(int)i_Size];
		}

		public Material FromPipColor(ePipColor i_Color)
		{
			return materials[(int)i_Color];
		}
	}
}