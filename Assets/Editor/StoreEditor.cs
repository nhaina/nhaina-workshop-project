using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Homeworlds.Common;

namespace Homeworlds.View.Editor
{
	[CustomEditor(typeof(ViewBoardPrefabStore))]
	public class StoreEditor : UnityEditor.Editor
	{
		private bool colorsFoldoutShown;
		private bool sizesFoldoutShown;

		SerializedProperty shipPrefab;
		SerializedProperty starPrefab;
		SerializedProperty bankFloorModelPrefab;

		private void OnEnable()
		{
			shipPrefab = serializedObject.FindProperty("shipPrefab");
			starPrefab = serializedObject.FindProperty("starPrefab");
			bankFloorModelPrefab = serializedObject.FindProperty("bankFloorModelPrefab");
		}


		public override void OnInspectorGUI()
		{
			bool targetingPrefab = AssetDatabase.Contains(target);

			ViewBoardPrefabStore targetStore = (ViewBoardPrefabStore)target;
			serializedObject.Update();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.PropertyField(shipPrefab);
			EditorGUILayout.PropertyField(starPrefab);
			EditorGUILayout.PropertyField(bankFloorModelPrefab);
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical();
			colorsFoldoutShown = EditorGUILayout.Foldout(colorsFoldoutShown, "Pip Color Materials:");
			if (colorsFoldoutShown)
			{
				foreach (ePipColor color in Logic.Utilities.AllPipColors)
				{
					targetStore.SetPipColorMaterial(color, (Material)EditorGUILayout.ObjectField(
						$"{color}:", targetStore.FromPipColor(color), typeof(Material), false));
				}
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical();
			sizesFoldoutShown = EditorGUILayout.Foldout(sizesFoldoutShown, "Pip Sizes Prefabs:");
			if (sizesFoldoutShown)
			{
				foreach (ePipSize size in Logic.Utilities.AllPipSizes)
				{
					targetStore.SetPipSizePrefab(size, (GameObject)EditorGUILayout.ObjectField(
						$"{size}:", targetStore.FromPipSize(size), typeof(GameObject), !targetingPrefab));
				}
			}
			EditorGUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();
			if (GUI.changed)
			{
				if (targetingPrefab)
				{
					PrefabUtility.SavePrefabAsset(targetStore.gameObject);
				}
			}
		}
	}

}