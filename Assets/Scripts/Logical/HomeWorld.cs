namespace Homeworlds.Logical
{
	public class HomeWorld : Star
	{
		public ePlayer Owner { get; set; }
		public Pip? SecondaryAttributes { get; set; }

		public HomeWorld()
			: this(Pip.SmallRed, null, ePlayer.Player1)
		{ }

		public HomeWorld(Pip i_MainAttributes, Pip? i_SecondaryAttributes, ePlayer i_Owner)
			:base(i_MainAttributes)
		{
			Owner = i_Owner;
			SecondaryAttributes = i_SecondaryAttributes;
		}

		public override int GetColorCount(ePipColor i_Color)
		{
			int count = base.GetColorCount(i_Color);
			if (SecondaryAttributes.HasValue && i_Color == SecondaryAttributes.Value.Color)
			{
				count++;
			}
			return count;
		}
	}
}