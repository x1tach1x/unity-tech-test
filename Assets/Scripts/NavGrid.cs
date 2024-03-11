using System.Collections.Generic;
using UnityEngine;
using UnityTechTest;

public class NavGrid : MonoBehaviour
{
    [SerializeField]
    private Grid _grid;
    [SerializeField]
    public GameObject pathMarker;
    [SerializeField]
    private Transform _gridPlane;

    private NavGridPathNodeNeighbors _neighborStrategy;

    private Dictionary<Vector2Int, NavGridPathNode> _navReferences = new Dictionary<Vector2Int, NavGridPathNode>();
    private IPathFinder _pathFinder = new AStarPathFinder();

    //default to 10
    int _width = 10;
    int _height = 10;

    void Awake()
    {
        _width = (int)_gridPlane.localScale.x * 10;
        _height = (int)_gridPlane.localScale.z * 10;
    }

    /// <summary>
    /// Given the current and desired location, return a path to the destination
    /// </summary>
    /// 
    public Stack<NavGridPathNode> GetPath(Vector3 origin, Vector3 destination)
    {
        var path = _pathFinder.FindPath(this,origin, destination);
        return path;
    }
    /// <summary>
    /// Gets a NavGridPathNode from a world coordinate
    /// </summary>
    /// <param name="pos">World coordinate</param>
    /// <returns>NavGridPath</returns>
    public NavGridPathNode GetNavGridPathNode(Vector3 pos)
    {
        Vector3Int cellPosition = WorldToCell(pos);
        return GetNavGridPathNode(cellPosition);
    }
    /// <summary>
    /// Gets a NagGridPathNode from a Cell Position
    /// </summary>
    /// <param name="cellPosition">Cell Position</param>
    /// <returns></returns>
    public NavGridPathNode GetNavGridPathNode(Vector3Int cellPosition)
    {
        Vector2Int cell2D = ToVector2(cellPosition);
        if (_navReferences.ContainsKey(cell2D) == false)
        {
            var gridNode = new NavGridPathNode();
            gridNode.Position = _grid.CellToWorld(cellPosition);
            gridNode.CellPosition = cellPosition;
            gridNode.Status = PathStatus.Empty;
            _navReferences[cell2D] = gridNode;
        }
        return _navReferences[cell2D];
    }
    /// <summary>
    /// Gets all the nodes neighbors as NavGridPathNode
    /// </summary>
    /// <param name="node"></param>
    /// <returns>List of neighbors</returns>
    public List<NavGridPathNode> GetNavGridPathNodeNeighbors(NavGridPathNode node)
    {
        if (node.Neighbors != null && node.Neighbors.Count > 0)
            return node.Neighbors;
        // Temporary Should make have manager handle later right now defaults to Square style
        SetNeighborStrategy(new SquareNodeNeighbors());
        var neighbors = _neighborStrategy.GetNeighbors(node);
        List<NavGridPathNode> result = new List<NavGridPathNode>();
        foreach (var neighbor in neighbors)
        {
            if(IsCellOutOfBounds(neighbor) == false)
                result.Add(GetNavGridPathNode(neighbor));
        }
        return result;
    }
    /// <summary>
    /// Clears the Navgrid of the given cell position
    /// </summary>
    /// <param name="cell"></param>
    public void ClearCell(Vector3Int cell)
    {
        if (_navReferences.TryGetValue(ToVector2(cell),out var value))
        {
            value.Status = PathStatus.Empty;
            _navReferences.Remove(ToVector2(cell));
        }
    }
    public void ClearCell(Vector3 pos)
    {
        ClearCell(_grid.WorldToCell(pos));
    }
    /// <summary>
    /// Adds new cell to the NavGrid
    /// </summary>
    /// <param name="cellPosition">Cell Position</param>
    /// <param name="status"></param>
    /// <returns></returns>
    public bool AddCell(Vector3Int cellPosition, PathStatus status)
    {
        if(IsCellOutOfBounds(cellPosition))
        {
            return false;
        }
        var cell2d = ToVector2(cellPosition);
        if (_navReferences.ContainsKey(cell2d) == false)
        {
            return _navReferences.TryAdd(cell2d, new NavGridPathNode() { Marker = pathMarker, Position = _grid.CellToWorld(cellPosition), CellPosition = cellPosition, Status = status });
        } else if(_navReferences[cell2d].Status == PathStatus.Empty)
        {
            _navReferences[cell2d].Status = status;
            return true;
        }
        return false;
    }
    /// <summary>
    /// Adds a new cell to the NavGrid 
    /// </summary>
    /// <param name="pos">World Coord</param>
    /// <param name="status"></param>
    /// <returns></returns>
    public bool AddCell(Vector3 pos, PathStatus status)
    {
        if (IsOutOfBounds(pos))
        {
            return false;
        }
        var cell = ToVector2(_grid.WorldToCell(pos));
        if (_navReferences.ContainsKey(cell) == false)
        {
            return _navReferences.TryAdd(cell, new NavGridPathNode() { Marker = pathMarker, Position = pos, CellPosition = _grid.WorldToCell(pos), Status = status });
        }
        if (_navReferences[cell].Status == PathStatus.Empty && status != PathStatus.Empty)
        {
            _navReferences.Remove(cell);
            return _navReferences.TryAdd(cell, new NavGridPathNode() { Marker = pathMarker, Position = pos, CellPosition = _grid.WorldToCell(pos), Status = status });
        }
        return false;
    }

    /// <summary>
    /// Sets the Strategy to use when getting neighbors
    /// </summary>
    /// <param name="strategy"></param>
    public void SetNeighborStrategy(NavGridPathNodeNeighbors strategy)
    {
        _neighborStrategy = strategy;
    }
    /// <summary>
    /// Helper function to Convert 3D Cell Position to 2D
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector2Int ToVector2(Vector3Int pos)
    {
        return new Vector2Int(pos.x, pos.y);
    }
    /// <summary>
    /// Helper function to Convert 3D Cell Position to 3D World Coordinate
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector3 CellToWorld(Vector3Int pos)
    {
        return _grid.CellToWorld(pos);
    }
    /// <summary>
    /// Helper function to Convert 3D  World Coordinate to 3D Cell Position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector3Int WorldToCell(Vector3 pos)
    {
        return _grid.WorldToCell(pos);
    }

    public bool IsOutOfBounds(Vector3 pos)
    {
        return (pos.x > _gridPlane.position.x + _width / 2 || pos.x < _gridPlane.position.x - _width / 2
            || pos.z > _gridPlane.position.z + _height / 2 || pos.z < _gridPlane.position.z - _height / 2);
    }
    public bool IsCellOutOfBounds(Vector3Int cellPos)
    {
        return (cellPos.x > _gridPlane.position.x + _width / 2 || cellPos.x < _gridPlane.position.x - _width / 2
            || cellPos.y > _gridPlane.position.y + _height / 2 || cellPos.y < _gridPlane.position.y - _height / 2);
    }

    public Vector3 LocalToWorld(Vector3 pos)
    {
        return _grid.LocalToWorld(pos);
    }
}
