using System.Collections.Generic;

namespace Logic
{
	public static class StackExtensions
	{
		public static T PeekAndPop<T>(this Stack<T> stack)
		{
			T res = stack.Peek();
			stack.Pop();
			return res;
		}
	}
}
