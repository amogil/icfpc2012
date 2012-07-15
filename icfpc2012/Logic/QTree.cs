using NUnit.Framework;

namespace Logic
{
	public class QTree
	{
		private MapCell data;
		private QTree[] children;

		public static QTree Set(QTree root, int x, int y, int leftX, int rightX, int leftY, int rightY, MapCell mapCell)
		{
			QTree newRoot = new QTree();
			if (leftX == rightX && leftY == rightY)
			{
				newRoot.data = mapCell;
				return newRoot;
			}
			int middleX = (leftX + rightX) / 2;
			int middleY = (leftY + rightY) / 2;
			int childId = (x > middleX ? 1 : 0);
			if (y > middleY) childId |= 2;
			newRoot.children = new QTree[4];
			for (int i = 0; i < 4; ++i)
				newRoot.children[i] = root.children[i];

			if (y <= middleY)
			{
				if (x <= middleX)
					newRoot.children[childId] = Set(root.children[childId], x, y, leftX, middleX, leftY, middleY, mapCell);
				else
					newRoot.children[childId] = Set(root.children[childId], x, y, middleX + 1, rightX, leftY, middleY, mapCell);
			}
			else
			{
				if (x <= middleX)
					newRoot.children[childId] = Set(root.children[childId], x, y, leftX, middleX, middleY + 1, rightY, mapCell);
				else
					newRoot.children[childId] = Set(root.children[childId], x, y, middleX + 1, rightX, middleY + 1, rightY, mapCell);
			}
			return newRoot;
		}

		public static MapCell Get(QTree root, int x, int y, int leftX, int rightX, int leftY, int rightY)
		{
			if (leftX == rightX && leftY == rightY)
				return root.data;
			int middleX = (leftX + rightX) / 2;
			int middleY = (leftY + rightY) / 2;
			int childId = (x > middleX ? 1 : 0);
			if (y > middleY) childId |= 2;
			if (y <= middleY)
			{
				if (x <= middleX)
					return Get(root.children[childId], x, y, leftX, middleX, leftY, middleY);
				return Get(root.children[childId], x, y, middleX + 1, rightX, leftY, middleY);
			}
			if (x <= middleX)
				return Get(root.children[childId], x, y, leftX, middleX, middleY + 1, rightY);
			return Get(root.children[childId], x, y, middleX + 1, rightX, middleY + 1, rightY);
		}

		public static void SimpleAdd(QTree root, int x, int y, int leftX, int rightX, int leftY, int rightY, MapCell mapCell)
		{
			if (leftX == rightX && leftY == rightY)
			{
				root.data = mapCell;
				return;
			}
			if (root.children == null)
				root.children = new QTree[4];
			int middleX = (leftX + rightX)/2;
			int middleY = (leftY + rightY)/2;
			int childId = (x > middleX ? 1 : 0);
			if (y > middleY) childId |= 2;
			if (root.children[childId] == null)
				root.children[childId] = new QTree();
			if (y <= middleY)
			{
				if (x <= middleX)
					SimpleAdd(root.children[childId], x, y, leftX, middleX, leftY, middleY, mapCell);
				else
					SimpleAdd(root.children[childId], x, y, middleX + 1, rightX, leftY, middleY, mapCell);
			}
			else
			{
				if (x <= middleX)
					SimpleAdd(root.children[childId], x, y, leftX, middleX, middleY + 1, rightY, mapCell);
				else
					SimpleAdd(root.children[childId], x, y, middleX + 1, rightX, middleY + 1, rightY, mapCell);
			}
		}
	}

	[TestFixture]
	public class QTree_Test
	{
		[Test]
		public void Init()
		{
			var t = new QTree();
			for (var x = 0; x < 3; x++)
				for (var y = 0; y < 10; ++y)
					switch (x + y%4)
					{
						case 0:
							QTree.SimpleAdd(t, x, y, 0, 2, 0, 9, MapCell.Earth);
							break;
						case 1:
							QTree.SimpleAdd(t, x, y, 0, 2, 0, 9, MapCell.Robot);
							break;
						case 2:
							QTree.SimpleAdd(t, x, y, 0, 2, 0, 9, MapCell.Lambda);
							break;
						case 3:
							QTree.SimpleAdd(t, x, y, 0, 2, 0, 9, MapCell.ClosedLift);
							break;
					}
			for (var x = 0; x < 3; x++)
				for (var y = 0; y < 10; ++y)
					switch (x + y%4)
					{
						case 0:
							Assert.AreEqual(QTree.Get(t, x, y, 0, 2, 0, 9), MapCell.Earth);
							break;
						case 1:
							Assert.AreEqual(QTree.Get(t, x, y, 0, 2, 0, 9), MapCell.Robot);
							break;
						case 2:
							Assert.AreEqual(QTree.Get(t, x, y, 0, 2, 0, 9), MapCell.Lambda);
							break;
						case 3:
							Assert.AreEqual(QTree.Get(t, x, y, 0, 2, 0, 9), MapCell.ClosedLift);
							break;
					}
		}
	}
}