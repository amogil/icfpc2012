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
				LastOpenedMapFile = openDialog.FileName;
			}
		}

		private void UpdateMap(Map newMap)
		{
			this.map = newMap;
			bitmap = new Bitmap(map.Width*CellSize, map.Height*CellSize);
			for (int x = 1; x < map.Width-1; x++)
				for (int y = 1; y < map.Height-1; y++)
					UpdateCell(x, y);
			picture.Image = bitmap;
		}

		private void UpdateCell(int x, int y)
		{
			Graphics g = Graphics.FromImage(bitmap);
			g.DrawImage(CellImages.Bitmaps[map[x, y]], (x-1)*CellSize, (map.Height - y-1-1)*CellSize, CellSize, CellSize);
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			var mapFile = LastOpenedMapFile;
			if (mapFile != null)
				UpdateMap(new Map(File.ReadAllLines(mapFile)));
			zoomBar.Value = CellSize;
		}

		public string LastOpenedMapFile
		{
			get
			{
				if (File.Exists("lastopenedMap"))
				{
					var file = File.ReadAllText("lastopenedMap");
					if (File.Exists(file)) return file;
				}
				return null;
			}
			set
			{
				File.WriteAllText("lastopenedMap", value);
			}
		}

		private void zoomBar_ValueChanged(object sender, EventArgs e)
		{
			CellSize = zoomBar.Value;
			UpdateMap(map);
		}
	}
}