using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    public interface IConnectionCheck
    {
        bool Connected(int fx, int fy, int tx, int ty);
    }

    public class PathNode2D : IPathNode<IConnectionCheck>
    {
        public PathNode2D() { }

        public PathNode2D(int x, int y, int height)
        {
            X = x;
            Y = y;
            Height = height;
        }

        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public int Height { get; set; }

        public bool Connected(IConnectionCheck checker, int x, int y)
        {
            return checker.Connected(X, Y, x, y);
        }

        public bool IsWalkable()
        {
            return true;
        }
    }

    public class AStarSolver<TPathNode> : SpatialAStar<TPathNode, IConnectionCheck> where TPathNode : IPathNode<IConnectionCheck>
    {
        protected override Double Heuristic(PathNode inStart, PathNode inEnd)
        {
            return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);
        }

        protected override Double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            return Heuristic(inStart, inEnd);
        }

        public AStarSolver(TPathNode[,] inGrid)
            : base(inGrid)
        {
        }
    }

    // A* 寻路器
    public class AStarPathFinder : IConnectionCheck
    {
        public bool Connected(int fx, int fy, int tx, int ty)
        {
            int h1 = nodes[fx, fy].Height;
            int h2 = nodes[tx, ty].Height;
            return h1 >= h2;
        }

        AStarSolver<PathNode2D> s = null;
        PathNode2D[,] nodes = null;

        // 重置地图数据
        public void ResetMapData(int[] walkable, int width, int height)
        {
            nodes = new PathNode2D[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    nodes[i, j] = new PathNode2D(i, j, walkable[j * width + i]);
                }
            }

            s = new AStarSolver<PathNode2D>(nodes);
        }

        // 搜索路径，结果以 [x1, y1, x2, y2, ...] 的形式返回
        public int[] FindPath(int fx, int fy, int ex, int ey)
        {
            LinkedList<PathNode2D> path = s.Search(fx, fy, ex, ey, this);
            if (path == null || path.Count == 0)
                return new int[0];

            int[] r = new int[path.Count * 2];
            int i = 0;
            foreach (PathNode2D n in path)
            {
                r[i * 2] = n.X;
                r[i * 2 + 1] = n.Y;
                i++;
            }

            return r;
        }

        // 设置指定点的行走高度
        public void SetHeight(int x, int y, int height)
        {
            nodes[x, y].Height = height;
        }
    }
}
