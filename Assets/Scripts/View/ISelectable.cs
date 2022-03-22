using System;

namespace Homeworlds.View
{
	public interface ISelectable
	{
		event Action<ISelectable> Selected;
		void Select();
	}
}