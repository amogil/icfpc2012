using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Logic;

namespace Visualizer
{
	public class CellImages
	{
		public static readonly IDictionary<MapCell, Bitmap> Bitmaps;

		static CellImages()
		{
			Bitmaps = Enum.GetNames(typeof (MapCell)).Select(name => new {bmp = LoadImage(name), name})
				.ToDictionary(
					namedBmp => (MapCell) Enum.Parse(typeof (MapCell), namedBmp.name),
					namedBmp => namedBmp.bmp);
		}

		private static Bitmap LoadImage(string name)
		{
			try
			{
				return new Bitmap(name + ".bmp");
			}catch(Exception e)
			{
				throw new Exception(name, e);
			}
		}
	}
}