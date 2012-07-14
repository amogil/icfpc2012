using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
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
		private IOverlay[] overlays;

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			InitOverlays();
			var mapFile = LastOpenedMapFile;
			if (mapFile != null)
				LoadMap(mapFile);
			zoomBar.Value = CellSize;
			InitRobots();
		}

		private void InitOverlays()
		{
			overlays = new IOverlay[] {new ToLambdasOverlay(),};
			foreach (var overlay in overlays)
			{
				var item = new ToolStripMenuItem(overlay.GetType().Name, null, (sender, args) => UpdateMap(map));
				item.Tag = overlay;
				item.CheckOnClick = true;
				overlaysToolStripMenuItem.DropDownItems.Add(item);
			}
		}

		private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var openDialog = new OpenFileDialog();
			openDialog.InitialDirectory = @"../../../maps";
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
			Graphics g = Graphics.FromImage(bitmap);
			for (int x = 1; x < map.Width - 1; x++)
				for (int y = 1; y < map.Height-1; y++)
					UpdateCell(g, x, y);
			DrawOverlays(g);
			picture.Image = bitmap;
			UpdateMoves();
			UpdateInfoPanel();
		}

		private void UpdateInfoPanel()
		{
			waterproofLabel.Text = map.WaterproofLeft.ToString(CultureInfo.InvariantCulture);
			scoreLabel.Text = map.Score.ToString(CultureInfo.InvariantCulture);
		}

		private void UpdateMoves()
		{
			movesBox.Text = GetMovesString();
		}

		private string GetMovesString()
		{
			return new string(moves.Select(m => m.ToChar()).ToArray());
		}

		private void UpdateCell(Graphics g, int x, int y)
		{
			var rect = new Rectangle((x-1)*CellSize, (map.Height - y-1-1)*CellSize, CellSize, CellSize);
			g.DrawImage(CellImages.Bitmaps[map[x, y]], rect);
			if (map.Water >= y)
				g.FillRectangle(new SolidBrush(Color.FromArgb(150, 0, 0, 255)), rect);
			if (map.Flooding > 0 && map.StepsToIncreaseWater == 1 && y == map.Water+1)
				g.FillRectangle(new SolidBrush(Color.FromArgb(50, 0, 0, 255)), rect);
		}

		private void DrawOverlays(Graphics graphics)
		{
			var drawer = new Drawer(graphics, CellSize, map.Width, map.Height);
			var selectedOverlays = 
				overlaysToolStripMenuItem.DropDownItems.Cast<ToolStripMenuItem>()
				.Where(item => item.Checked).Select(item => item.Tag)
				.Cast<IOverlay>();
			foreach (var overlay in selectedOverlays)
			{
				overlay.Draw(map, drawer);
			}
		}



		private void LoadMap(string mapFile)
		{
			moves.Clear();
			UpdateMap(new Map(File.ReadAllLines(mapFile)));
			Text = mapFile;
		}

		private void InitRobots()
		{
			Type[] robotTypes = RobotAI.GetAllRobotsTypes();
			foreach (var robotType in robotTypes.OrderBy(t => t.Name))
			{
				var rt = robotType;
				var robotItem = new ToolStripMenuItem(robotType.Name, null, (sender, args) => RunRobot(rt));
				robotToolStripMenuItem.DropDownItems.Add(robotItem);
			}
		}

		private void RunRobot(Type robotType)
		{
			LoadMap(LastOpenedMapFile);
			robot = RobotAI.Create(robotType, map).GetMoves().GetEnumerator();
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
			if (e.KeyCode == Keys.Enter)
			{
				if (robot != null)
				{
					DoMove(robot.Current);
					if (!robot.MoveNext())
						robot = null;
				}
			}
		}

		private void DoMove(RobotMove robotMove)
		{
			if (map.State != CheckResult.Nothing) return;
			Map newMap;
			try
			{
				newMap = map.Move(robotMove);
				if (newMap.State != CheckResult.Nothing)
					throw new GameFinishedException();
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
				moves.Add(robotMove);
				UpdateMap(map);
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
				Path.GetFileNameWithoutExtension(LastOpenedMapFile) + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff") + ".moves");
			File.WriteAllText(movesFile,
				GetMovesString()
				+ Environment.NewLine
				+ string.Format("LambdasGathered: {0}", map.LambdasGathered)
				+ Environment.NewLine
				+ string.Format("Score: {0}", map.Score)
				+ Environment.NewLine
				+ map);
			MessageBox.Show("Moves saved to " + movesFile);
		}

		private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadMap(LastOpenedMapFile);
		}

		private readonly List<RobotMove> moves = new List<RobotMove>();
		private IEnumerator<RobotMove> robot;
	}
}