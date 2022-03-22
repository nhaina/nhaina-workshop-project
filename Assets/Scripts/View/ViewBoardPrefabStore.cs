using Homeworlds.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Homeworlds.View
{
	public class ViewBoardPrefabStore : MonoBehaviour
	{
		[SerializeField]
		private GameObject shipPrefab;
		[SerializeField]
		private GameObject starPrefab;
		[SerializeField]
		private List<GameObject> pipPrefabs;
		[SerializeField]
		private List<Material> materials;
		[SerializeField]
		private GameObject bankFloorModelPrefab;

		public GameObject ShipPrefab { get { return shipPrefab; } set { shipPrefab = value; } }
		public GameObject StarPrefab { get { return starPrefab; } set { starPrefab = value; } }
		public GameObject BankFloorModelPrefab { get { return bankFloorModelPrefab; } set { bankFloorModelPrefab = value; } }

		public GameObject FromPipSize(ePipSize i_Size)
		{
			return pipPrefabs[(int)i_Size];
		}

		public Material FromPipColor(ePipColor i_Color)
		{
			return materials[(int)i_Color];
		}

		public void SetPipSizePrefab(ePipSize i_Size, GameObject i_Prefab)
		{
			if (!Enum.IsDefined(typeof(ePipSize), i_Size))
			{
				throw new ArgumentOutOfRangeException($"{(int)i_Size} is not a valid value for ePipSize!");
			}
			if (pipPrefabs == null)
			{
				pipPrefabs = new List<GameObject>() { null, null, null };
			}
			pipPrefabs[(int)i_Size] = i_Prefab;
		}

		public void SetPipColorMaterial(ePipColor i_Color, Material i_Material)
		{
			if (!Enum.IsDefined(typeof(ePipColor), i_Color))
			{
				throw new ArgumentOutOfRangeException($"{(int)i_Color} is not a valid value for ePipColor!");
			}
			if (materials == null)
			{
				materials = new List<Material>() { null, null, null };
			}
			materials[(int)i_Color] = i_Material;
		}
	}
}