using Photon.Pun;
using UnityEngine;

public class Player : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private float _moveSpeed = 3;
    [SerializeField] private float _jumpForce = 5;

    private Vector3 _smoothMove;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_photonView.IsMine)
        {
            ProcessInputs();
        }
        else
        {
            SmoothMovement();
        }
    }

    private void SmoothMovement()
    {
        transform.position = Vector3.Lerp(transform.position, _smoothMove, Time.deltaTime);
    }

    private void ProcessInputs()
    {
        var _move = new Vector3(Input.GetAxisRaw("Horizontal"), 0);
        transform.position += _move * _moveSpeed * Time.deltaTime;

        PlayerTurn();
    }

    private void PlayerTurn()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _spriteRenderer.flipX = false;
            _photonView.RPC("OnDirectionChanged_RIGHT", RpcTarget.Others);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _spriteRenderer.flipX = true;
            _photonView.RPC("OnDirectionChanged_LEFT", RpcTarget.Others);
        }
    }

    [PunRPC]
    private void OnDirectionChanged_LEFT()
    {
        _spriteRenderer.flipX = true;
    }

    [PunRPC]
    private void OnDirectionChanged_RIGHT()
    {
        _spriteRenderer.flipX = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            stream.ReceiveNext(); 
        }
    }
}
