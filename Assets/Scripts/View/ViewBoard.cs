﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Homeworlds.Common;
using Homeworlds.Logic;
using System;
using System.Linq;

namespace Homeworlds.View
{
	public class ViewBoard : MonoBehaviour
	{
		private BoardState current;
		[SerializeField]
		private ViewBoardPrefabStore store;
		[SerializeField]
		private BankDescriptor bank;
		[SerializeField]
		private Rect gameField;
		public float arrangerMinRowHeight = 1.4f;
		public IStarArranger StarArranger { get; set; }
		private readonly List<StarDescriptor> regularStars = new List<StarDescriptor>();
		private readonly List<ShipDescriptor> ships = new List<ShipDescriptor>();
		StarDescriptor player1HomeworldDesc, player2HomeworldDesc;

		private void OnDrawGizmos()
		{
			Vector3 center, size;
			center = transform.position + new Vector3(gameField.center.x, 0, gameField.center.y);
			size = new Vector3(gameField.width, 0f, gameField.height);
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(center, size);
			Gizmos.DrawLine(center + new Vector3(0, 0, 0.5f * gameField.height), center + new Vector3(0, 0, 0.05f * gameField.height));
			Gizmos.DrawLine(center - new Vector3(0, 0, 0.5f * gameField.height), center - new Vector3(0, 0, 0.05f * gameField.height));
		}

		private void Awake()
		{
			StarArranger = StarArranger ?? new LinesFirstArranger() { RowHeight = arrangerMinRowHeight };
		}

		#region TODO: Refactor this

		public void ShowBoard(bool randomOrStart)
		{
			BoardState old = current;
			BoardState created = createStart();
			try
			{
				if (randomOrStart)
				{
					created = createRandom();
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning($"createRandom failed with {e.Message}");
			}
			UpdateField(created);
			current = old;
		}

		private BoardState createRandom()
		{
			Dictionary<Pip, int> available = new Dictionary<Pip, int>();
			for (ePipColor color = 0; color <= ePipColor.Yellow; color++)
			{
				for (ePipSize size = 0; size <= ePipSize.Large; size++)
				{
					available[new Pip(color, size)] = 3;
				}
			}

			var rnd = new System.Random();
			byte[] randomBytes = new byte[256];
			int bytesIdx = 0;
			rnd.NextBytes(randomBytes);
			Star[] stars = new Star[2 + randomBytes[bytesIdx++] % 10];
			Ship[] ships = new Ship[4 + randomBytes[bytesIdx++] % 10 + stars.Length];

			Homeworld p1 = new Homeworld(randomPip(available, randomBytes, bytesIdx++), randomPip(available, randomBytes, bytesIdx++), ePlayer.Player1, false);
			Homeworld p2 = new Homeworld(randomPip(available, randomBytes, bytesIdx++), randomPip(available, randomBytes, bytesIdx++), ePlayer.Player2, false);
			for (int i = 0; i < stars.Length; i++)
			{
				stars[i] = new Star(randomPip(available, randomBytes, bytesIdx++));
				ships[i] = new Ship(randomPip(available, randomBytes, bytesIdx++), (ePlayer)(randomBytes[bytesIdx] & 1), stars[i]);
			}
			for (int i = stars.Length; i < ships.Length; i++)
			{
				int locationIdx = randomBytes[bytesIdx++] % (stars.Length + 2);
				IStar location = locationIdx == 0 ? p1 : locationIdx == 1 ? (IStar)p2 : stars[locationIdx - 2];
				ships[i] = new Ship(randomPip(available, randomBytes, bytesIdx++), (ePlayer)(randomBytes[bytesIdx] & 1), location);
			}
			return new BoardState(ships, stars, eBoardLifecycle.Ongoing, p1, p2);
		}

		private Pip lastCreated;
		private Pip randomPip(Dictionary<Pip, int> available, byte[] bytes, int bytesIdx)
		{
			Pip[] pips = available.Keys.ToArray();
			Pip selection = pips[bytes[bytesIdx] % pips.Length];
			if (selection.Color == lastCreated.Color || selection.Size == lastCreated.Size)
			{
				selection = pips[bytes[bytes[bytesIdx]] % pips.Length];
			}
			if (--available[selection] <= 0)
			{
				available.Remove(selection);
			}

			lastCreated = selection;
			return selection;
		}

		private BoardState createStart()
		{
			Homeworld p1 = new Homeworld(ePipColor.Green, ePipSize.Small, ePipColor.Yellow, ePipSize.Medium, ePlayer.Player1, false);
			Homeworld p2 = new Homeworld(ePipColor.Blue, ePipSize.Small, ePipColor.Red, ePipSize.Large, ePlayer.Player2, false);
			Ship p1ship = new Ship(new Pip(ePipColor.Red, ePipSize.Large), ePlayer.Player1, p1);
			Ship p2ship = new Ship(new Pip(ePipColor.Green, ePipSize.Large), ePlayer.Player2, p2);
			return BoardState.CreateInitial(p1, p1ship, p2, p2ship);
		}

		#endregion

		public void UpdateField(BoardState newState)
		{
			destroyChildren();

			current = newState;
			player1HomeworldDesc = createStarDescriptor(newState.Player1Homeworld);
			player2HomeworldDesc = createStarDescriptor(newState.Player2Homeworld);
			foreach (Star star in newState.Stars)
			{
				regularStars.Add(createStarDescriptor(star));
			}

			foreach (Ship ship in newState.Ships)
			{
				GameObject go = Instantiate(store.ShipPrefab, Vector3.zero, Quaternion.identity, transform);
				ShipDescriptor shipDesc = go.GetComponent<ShipDescriptor>();
				shipDesc.Store = store;
				shipDesc.Initialize(ship, findStarDescriptor(ship.Location));
				ships.Add(shipDesc);
			}

			arrangeStars();

			bank.UpdateState(current);
		}

		private void arrangeStars()
		{
			Vector3 center = new Vector3(gameField.center.x, 0, gameField.center.y);
			player1HomeworldDesc.transform.position = transform.position + new Vector3(center.x, center.y, gameField.yMin);
			player2HomeworldDesc.transform.position = transform.position + new Vector3(center.x, center.y, gameField.yMax);
			Rect starsRect = new Rect(gameField.x, gameField.y + arrangerMinRowHeight, gameField.width, gameField.height - arrangerMinRowHeight);
			IEnumerable<ePipSize> allSizes = (ePipSize[])Enum.GetValues(typeof(ePipSize));
			IEnumerable<ePipSize> from = allSizes.Except(player1HomeworldDesc.Star.Attributes.Select(p => p.Size));
			IEnumerable<ePipSize> to = allSizes.Except(player2HomeworldDesc.Star.Attributes.Select(p => p.Size));

			Dictionary<ePipSize, IEnumerable<StarDescriptor>> starGroups = regularStars
				.GroupBy(sd => ((Star)sd.Star).Attributes.Size)
				.ToDictionary(grp => grp.Key, grp => (IEnumerable<StarDescriptor>)grp);

			IEnumerable<StarDescriptor> groupToArrange;
			if (!from.SequenceEqual(to) && from.Count() + to.Count() < 3)
			{
				ePipSize fromSingle = from.Single();
				ePipSize toSingle = to.Single();
				if (starGroups.TryGetValue(fromSingle, out groupToArrange))
				{
					Vector2 fromSize = StarArranger.CalculateBounds(starsRect, groupToArrange);
					StarArranger.ArrangeStars(new Rect(starsRect.center.x - 0.5f * fromSize.x, starsRect.y, fromSize.x, fromSize.y), groupToArrange);
					starsRect.y += fromSize.y;
					starsRect.height -= fromSize.y;
					starGroups.Remove(fromSingle);
				}
				if (starGroups.TryGetValue(toSingle, out groupToArrange))
				{
					Vector2 toSize = StarArranger.CalculateBounds(starsRect, groupToArrange);
					StarArranger.ArrangeStars(new Rect(starsRect.center.x - 0.5f * toSize.x, starsRect.yMax - toSize.y, toSize.x, toSize.y), groupToArrange);
					starsRect.height -= toSize.y;
					starGroups.Remove(toSingle);
				}
			}
			if (starGroups.Count > 0)
			{
				groupToArrange = starGroups.Values.Aggregate((a, b) => a.Concat(b));
				Vector2 remainderSize = StarArranger.CalculateBounds(starsRect, groupToArrange);
				StarArranger.ArrangeStars(new Rect(starsRect.center - 0.5f * remainderSize, remainderSize), groupToArrange);
			}
		}

		private StarDescriptor createStarDescriptor(IStar star)
		{
			GameObject go = Instantiate(store.StarPrefab, Vector3.zero, Quaternion.identity, transform);
			StarDescriptor starDesc = go.GetComponent<StarDescriptor>();
			starDesc.Store = store;
			starDesc.Initialize(star);
			return starDesc;
		}

		private StarDescriptor findStarDescriptor(IStar location)
		{
			StarDescriptor result = null;
			GenericStarVisitor visitor = new GenericStarVisitor(
				s => result = result = regularStars.Find(sd => sd.Star.Identifier == location.Identifier),
				hw => result = hw.Owner == ePlayer.Player1 ? player1HomeworldDesc : player2HomeworldDesc
			);
			location.Accept(visitor);

			return result;
		}

#if UNITY_EDITOR
		private void destroyChildren()
		{
			regularStars.Clear();
			ships.Clear();
			Transform[] children = transform.Cast<Transform>().ToArray();
			foreach (Transform child in children)
			{
				DestroyImmediate(child.gameObject);
			}
		}
#else
		private void destroyChildren()
		{
			regularStars.Clear();
			ships.Clear();
			Transform[] children = transform.Cast<Transform>().ToArray();
			foreach (Transform child in transform)
			{
				Destroy(child.gameObject);
			}
		}
#endif
	}
}