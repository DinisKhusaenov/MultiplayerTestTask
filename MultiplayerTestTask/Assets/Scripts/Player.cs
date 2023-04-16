using Photon.Pun;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private float _moveSpeed = 3;
    [SerializeField] private float _jumpForce = 200;
    [SerializeField] private GameObject _playerCamera;
    [SerializeField] private TMP_Text _playerName;

    private GameObject _sceneCamera;
    private Vector3 _smoothMove;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidBody2D;
    private bool _isGrounded;

    private void Start()
    {
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 15;
        if (photonView.IsMine)
        {
            _playerName.text = PhotonNetwork.NickName;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidBody2D = GetComponent<Rigidbody2D>();

            _sceneCamera = GameObject.Find("Main Camera");
            _sceneCamera.SetActive(false);
            _playerCamera.SetActive(true);
        }
        else
        {
            _playerName.text = _photonView.Owner.NickName;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
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
        PlayerJump();
    }

    private void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _rigidBody2D.AddForce(Vector2.up * _jumpForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (photonView.IsMine)
        {
            if (collision.gameObject.tag == "Ground")
            {
                _isGrounded = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (photonView.IsMine)
        {
            if (collision.gameObject.tag == "Ground")
            {
                _isGrounded = false;
            }
        }
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
