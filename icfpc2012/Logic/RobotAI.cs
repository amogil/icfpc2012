using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Logic
{
	public abstract class RobotAI
	{
		public static Type[] GetAllRobotsTypes()
		{
			return typeof (RobotAI).Assembly.GetTypes().Where(IsRobotClass).ToArray();
		}

		private static bool IsRobotClass(Type type)
		{
			if (type.IsAbstract || !typeof (RobotAI).IsAssignableFrom(type)) return false;
			ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
			return constructorInfo != null;
		}

		public static RobotAI Create(Type robotType, Map map)
		{
			ConstructorInfo constructorInfo = robotType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
			return (RobotAI) constructorInfo.Invoke(new object[0]);
		}

		public abstract RobotMove NextMove(Map map);
		public bool StopNow { get; set; }
	}

	public class FixedProgramRobot : RobotAI
	{
		private int index = 0;
		private readonly RobotMove[] moves;

		public FixedProgramRobot(params RobotMove[] moves)
		{
			this.moves = moves;
		}

		public override RobotMove NextMove(Map map)
		{
			return moves[index++];
		}
	}

	public class StupidRobot : RobotAI
	{
		private RobotMove[] moves = new[] {RobotMove.Down, RobotMove.Left, RobotMove.Right, RobotMove.Up, RobotMove.Wait,};
		private int index = 0;

		public override RobotMove NextMove(Map map)
		{
			return moves[(index++)%moves.Length];
		}
	}
}