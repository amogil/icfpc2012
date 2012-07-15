using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Logic;
using NUnit.Framework;

namespace Visualizer
{
	[TestFixture]
	public class CellImages_Test
	{
		[Test]
		[Explicit]
		public void GenerateImages()
		{
			var font = new Font("Arial", 24, FontStyle.Bold);
			int i = 1;
			for (MapCell c = MapCell.Trampoline1; c <= MapCell.Trampoline9; c++)
			{
				var bitmap = new Bitmap(48, 48, PixelFormat.Format24bppRgb);
				Graphics g = Graphics.FromImage(bitmap);
				g.FillRectangle(Brushes.Black, 0, 0, 48, 48);
				string s = "" + (char) c;
				g.DrawString(s, font, Brushes.White, 5, 5);
				bitmap.Save(@"..\..\images\trampoline" + i + ".bmp");
				i++;
			}
			i = 1;
			for (MapCell c = MapCell.Target1; c <= MapCell.Target9; c++)
			{
				var bitmap = new Bitmap(48, 48, PixelFormat.Format24bppRgb);
				Graphics g = Graphics.FromImage(bitmap);
				g.FillRectangle(Brushes.Black, 0, 0, 48, 48);
				string s = "" + (char) c;
				g.DrawString(s, font, Brushes.White, 5, 5);
				bitmap.Save(@"..\..\images\target" + i + ".bmp");
				i++;
			}
		}
	}

	public class CellImages
	{
		public static readonly IDictionary<MapCell, Bitmap> Bitmaps;

		public static readonly IDictionary<MapCell, Brush> CellBrushes =
			new Dictionary<MapCell, Brush>
				{
					{MapCell.ClosedLift, Brushes.Gray},
					{MapCell.OpenedLift, Brushes.Gold},
					{MapCell.Lambda, Brushes.GreenYellow},
					{MapCell.Robot, Brushes.Magenta},
					{MapCell.Beard, Brushes.Red},
					{MapCell.Razor, Brushes.Blue},
					{MapCell.Earth, Brushes.Brown},
					{MapCell.Empty, Brushes.Black},
					{MapCell.Rock, Brushes.BlueViolet},
					{MapCell.Wall, Brushes.White},
					{MapCell.LambdaRock, Brushes.DarkGreen}
				};

		static CellImages()
		{
			Bitmaps = Enum.GetNames(typeof (MapCell)).Select(name => new {bmp = LoadImage(name), name})
				.ToDictionary(
					namedBmp => (MapCell) Enum.Parse(typeof (MapCell), namedBmp.name),
					namedBmp => namedBmp.bmp);
			for (MapCell c = MapCell.Trampoline1; c <= MapCell.Trampoline9; c++)
				CellBrushes.Add(c, Brushes.Orange);
			for (MapCell c = MapCell.Target1; c <= MapCell.Target9; c++)
				CellBrushes.Add(c, Brushes.DeepSkyBlue);
		}

		private static Bitmap LoadImage(string name)
		{
			try
			{
				return new Bitmap("images\\" + name + ".bmp");
			}
			catch (Exception e)
			{
				throw new Exception(name, e);
			}
		}
	}
}