using System.Collections.Generic;
using UnityEngine;

public abstract class NavGridPathNodeNeighbors
{
    public abstract List<Vector3Int> GetNeighbors(NavGridPathNode node);
}