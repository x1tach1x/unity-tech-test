using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace UnityTechTest {

    class AStarPathFinder : IPathFinder
    {
        private HashSet<Vector3Int> _closed = new HashSet<Vector3Int>();
        private Dictionary<Vector3Int, AStarNode> _starNodeCache = new Dictionary<Vector3Int, AStarNode>();
        private List<AStarNode> _openList = new List<AStarNode>();
        private AStarNode _startNode;
        private AStarNode _destinationNode;
        private AStarNode _lastPos;

        private bool done = false;

        /// <summary>
        /// Finds the shortest path within the given NavGrid between two points
        /// </summary>
        /// <param name="navGrid"></param>
        /// <param name="startPos"></param>
        /// <param name="destinationPos"></param>
        /// <returns></returns>
        public Stack<NavGridPathNode> FindPath(NavGrid navGrid, Vector3 startPos, Vector3 destinationPos)
        {
            done = false;
            _startNode = new AStarNode(navGrid.GetNavGridPathNode(startPos));
            _destinationNode = new AStarNode(navGrid.GetNavGridPathNode(destinationPos));
            _starNodeCache.TryAdd(_destinationNode.CellPosition, _destinationNode);
            _startNode.CaluculateCosts(_startNode, _destinationNode);
            _starNodeCache.TryAdd(_startNode.CellPosition, _startNode);
            _openList.Add(_startNode);
            _lastPos = _startNode;

            while (!done && _openList.Count > 0)
            {
                  Search(ref _lastPos,ref navGrid);
            }
            var sol = new Stack<NavGridPathNode>();
            if (_lastPos.CellPosition == _destinationNode.CellPosition)
            {
                sol = RetraceSteps(_startNode, _lastPos);
            }
            _closed.Clear();
            _openList.Clear();
            _starNodeCache.Clear();
            return sol;
        }
        /// <summary>
        /// Handles the calculations for determining the optimal path
        /// </summary>
        /// <param name="thisNode"></param>
        /// <param name="navGrid"></param>
        private void Search(ref AStarNode thisNode,ref NavGrid navGrid)
        {
            GetLowestFCostNode(ref thisNode);
            if (thisNode.CellPosition == _destinationNode.CellPosition)
            {
                done = true;
                return;
            }
            if (thisNode.Neighbors == null)
            {
                thisNode.Neighbors = new List<NavGridPathNode>();
            } 
            else
            {
                thisNode.Neighbors.Clear();
            }
            _openList.Remove(thisNode);
            _closed.Add(thisNode.CellPosition);
            var neighbors = navGrid.GetNavGridPathNodeNeighbors(thisNode);
            HandleNeighbors(ref thisNode, ref neighbors);
            
        }
        private void GetLowestFCostNode(ref AStarNode thisNode)
        {
            AStarNode frontNode = _openList.ElementAt(0);
            for(int i = 0; i < _openList.Count; i++)
            {
                if (_openList[i].FCost < frontNode.FCost)
                {
                    frontNode = _openList[i];
                }
            }
            thisNode = frontNode;
        }
        private void HandleNeighbors(ref AStarNode thisNode, ref List<NavGridPathNode> neighbors)
        {
            foreach (var neighbor in neighbors)
            {
                if (neighbor.Status == PathStatus.Unpassable || _closed.Contains(neighbor.CellPosition))
                {
                    _closed.Add(neighbor.CellPosition);
                    continue;
                }
                if(_starNodeCache.TryGetValue(neighbor.CellPosition,out var neighborNode) == false){
                    neighborNode = new AStarNode(neighbor);
                }          
                
                int G = (int)(AStarNode.GetDistance(thisNode, neighbor) + thisNode.GCost);
                int H = AStarNode.GetDistance(neighbor,_destinationNode);
                int F = G + H;
                neighborNode.parent = thisNode;
                neighborNode.GCost = G;
                neighborNode.HCost = H;
                neighborNode.FCost = F;

                thisNode.Neighbors.Add(neighborNode);
                if (_starNodeCache.TryAdd(neighbor.CellPosition, neighborNode) == false)
                {
                    _starNodeCache[neighbor.CellPosition] = neighborNode;
                }

                bool found = false;         
                foreach (var node in _openList)
                {
                    if (node.CellPosition == neighborNode.CellPosition)
                    {
                        node.GCost = G;
                        node.HCost = H;
                        node.FCost = F;
                        node.parent = thisNode;
                        found = true;
                    }
                }
                if (found == false)
                {
                    _openList.Add(neighborNode);
                }
            }
        }
        private Stack<NavGridPathNode> RetraceSteps(AStarNode start, AStarNode destinationNode)
        {
            Stack<NavGridPathNode> path = new Stack<NavGridPathNode>();
            AStarNode currentNode = destinationNode;
            while (currentNode != start && currentNode != null)
            {
                path.Push(currentNode);
                currentNode = currentNode.parent;
            }
            return path;
        }
    }
}
