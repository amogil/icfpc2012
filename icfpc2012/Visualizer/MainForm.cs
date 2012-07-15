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
		private Tuple<int, string> bestPath = new Tuple<int, string>(0, "A");

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
			if (newMap == null) return;
			this.map = newMap;
			if (bitmap == null || map.Width*CellSize != bitmap.Width || map.Height*CellSize != bitmap.Height)
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
			scoreLabel.Text = map.GetScore().ToString(CultureInfo.InvariantCulture);
			movesLabel.Text = map.MovesCount.ToString(CultureInfo.InvariantCulture);
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
			if (CellSize > 6)
				g.DrawImage(CellImages.Bitmaps[map[x, y]], rect);
			else
				g.FillRectangle(CellImages.CellBrushes[map[x,y]], rect);
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
			bestPath = new Tuple<int, string>(0, "A");
			var newMap = new Map(File.ReadAllLines(mapFile));
			engine = new Engine(newMap);
			engine.OnMapUpdate += UpdateMap;
			engine.OnMoveAdded += m =>
			                      	{
			                      		moves.Add(m);
										if (engine.Map.State != CheckResult.Nothing)
											CheckState();
			                      		else
										{
											engine.Map.Move(RobotMove.Abort);
											CheckState();
											engine.Map.Rollback();
										}
			                      	};
			if (newMap.Width * newMap.Height > 150 * 150 && CellSize > 2) zoomBar.Value = 2;
			if (newMap.Width * newMap.Height > 50 * 50 && CellSize > 10) zoomBar.Value = 10;
			UpdateMap(newMap);
			Text = mapFile;
			robot = null;
		}

		private void CheckState()
		{
			if (engine.Map.GetScore() > bestPath.Item1)
				bestPath = new Tuple<int, string>(bestPath.Item1, GetMovesString() + "A");
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

			var fixedProgramItem = new ToolStripMenuItem("Program from clipboard", null, (sender, args) =>
			{
				LoadMap(LastOpenedMapFile);
				robot = new FixedProgramRobot(Clipboard.GetText().Select(c => c.ToRobotMove()).ToArray());
			});
			robotToolStripMenuItem.DropDownItems.Add(fixedProgramItem);
		}

		private void RunRobot(Type robotType)
		{
			LoadMap(LastOpenedMapFile);
			robot = RobotAI.Create(robotType, map);
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
					DoMove(robot.NextMove(map));
			}
			if (e.KeyCode == Keys.Back)
			{
				if (robot == null) RollbackMove();

			}
			//Application.DoEvents();
		}

		private void RollbackMove()
		{
			if (map.Rollback())
			{
				moves.RemoveAt(moves.Count - 1);
				UpdateMap(map);
			}
		}

		private void DoMove(RobotMove robotMove)
		{
			try
			{
				engine.DoMove(robotMove);
			}
			catch (GameFinishedException)
			{
				SaveMoves();
			}
		}

		private void SaveMoves()
		{
			string directoryName = Path.GetDirectoryName(Path.GetFullPath(LastOpenedMapFile));
			Debug.Assert(directoryName != null);
			string movesFile = 
				Path.Combine(
				directoryName,
				Path.GetFileNameWithoutExtension(LastOpenedMapFile) + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ffff") + ".moves");

			while (engine.Map.MovesCount > bestPath.Item2.Length - 1)
			{
				engine.Map.Rollback();
			}
			engine.Map.Move(RobotMove.Abort);

			File.WriteAllText(movesFile,
				GetMovesString()
				+ Environment.NewLine
				+ string.Format("LambdasGathered: {0}", map.LambdasGathered)
				+ Environment.NewLine
				+ string.Format("Score: {0}", map.GetScore())
				+ Environment.NewLine
				+ map);
			MessageBox.Show("Moves saved to " + movesFile);
		}

		private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LoadMap(LastOpenedMapFile);
		}

		private readonly List<RobotMove> moves = new List<RobotMove>();
		private RobotAI robot;
		private Engine engine;
	}
}