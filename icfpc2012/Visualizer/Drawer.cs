using System.Collections.Generic;
using System.Drawing;
using Logic;

namespace Visualizer
{
	public class Drawer
	{
		private class StyleSettings
		{
			public Pen Pen;

			public Brush Brush
			{
				get { return new SolidBrush(Pen.Color); }
			}
		}
		private readonly Graphics g;
		private readonly int cellSize;
		private readonly int width;
		private readonly int height;
		private IDictionary<string, StyleSettings> styles = new Dictionary<string, StyleSettings>();

		private PointF Convert(Vector v, string style)
		{
			// TODO add micro offsets different for each style
			int newX = (v.X-1)*cellSize + 2*cellSize/3;
			int newY = height*cellSize - ((v.Y+1)*cellSize + 2*cellSize/3);
			return new PointF(newX, newY);
		}

		public Drawer(Graphics g, int cellSize, int width, int height)
		{
			this.g = g;
			this.cellSize = cellSize;
			this.width = width;
			this.height = height;
		}

		public void AddStyle(string style, Pen pen)
		{
			styles.Add(style, new StyleSettings{Pen = pen});
		}

		public void Line(string style, Vector fromCell, Vector toCell)
		{
			g.DrawLine(styles[style].Pen, Convert(fromCell, style), Convert(toCell, style));
		}

		public void Text(string style, Vector cell, string text)
		{
			g.DrawString(text, new Font("Arial", 6), Brushes.Black, Convert(cell, style));
		}

		public void Dot(string style, Vector cell)
		{
			var center = Convert(cell, style);
			var radius = cellSize/10.0f;
			g.FillEllipse(styles[style].Brush, center.X - radius, center.Y - radius, 2*radius, 2*radius);

		}
	}
}