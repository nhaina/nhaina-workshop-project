using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Homeworlds.Logical;
using System;

namespace Homeworlds.View
{
	[CustomEditor(typeof(Bank))]
	public class BankEditor : Editor
	{
		private static ePipColor[] colorValues = (ePipColor[])Enum.GetValues(typeof(ePipColor));
		private static ePipSize[] sizeValues = (ePipSize[])Enum.GetValues(typeof(ePipSize));
		public override void OnInspectorGUI()
		{
			Bank targetBank = target as Bank;

			GameObject[] prefabs = copyPrefabs(targetBank); // new GameObject[colorValues.Length];
			Material[] mats = copyMaterials(targetBank); // new Material[sizeValues.Length];

			var visuals = bankGroup(targetBank);
			sizeGroup(prefabs);
			colorGroup(mats);
			Dictionary<Pip, int> state = stateGroup(targetBank);

			if (GUI.changed)
			{
				Debug.Log("Changed!");
				targetBank.State = state;
				targetBank.Prefabs = prefabs;
				targetBank.Materials = mats;
				targetBank.BankBounds = visuals.Item1;
				targetBank.VerticalOffsetPerSize = visuals.Item2;
				targetBank.UpdateState();
				EditorUtility.SetDirty(target);
			}
			//base.OnInspectorGUI();
		}

		private Tuple<Rect, float> bankGroup(Bank targetBank)
		{
			Rect bounds = EditorGUILayout.RectField(targetBank.BankBounds);
			float y = EditorGUILayout.FloatField(targetBank.VerticalOffsetPerSize);
			return new Tuple<Rect, float>(bounds, y);
		}

		private Dictionary<Pip, int> stateGroup(Bank targetBank)
		{
			if (targetBank.State == null)
			{
				targetBank.State = new Dictionary<Pip, int>();
			}
			Dictionary<Pip, int> dict = targetBank.State;

			foreach (ePipColor color in colorValues)
			{
				foreach (ePipSize size in sizeValues)
				{
					Pip key = new Pip(color, size);
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField($"{color} {size} pip count: ");
					int count;
					if (!targetBank.State.TryGetValue(key, out count))
					{
						count = 0;
					}
					dict[key] = EditorGUILayout.IntField(count);
					EditorGUILayout.EndHorizontal();
				}
			}

			return dict;
		}

		private static void colorGroup(Material[] i_Materials)
		{
			EditorGUILayout.BeginVertical();
			for (int i = 0; i < i_Materials.Length; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(colorValues[i].ToString());
				i_Materials[i] = (Material)EditorGUILayout.ObjectField(i_Materials[i], typeof(Material), false);
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}

		private static void sizeGroup(GameObject[] i_Prefabs)
		{
			EditorGUILayout.BeginVertical();
			for (int i = 0; i < i_Prefabs.Length; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(sizeValues[i].ToString());
				i_Prefabs[i] = (GameObject)EditorGUILayout.ObjectField(i_Prefabs[i], typeof(GameObject), false);
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}

		private GameObject[] copyPrefabs(Bank i_TargetBank)
		{
			GameObject[] prefabsResult = new GameObject[sizeValues.Length];

			int idx = 0;
			foreach (GameObject go in i_TargetBank.Prefabs)
			{
				if (idx >= prefabsResult.Length)
				{
					break;
				}
				prefabsResult[idx++] = go;
			}

			return prefabsResult;
		}

		private Material[] copyMaterials(Bank i_TargetBank)
		{
			Material[] materials = new Material[colorValues.Length];
			int idx = 0;
			foreach (Material mat in i_TargetBank.Materials)
			{
				if (idx >= materials.Length)
				{
					break;
				}
				materials[idx++] = mat;
			}

			return materials;
		}
	}
}