using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assembly_CSharp
{
    internal interface IPathFinder
    {
        abstract List<NavGridPathNode> GetPath(Vector3Int start, Vector3Int destination);
    }
}
