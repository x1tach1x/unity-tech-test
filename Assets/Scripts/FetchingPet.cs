using UnityEngine;

public class FetchingPet : PathFindingObject
{
    Vector3 _playerPos = new Vector3();
    bool _goFetch = false;
    bool _gotBall = false;
    bool _returnedToPlayer = false;
    [SerializeField]
    private Animator _animator;
    private GameObject _ball;

    // Update is called once per frame
    void Update()
    {
        if (_goFetch)
        {
            if(AtLocation() && _gotBall == false)
            {
                _gotBall = true;
                MoveToLocation(_playerPos);
            } 
            else if(AtLocation() && _returnedToPlayer == false)
            {
                _returnedToPlayer = true;
                _goFetch = false;
            }
            if (_gotBall == true) {
                _ball.transform.position = this.transform.position;
            }
            Traverse();
            
        } 
        else
        {
            _animator.StopPlayback();
            _returnedToPlayer = false;
            _gotBall = false;
        }
    }
    /// <summary>
    /// Sends the Fetching Pet to go retrieve the object and bring it to desired postion
    /// </summary>
    /// <param name="playerPos"></param>
    /// <param name="ball"></param>
    public void Fetch(Vector3 playerPos, ref GameObject ball)
    {
        this._ball = ball;
        MoveToLocation(ball.transform.position);
        if(_currentPath.Count > 0)
        {
            _goFetch = true;
        }
        this._playerPos = playerPos;
    }

    protected override void Traverse()
    {
        _animator.StartPlayback();
        base.Traverse();
    }
}
