namespace Homeworlds.Logic
{
	public interface IAbstractStarVisitor
	{
		/// <summary>
		/// default implementation.
		/// Trying to avoid mutual dependecy
		/// </summary>
		void Visit(); 
	}
}