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
			var mg = new Generator(Convert.ToInt32(tbHeights.Text), Convert.ToInt32(tbWidth.Text));
			var map = mg.Generate();
			tbResult.Text = map;
		}
	}
}