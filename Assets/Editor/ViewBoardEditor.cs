using Homeworlds.Logic;
using Homeworlds.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UEditor = UnityEditor.Editor;
using System.Linq;

namespace Homeworlds.View.Editor
{
	[CustomEditor(typeof(ViewBoard))]
	public class ViewBoardEditor : UEditor
	{
		private IStar starToAdd;
		private Ship shipToAdd;
		private List<Ship> ships = null;
		private List<IStar> stars = null;
		public override void OnInspectorGUI()
		{
			ViewBoard targetViewBoard = target as ViewBoard;

			ViewBoardPrefabStore newStore = (ViewBoardPrefabStore)EditorGUILayout.ObjectField("Prefab Store",
				targetViewBoard.s_Store, typeof(ViewBoardPrefabStore), true);

			EditorGUILayout.Space();
			stars = starsSection(stars?? targetViewBoard.current.Stars);
			EditorGUILayout.Space();
			ships = shipsSection(ships?? targetViewBoard.current.Ships);
			if (GUILayout.Button("Update Field"))
			{
				targetViewBoard.s_Store = newStore;
				targetViewBoard.UpdateField(new BoardState(ships, stars, targetViewBoard.current.Status));
			}

		}

		private List<Ship> shipsSection(IEnumerable<Ship> ships)
		{
			List<Ship> shipsResult = new List<Ship>();
			List<int> deleteIndices = new List<int>();
			EditorGUILayout.BeginVertical();
			if (ships != null)
			{
				int idx = 0;
				foreach (Ship item in ships)
				{
					EditorGUILayout.BeginHorizontal();
					if(GUILayout.Button(string.Empty))
					{
						deleteIndices.Add(idx);
					}
					shipsResult.Add(shipLine(item.Color, item.Size, item.Owner, item.StarIdx));
					EditorGUILayout.EndHorizontal();
					idx++;
				}
			}
			foreach (int idx in deleteIndices)
			{
				shipsResult.RemoveAt(idx);
			}
			GUILayout.Label("New Ship:");
			shipToAdd = shipLine(shipToAdd.Color, shipToAdd.Size, shipToAdd.Owner, shipToAdd.StarIdx);
			if (GUILayout.Button("Add"))
			{
				shipsResult.Add(shipToAdd);
			}

			EditorGUILayout.EndVertical();
			return shipsResult;
		}

		private static Ship shipLine(ePipColor i_DefaultColor, ePipSize i_DefaultSize, ePlayer i_DefaultOwner, int i_DefaultStarIdx)
		{
			EditorGUILayout.BeginHorizontal();
			Ship result = new Ship(new Pip(
					(ePipColor)EditorGUILayout.EnumPopup(i_DefaultColor),
					(ePipSize)EditorGUILayout.EnumPopup(i_DefaultSize)),
				(ePlayer)EditorGUILayout.EnumPopup(i_DefaultOwner),
				EditorGUILayout.IntField(i_DefaultStarIdx));
			EditorGUILayout.EndHorizontal();
			return result;
		}

		private List<IStar> starsSection(IEnumerable<IStar> stars)
		{
			List<IStar> starsResult = new List<IStar>();
			List<int> deleteIndices = new List<int>();
			EditorGUILayout.BeginVertical();

			if (stars != null)
			{
				int idx = 0;
				foreach (IStar item in stars)
				{
					EditorGUILayout.BeginHorizontal();
					if (GUILayout.Button(string.Empty))
					{
						deleteIndices.Add(idx);
					}
					starsResult.Add(starLine(item));
					EditorGUILayout.EndHorizontal();
					idx++;
				}
			}
			foreach (int idx in deleteIndices)
			{
				starsResult.RemoveAt(idx);
			}
			GUILayout.Label("New Star:");
			starToAdd = starLine(starToAdd);
			if (GUILayout.Button("Add"))
			{
				starsResult.Add(starToAdd);
			}

			EditorGUILayout.EndVertical();
			return starsResult;
		}

		private IStar starLine(IStar item)
		{
			IStar result;
			item = item?? new Star();
			Pip first = item.Attributes.First();
			EditorGUILayout.BeginHorizontal();
			bool isHw = EditorGUILayout.Toggle(string.Empty, item is Homeworld, GUILayout.MaxWidth(15));
			if (isHw)
			{
				ePlayer owner = ePlayer.Player1;
				Pip? secondary = null;
				if (item is Homeworld itemHw)
				{
					owner = itemHw.Owner;
					secondary = itemHw.SecondaryAttributes;
				}

				owner = (ePlayer)EditorGUILayout.EnumPopup(owner);
				first = new Pip((ePipColor)EditorGUILayout.EnumPopup(first.Color), (ePipSize)EditorGUILayout.EnumPopup(first.Size));
				if (GUILayout.Toggle(secondary.HasValue,string.Empty))
				{
					ePipColor assignedColor = secondary.GetValueOrDefault().Color;
					ePipSize assignedSize = secondary.GetValueOrDefault().Size;
					secondary = new Pip((ePipColor)EditorGUILayout.EnumPopup(assignedColor), (ePipSize)EditorGUILayout.EnumPopup(assignedSize));
				}
				else
				{
					secondary = null;
				}

				result = new Homeworld(first, secondary, owner);
			}
			else
			{

				result = new Star((ePipColor)EditorGUILayout.EnumPopup(first.Color), (ePipSize)EditorGUILayout.EnumPopup(first.Size));
			}
			EditorGUILayout.EndHorizontal();
			return result;
		}
	}

}