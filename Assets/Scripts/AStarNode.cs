using UnityEngine;

namespace UnityTechTest
{
    public class AStarNode : NavGridPathNode
    {
        private const int MOVE_STRAIGHT_COST = 10;
        private const int MOVE_DIAGNAL_COST = 14;

        public float GCost;
        public float HCost;
        public float FCost;
        public AStarNode parent;

        public AStarNode(NavGridPathNode nd) 
        { 
            Position = nd.Position; 
            Status = nd.Status; 
            CellPosition = nd.CellPosition;
            Marker = nd.Marker;
        }

        public void CaluculateCosts(NavGridPathNode origin, NavGridPathNode target)
        {
            GCost = GetDistance(origin, this);
            HCost = GetDistance(this, target);
            FCost = GCost + HCost;
        }

        public static int GetDistance(NavGridPathNode nodeA, NavGridPathNode nodeB)
        {
            int distanceX = Mathf.Abs(nodeA.CellPosition.x - nodeB.CellPosition.x);
            int distanceY = Mathf.Abs(nodeA.CellPosition.y - nodeB.CellPosition.y);
            int remaining = Mathf.Abs(distanceX - distanceY);
            return MOVE_DIAGNAL_COST * Mathf.Min(distanceX, distanceY) + MOVE_STRAIGHT_COST * remaining;
        }
    }
}
