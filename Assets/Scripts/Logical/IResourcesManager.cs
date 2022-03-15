using System;
using System.Collections.Generic;

namespace Homeworlds.Logical
{
	public interface IResourcesManager
	{
		IBoardManager BoardManager { get; set; }
		bool TryRemovePip(Pip i_Attributes);
		bool TryAddPip(Pip i_Attributes);
		void AddPips(Dictionary<Pip, int> i_PipsAttributes);
		ResourcesState CreateResourcesState(Dictionary<Pip, int> i_Values);
		int GetCurrentCount(Pip i_Attributes);
	}
}