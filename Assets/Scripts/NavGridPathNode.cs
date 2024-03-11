using System.Collections.Generic;
using UnityEngine;

public enum PathStatus
{
    Empty = 0,
    Unpassable = 1
}

public class NavGridPathNode
{
    /// <summary>
    /// World position of the node
    /// </summary>
    public Vector3 Position;
    public Vector3Int CellPosition;
    public PathStatus Status;
    public List<NavGridPathNode> Neighbors;
    public GameObject Marker;
}

