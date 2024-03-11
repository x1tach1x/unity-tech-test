using UnityEngine;

public class Player : PathFindingObject {

    [SerializeField]
    private Rigidbody _rigidbody;

    void Update()
    {
        // Check Input
        if (Input.GetMouseButtonUp(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo))
            {
                var point = hitInfo.point;
                point.y = 0f;
                MoveToLocation(point);
            }
        }
        // Moves to next Location in path
        Traverse();

        // Locks in place when reaches destination
        if(_currentPath.Count == 0)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}
