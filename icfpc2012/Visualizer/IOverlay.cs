using Logic;

namespace Visualizer
{
	public interface IOverlay
	{
		void Draw(IMap map, Drawer drawer);
	}
}