using System.Collections;
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
		[SerializeField]
		private ViewBoardPrefabStore store;
		[SerializeField]
		private BankDescriptor bank;
		[SerializeField]
		private Rect gameField;
		public float arrangerMinRowHeight = 1.4f;
		public IStarArranger StarArranger { get; set; }

		public Action<ISelectable> SelectStarCallback { get; set; }
		public Action<ISelectable> SelectShipCallback { get; set; }

		private readonly List<StarDescriptor> regularStars = new List<StarDescriptor>();
		private readonly List<ShipDescriptor> ships = new List<ShipDescriptor>();
		StarDescriptor player1HomeworldDesc, player2HomeworldDesc;

		private void OnDrawGizmos()
		{
			Vector3 center, size;
			center = transform.TransformPoint(new Vector3(gameField.center.x, 0, gameField.center.y));
			size = transform.TransformVector(new Vector3(gameField.width, 0f, gameField.height));
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(center, size);
			Gizmos.DrawLine(center + new Vector3(0, 0, 0.5f * gameField.height), center + new Vector3(0, 0, 0.05f * gameField.height));
			Gizmos.DrawLine(center - new Vector3(0, 0, 0.5f * gameField.height), center - new Vector3(0, 0, 0.05f * gameField.height));
		}

		private void Awake()
		{
			StarArranger = StarArranger ?? new LinesFirstArranger() { RowHeight = arrangerMinRowHeight };
		}

		public void UpdateField(BoardState newState, Dictionary<Pip, int> bankState)
		{
			destroyChildren();

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
				shipDesc.Selected += SelectShipCallback;
				ships.Add(shipDesc);
			}

			arrangeStars();

			bank.UpdateState(bankState);
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
			starDesc.Selected += SelectStarCallback;
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