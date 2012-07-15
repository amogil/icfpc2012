using System.Collections.Generic;
using NUnit.Framework;

namespace Logic
{
	public class Pack
	{
		public int x, y;
		public MapCell data;
	}

	public class QTree
	{
		private MapCell data;
		private QTree[] children;

		public static QTree SetPack(QTree root, List<Pack> listData, int listLeft, int listRight, int leftX, int rightX, int leftY, int rightY)
		{
			QTree newRoot = new QTree();
			if (leftX == rightX && leftY == rightY)
			{
				newRoot.data = listData[listLeft].data;
				return newRoot;
			}
			newRoot.children = new QTree[4];
			int ptr = listLeft;
			int from = listLeft;
			int middleX = (leftX + rightX) / 2;
			int middleY = (leftY + rightY) / 2;
			int[] leftLimitX = new[] {leftX, middleX + 1};
			int[] rightLimitsX = new[] {middleX, rightX};
			int[] leftLimitY = new[] {leftY, middleY + 1};
			int[] rightLimitsY = new[] {middleY, rightY};

			for (int i = 0; i < 3; ++i)
			{
				for (int j = from; j <= listRight; ++j) 
					if (listData[j].x >= leftLimitX[i&1] && listData[j].x <= rightLimitsX[i&1]) 
						if (listData[j].y >= leftLimitY[(i&2)>>1] && listData[j].y <= rightLimitsY[(i&2) >> 1])
						{
							var temp = listData[j];
							listData[j] = listData[ptr];
							listData[ptr] = temp;
							++ptr;
						}
				if (ptr > from)
					newRoot.children[i] = SetPack(root.children[i], listData, from, ptr - 1, leftLimitX[i & 1], rightLimitsX[i & 1], leftLimitY[(i & 2) >> 1], rightLimitsY[(i & 2) >> 1]);
				else
					newRoot.children[i] = root.children[i];
				from = ptr;
			}
			if (from <= listRight)
				newRoot.children[3] = SetPack(root.children[3], listData, from, listRight, leftLimitX[1], rightLimitsX[1], leftLimitY[1], rightLimitsY[1]);
			else
				newRoot.children[3] = root.children[3];

			return newRoot;
		}

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

		[Test]
		public void PackTest()
		{
			var t = new QTree();
			List<Pack> list = new List<Pack>();
			var t2 = new QTree();
			
			for (var i = 0; i < 501; ++i)
				for (var j = 0; j < 501; ++j)
				{
					QTree.SimpleAdd(t, i, j, 0, 500, 0, 500, MapCell.Earth);
					QTree.SimpleAdd(t2, i, j, 0, 500, 0, 500, MapCell.Earth);
				}
			int x = 400, y = 400;
			List <MapCell> cells = new List<MapCell> {MapCell.Earth, MapCell.OpenedLift, MapCell.Lambda, MapCell.Trampoline3, MapCell.Wall };
			for (int i = 0; i < 500; ++i)
			{
				list.Add(new Pack {data = cells[i%5], x = x, y = y});
				--x;
				if (x == 0)
				{
					x = 400;
					--y;
				}
			}
			foreach (var item in list)
				QTree.Set(t2, item.x, item.y, 0, 500, 0, 500, item.data);
			QTree.SetPack(t, list, 0, list.Count - 1, 0, 500, 0, 500);
			foreach (var item in list)
				Assert.AreEqual(QTree.Get(t2, item.x, item.y, 0, 500, 0, 500), QTree.Get(t2, item.x, item.y, 0, 500, 0, 500));
		}
	}
}