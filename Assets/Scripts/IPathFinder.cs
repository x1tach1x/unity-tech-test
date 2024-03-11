using System.Collections.Generic;
using UnityEngine;

namespace UnityTechTest { 
    interface IPathFinder
    {
        abstract Stack<NavGridPathNode> FindPath(NavGrid navGrid , Vector3 start, Vector3 destination);
    }
}
