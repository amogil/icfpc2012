using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Logic;

namespace Visualizer
{
	public partial class MainForm : Form
	{
		private int CellSize = 48;
		private Bitmap bitmap;
		private Map map;

		public MainForm()
		{
			InitializeComponent();
		}

		private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var openDialog = new OpenFileDialog();
			if (openDialog.ShowDialog(this) == DialogResult.OK)
			{
				UpdateMap(new Map(File.ReadAllLines(openDialog.FileName)));
			}
		}

		private void UpdateMap(Map newMap)
		{
			this.map = newMap;
			bitmap = new Bitmap(map.Width*CellSize, map.Height*CellSize);
			for (int x = 0; x < map.Width; x++)
				for (int y = 0; y < map.Height; y++)
				{
					UpdateCell(x, y);
				}
			picture.Image = bitmap;
		}

		private void UpdateCell(int x, int y)
		{
			Graphics g = Graphics.FromImage(bitmap);
			g.DrawImage(CellImages.Bitmaps[map[x, y]], x*CellSize, y*CellSize, CellSize, CellSize);
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			UpdateMap(new Map(new string[0]));
		}
	}
}