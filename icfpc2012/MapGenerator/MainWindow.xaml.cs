using System;
using System.Windows;

namespace MapGenerator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, RoutedEventArgs e)
		{
			var mg = new Generator(height: Convert.ToInt32(tbHeights.Text),
			                       width: Convert.ToInt32(tbWidth.Text),
			                       hasLift: cbLift.IsChecked ?? false,
			                       rocksCount: Convert.ToInt32(tbRocks.Text),
			                       earthCount: Convert.ToInt32(tbEarth.Text),
			                       wallCount: Convert.ToInt32(tbWalls.Text),
			                       lambdaCount: Convert.ToInt32(tbLambdas.Text));
			var map = mg.Generate();
			tbResult.Text = map;
		}
	}
}