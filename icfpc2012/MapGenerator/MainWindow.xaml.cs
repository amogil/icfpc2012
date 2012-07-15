using System;
using System.Windows;

namespace MapGenerator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			var options = new MapGeneratorOptions(height: Convert.ToInt32(tbHeights.Text),
			                                      width: Convert.ToInt32(tbWidth.Text),
			                                      hasLift: cbLift.IsChecked ?? false,
			                                      rockCount: Convert.ToInt32(tbRocks.Text),
			                                      earthCount: Convert.ToInt32(tbEarth.Text),
			                                      wallCount: Convert.ToInt32(tbWalls.Text),
			                                      lambdaCount: Convert.ToInt32(tbLambdas.Text),
			                                      waterLevel: Convert.ToInt32(tbWater.Text),
			                                      flooding: Convert.ToInt32(tbFlooding.Text),
			                                      waterproof: Convert.ToInt32(tbWaterproof.Text),
			                                      trampolineCount: Convert.ToInt32(tbTrampolineCount.Text),
			                                      beardCount: Convert.ToInt32(tbBeards.Text),
			                                      beardGrowth: Convert.ToInt32(tbBeardGrow.Text),
			                                      mapRazorCount: Convert.ToInt32(tbMapRazor.Text),
			                                      pocketRazorCount: Convert.ToInt32(tbPocketRazor.Text),
			                                      highRockCount: Convert.ToInt32(tbHighRocks.Text));
			var generator = cbIsolatedSegments.IsChecked.Value
			                	? new IsolatedMapGenerator(options)
			                	: new SmartWallsMapGenerator(options);
			var map = generator.Generate();
			tbResult.Text = map;
			Clipboard.SetText(map);
		}
	}
}