using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string  OBSTACLES = "Obstacles";
  
    [SerializeField]
    private NavGrid _grid;
    [SerializeField]
    private LayerMask _gridLayerMask;
    [SerializeField]
    private LayerMask _obstacleLayerMask;
    [SerializeField]
    private GameObject _mouseIndicator;
    [SerializeField]
    private GameObject _startMarker;
    [SerializeField]
    private GameObject _endMarker;
    [SerializeField]
    private GameObject _pathMarker;
    [SerializeField]
    private GameObject _obstacle;
    [SerializeField]
    private GameObject _spawnObstacle;
    [SerializeField]
    private GameObject _ballTracker;
    [SerializeField]
    private FetchingPet _fetchingPet;

    private GameObject _Player;
    private Rigidbody _ballRB;

    private bool _showMarkers = false;
    private IObjectManager _objectManager = new ObjectManager();

    private void Awake()
    {
        _startMarker = _objectManager.SpawnObject(_startMarker, _startMarker.transform.position, "");
        _endMarker = _objectManager.SpawnObject(_endMarker, _endMarker.transform.position, "");
        _startMarker.SetActive(false);
        _endMarker.SetActive(false);
    }
    void Start()
    {
        _Player = GameObject.FindWithTag("Player");
        _objectManager.AddParent(OBSTACLES);
        _ballTracker = _objectManager.SpawnObject(_ballTracker.gameObject, _Player.transform.position);
        _ballRB = _ballTracker.GetComponent<Rigidbody>();
        if (_obstacle != null)
        {
            int childCount = _obstacle.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                _grid.AddCell(_obstacle.transform.GetChild(i).position, PathStatus.Unpassable);
            }
        }
    }

    public IObjectManager GetObjectManager()
    {
        return _objectManager;
    }
    /// <summary>
    /// Gets a path from start to finish within the NavGrid
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns>Stack with nodes in path from start to finish </returns>
    public Stack<NavGridPathNode> GetPath(Vector3 start, Vector3 end)
    {
        var path = _grid.GetPath(start,end);
        if (Debug.isDebugBuild && _showMarkers) {
            DebugDrawStartEndMarkers(start, end);
            DebugDrawPathMarkers(path);
        }     
        return path;
    }
    /// <summary>
    /// Getst he NavGrid
    /// </summary>
    /// <returns></returns>
    public NavGrid GetGrid()
    {
        return _grid;
    }

    // Update is called once per frame
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, 100, _gridLayerMask))
        {
            _mouseIndicator.transform.position = _grid.CellToWorld(_grid.WorldToCell(hitInfo.point));
            // Add Obstacles
            if (Input.GetKeyDown(KeyCode.M))
            {
                if (_grid.AddCell(_grid.WorldToCell(hitInfo.point), PathStatus.Unpassable))
                {
                    _objectManager.SpawnObject(_spawnObstacle, _grid.CellToWorld(_grid.WorldToCell(hitInfo.point)), OBSTACLES);
                }
            }
        }
        // Remove Obstacles
        if (Physics.Raycast(ray, out var obstacleHitInfo, 100, _obstacleLayerMask)) {
            if (Input.GetKeyDown(KeyCode.N))
            {
                _objectManager.FreeObject(obstacleHitInfo.transform.parent.gameObject, OBSTACLES);
                _grid.ClearCell(_grid.WorldToCell(_mouseIndicator.transform.position));
            }
        }
        //Remove Markers
        if (Input.GetKeyDown(KeyCode.C))
        {
            _objectManager.ClearAll(_objectManager.GetParentName(_pathMarker));
        }

        //Throw Ball
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _ballTracker.transform.position = _Player.transform.position + new Vector3(0f,2f,0f);
            _ballRB.AddForce((_mouseIndicator.transform.position - _ballTracker.transform.position) * 20);
        }
        //Tell Dog Fetch
        if(Input.GetKeyDown(KeyCode.F))
        {
            _fetchingPet.Fetch(_Player.transform.position,ref _ballTracker);
        }
        // Toggle Debug Markers
        if (Input.GetKeyDown(KeyCode.T))
        {
            _showMarkers = !_showMarkers;
            _startMarker.SetActive(_showMarkers);
            _endMarker.SetActive(_showMarkers);
            if(_showMarkers == false)
            {
                _objectManager.ClearAll(_objectManager.GetParentName(_pathMarker));
            }
        }
    }

    private void DebugDrawStartEndMarkers(Vector3 start, Vector3 end)
    {
        _startMarker.transform.position = start;
        _endMarker.transform.position = end;
        _startMarker.SetActive(true);
        _endMarker.SetActive(true);
    }
    private void DebugDrawPathMarkers(Stack<NavGridPathNode> path)
    {
        _objectManager.ClearAll(_objectManager.GetParentName(_pathMarker));
        foreach (var obj in path)
        {
            obj.Marker = _objectManager.SpawnObject(_pathMarker, obj.Position, "");
        }
    }
}
