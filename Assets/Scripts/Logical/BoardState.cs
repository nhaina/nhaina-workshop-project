using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Homeworlds.Logical
{
	public class BoardState
	{
		private readonly ResourcesState r_Bank;
		private readonly HashSet<Star> r_Stars;
		private readonly ePlayer r_ActivePlayer;
		private readonly HomeWorld r_Player1HomeWorld;
		private readonly HomeWorld r_Player2HomeWorld;

		public BoardState()
			: this(ResourcesState.Full, null,
				  ePlayer.Player1)
		{
		}

		public BoardState(ResourcesState i_BankState, IEnumerable<Star> i_Stars,
			ePlayer i_ActivePlayer)
		{
			r_Bank = i_BankState;
			r_Stars = new HashSet<Star>();
			r_Player1HomeWorld = r_Player2HomeWorld = null;
			if (i_Stars != null)
			{
				r_Stars.UnionWith(i_Stars);
				r_Player1HomeWorld = i_Stars.FirstOrDefault(s => s is HomeWorld && ((HomeWorld)s).Owner == ePlayer.Player1) as HomeWorld;
				r_Player1HomeWorld = i_Stars.FirstOrDefault(s => s is HomeWorld && ((HomeWorld)s).Owner == ePlayer.Player2) as HomeWorld;
			}
			r_ActivePlayer = i_ActivePlayer;
		}

		public ResourcesState Bank
		{
			get
			{
				return r_Bank;
			}
		}

		public IEnumerable<Star> Stars
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
				return r_Player1HomeWorld;
			}
		}

		public HomeWorld Player2Star
		{
			get
			{
				return r_Player2HomeWorld;
			}
		}
	}

}