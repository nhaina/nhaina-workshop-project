using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Homeworlds.Logical
{
	public class BoardState
	{
		private readonly ResourcesState r_Bank;
		private readonly List<Star> r_Stars;
		private readonly ePlayer r_ActivePlayer;
		private readonly int r_StepNumber;

		public BoardState()
			: this(ResourcesState.MaxState, new Star[2],
				  ePlayer.Player1, 0)
		{
		}

		public BoardState(ResourcesState i_BankState, IEnumerable<Star> i_Stars,
			ePlayer i_ActivePlayer, int i_StepNumber)
		{
			r_Bank = i_BankState;
			r_Stars = new List<Star>(i_Stars);
			r_ActivePlayer = i_ActivePlayer;
			r_StepNumber = i_StepNumber;
		}

		public ResourcesState Bank
		{
			get
			{
				return r_Bank;
			}
		}

		public List<Star> Stars
		{
			get
			{
				return r_Stars;
			}
		}

		public ePlayer ActivePlayer
		{
			get
			{
				return r_ActivePlayer;
			}
		}

		public HomeWorld Player1Star
		{
			get
			{
				return r_Stars.Count > 0 ? (HomeWorld)r_Stars[0] : null;
			}
		}

		public HomeWorld Player2Star
		{
			get
			{
				return r_Stars.Count > 1 ? (HomeWorld)r_Stars[1] : null;
			}
		}

		public int StepNumber
		{
			get
			{
				return r_StepNumber;
			}
		}
	}

}