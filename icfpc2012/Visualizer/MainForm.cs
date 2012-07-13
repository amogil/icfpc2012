using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Logic;
using System.Linq;

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
				LoadMap(openDialog.FileName);
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
			UpdateMoves();
		}

		private void UpdateMoves()
		{
			movesBox.Text = GetMovesString();
		}

		private string GetMovesString()
		{
			return new string(moves.Select(m => m.ToChar()).ToArray());
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
				LoadMap(mapFile);
			zoomBar.Value = CellSize;
		}

		private void LoadMap(string mapFile)
		{
			moves.Clear();
			UpdateMap(new Map(File.ReadAllLines(mapFile)));
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
		    picture.Focus();
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Left) DoMove(RobotMove.Left);
			if (e.KeyCode == Keys.Right) DoMove(RobotMove.Right);
			if (e.KeyCode == Keys.Up) DoMove(RobotMove.Up);
			if (e.KeyCode == Keys.Down) DoMove(RobotMove.Down);
			if (e.KeyCode == Keys.Space) DoMove(RobotMove.Wait);
		}

		private void DoMove(RobotMove robotMove)
		{
			if (map.State != CheckResult.Nothing) return;
			Map newMap;
			try
			{
				newMap = map.Move(robotMove);
			}
			catch (GameFinishedException)
			{
				moves.Add(robotMove);
				UpdateMap(map);
				SaveMoves();
				return;
			}
			catch (NoMoveException)
			{
				return;
			}
			moves.Add(robotMove);
			UpdateMap(newMap);
		}

		private void SaveMoves()
		{
			string directoryName = Path.GetDirectoryName(Path.GetFullPath(LastOpenedMapFile));
			Debug.Assert(directoryName != null);
			string movesFile = 
				Path.Combine(
				directoryName,
				Path.GetFileNameWithoutExtension(LastOpenedMapFile) + "_" + DateTime.Now.Ticks + ".moves");
			File.WriteAllText(movesFile, GetMovesString() + Environment.NewLine + map);
			MessageBox.Show("Moves saved to " + movesFile);

		}

		private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadMap(LastOpenedMapFile);
		}

		private readonly List<RobotMove> moves = new List<RobotMove>();
	}
}