using System;
using System.Collections;
using System.Collections.Generic;

namespace Visualizer
{
	public class RollbackableEnumerator<T> : IEnumerator<T>
	{
		private readonly IEnumerator<T> enumerator;
		private readonly List<T> history = new List<T>();
		private int inHistoryIndex;

		public RollbackableEnumerator(IEnumerator<T> enumerator)
		{
			this.enumerator = enumerator;
		}

		#region IEnumerator<T> Members

		public void Dispose()
		{
			enumerator.Dispose();
		}

		public bool MoveNext()
		{
			if (inHistoryIndex + 1 < history.Count)
			{
				inHistoryIndex++;
				return true;
			}
			if (!enumerator.MoveNext()) return false;
			history.Add(enumerator.Current);
			inHistoryIndex = history.Count - 1;
			return true;
		}

		public void Reset()
		{
			history.Clear();
			inHistoryIndex = 0;
			enumerator.Reset();
		}

		public T Current
		{
			get { return history[inHistoryIndex]; }
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		#endregion

		public void Rollback()
		{
			if (inHistoryIndex == 0) throw new InvalidOperationException();
			inHistoryIndex--;
		}
	}
}