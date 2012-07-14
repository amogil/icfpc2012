using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Logic
{
	public abstract class RobotAI
	{
		protected RobotAI(Map map)
		{
			Map = map;
		}

		protected Map Map { get; set; }

		public static Type[] GetAllRobotsTypes()
		{
			return typeof (RobotAI).Assembly.GetTypes().Where(IsRobotClass).ToArray();
		}

		private static bool IsRobotClass(Type type)
		{
			if (type.IsAbstract || !typeof (RobotAI).IsAssignableFrom(type)) return false;
			ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] {typeof (Map)}, null);
			return constructorInfo != null;
		}

		public static RobotAI Create(Type robotType, Map map)
		{
			ConstructorInfo constructorInfo = robotType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] {typeof (Map)}, null);
			return (RobotAI) constructorInfo.Invoke(new object[] {map});
		}

		public abstract IEnumerable<RobotMove> GetMoves();
	}

	public class StupidRobot : RobotAI
	{
		public StupidRobot(Map map) : base(map)
		{
		}

		public override IEnumerable<RobotMove> GetMoves()
		{
			while (true)
			{
				foreach (
					RobotMove robotMove in
						new[] {RobotMove.Down, RobotMove.Left, RobotMove.Right, RobotMove.Up, RobotMove.Wait, })
					yield return robotMove;
			}
		}
	}
}