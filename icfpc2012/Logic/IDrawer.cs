using Logic;

namespace Visualizer
{
	public interface IDrawer
	{
		void AddStyle(string style, string color);
		void Line(string style, Vector fromCell, Vector toCell);
		void Text(string style, Vector cell, string text);
		void Dot(string style, Vector cell);
	}
}