namespace Homeworlds.Logical
{
	public readonly struct Pip
	{
		public static readonly Pip Empty = new Pip();
		public readonly ePipColor Color;
		public readonly ePipSize Size;

		public Pip(ePipColor i_Color, ePipSize i_Size)
		{
			Color = i_Color;
			Size = i_Size;
		}
	}
}