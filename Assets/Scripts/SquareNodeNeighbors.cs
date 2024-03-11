using System.Collections.Generic;
using UnityEngine;

public class SquareNodeNeighbors : NavGridPathNodeNeighbors
{
    private Vector3Int up;
    private Vector3Int down;
    private Vector3Int left;
    private Vector3Int right;
    private Vector3Int upLeft;
    private Vector3Int upRight;
    private Vector3Int downLeft;
    private Vector3Int downRight;

    /// <summary>
    /// Gets a list of neighbors in clockwise order starting from up.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public override List<Vector3Int> GetNeighbors(NavGridPathNode node)
    {
        up = node.CellPosition + Vector3Int.up;
        down = node.CellPosition + Vector3Int.down;
        left = node.CellPosition + Vector3Int.left;
        right = node.CellPosition + Vector3Int.right;
        upLeft = node.CellPosition + Vector3Int.up + Vector3Int.left;
        upRight = node.CellPosition + Vector3Int.up + Vector3Int.right;
        downLeft = node.CellPosition + Vector3Int.down + Vector3Int.left;
        downRight = node.CellPosition + Vector3Int.down + Vector3Int.right;
        return new List<Vector3Int>{up, upRight, right , downRight, down, downLeft,left, upLeft };
    }
}
