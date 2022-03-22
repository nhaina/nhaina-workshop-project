using System;

namespace Homeworlds.Logic
{
	public class GenericStarVisitor : IAbstractStarVisitor, Star.IConcreteStarVisitor, Homeworld.IHomeWorldVisitor
	{
		public event Action DefaultVisiting;
		public event Action<Star> VisitingStar;
		public event Action<Homeworld> VisitingHomeworld;

		public GenericStarVisitor()
		{ }

		public GenericStarVisitor(Action<Star> i_OnVisitingStar, Action<Homeworld> i_OnVisitingHomeworld)
		{
			VisitingStar += i_OnVisitingStar;
			VisitingHomeworld += i_OnVisitingHomeworld;
		}

		public void Visit()
		{
			DefaultVisiting?.Invoke();
		}

		public void Visit(Star star)
		{
			VisitingStar?.Invoke(star);
		}

		public void Visit(Homeworld homeworld)
		{
			VisitingHomeworld?.Invoke(homeworld);
		}
	}
}
