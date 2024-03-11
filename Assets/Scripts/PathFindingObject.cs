using System.Collections.Generic;
using UnityEngine;

public class PathFindingObject : MonoBehaviour
{

    protected Stack<NavGridPathNode> _currentPath = new Stack<NavGridPathNode>();
    [SerializeField]
    protected float _speed = 10.0f;
    protected float _timer = 0f;
    protected GameManager _gameManager;

    private void Start()
    {
        _gameManager = GameObject.FindFirstObjectByType<GameManager>();
    }
    void Update()
    {
        // Check Input
        Traverse();
    }
    /// <summary>
    /// Moves the object to the desired location
    /// </summary>
    /// <param name="targetPosition"></param>
    public void MoveToLocation(Vector3 targetPosition)
    {
        _currentPath = _gameManager.GetPath(transform.position, targetPosition);
    }
    /// <summary>
    /// True if the Object is at its final location
    /// </summary>
    /// <returns></returns>
    public bool AtLocation()
    {
        return _currentPath.Count == 0;
    }
    /// <summary>
    /// The logic for object movement
    /// </summary>
    protected virtual void Traverse()
    {
        if (!AtLocation())
        {
            var currentNode = _currentPath.Peek();
            _timer += _speed * Time.deltaTime;
            //Ignore the Y
            currentNode.Position.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, currentNode.Position, _timer);
            transform.LookAt(currentNode.Position);
            if (transform.position.x == currentNode.Position.x && transform.position.z == currentNode.Position.z)
            {
                _timer = 0;
                _currentPath.Pop();
            }
        }
        if (_currentPath.Count == 0)
        {
            transform.position = transform.position;
        }
    }
}

