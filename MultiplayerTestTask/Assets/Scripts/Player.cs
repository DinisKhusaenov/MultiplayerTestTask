using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float _moveSpeed = 3;
    [SerializeField] private float _jumpForce = 200;

    [SerializeField] private PhotonView _photonView;
    [SerializeField] private GameObject _playerCamera;
    [SerializeField] private TMP_Text _playerName;

    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Transform _bulletSpawnRight;
    [SerializeField] private Transform _bulletSpawnLeft;

    [SerializeField] private GameObject _buttons;

    private GameObject _sceneCamera;
    private Vector3 _smoothMove;
    private Vector3 _move = new Vector3(0, 0, 0);

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidBody2D;

    private bool _isGrounded;
    private readonly int _playerHP = 20;
    private int _playerCurrentHP;

    public event Action<float> EnemyHealthChanged;

    private void Start()
    {
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 15;

        _playerCurrentHP = _playerHP;

        if (photonView.IsMine)
        {
            _playerName.text = PhotonNetwork.NickName;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidBody2D = GetComponent<Rigidbody2D>();

            _buttons.SetActive(true);

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

    public void OnShootButtonClick()
    {
        if (_spriteRenderer.flipX == true)
        {
            GameObject _bullet = PhotonNetwork.Instantiate(_bulletPrefab.name, _bulletSpawnLeft.position, Quaternion.identity);
            _bullet.GetComponent<PhotonView>().RPC("ChangedDirection", RpcTarget.AllBuffered);
        }
        else
        {
            GameObject _bullet = PhotonNetwork.Instantiate(_bulletPrefab.name, _bulletSpawnRight.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (photonView.IsMine)
        {
            if (collision.TryGetComponent<Bullet>(out Bullet _bullet))
            {
                TakeDamage();
            }
        }
    }

    private void TakeDamage()
    {
        Debug.Log("damage");
        ChangeHealth();
    }

    private void ChangeHealth()
    {
        _playerCurrentHP -= Bullet.Damage;

        if (_playerCurrentHP < 0)
        {
            DeadPlayer();
        }
        else
        {
            float _currentHealthAsPercantage = (float)_playerCurrentHP / _playerHP;
            EnemyHealthChanged?.Invoke(_currentHealthAsPercantage);
        }
    }

    private void DeadPlayer()
    {
        EnemyHealthChanged?.Invoke(0);

        if (_playerCurrentHP <= 0)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }

        SceneManager.LoadScene(1);
    }

    public void OnJumpButtonClick()
    {
        if (_isGrounded)
        {
            _rigidBody2D.AddForce(Vector2.up * _jumpForce);
        }
    }

    public void OnRightButtonDown()
    {
        _move = new Vector3(1, 0, 0);
    }

    public void OnLeftButtonDown()
    {
        _move = new Vector3(-1, 0, 0);
    }

    public void OnMoveButtonUp()
    {
        _move = new Vector3(0, 0, 0);
    }

    private void SmoothMovement()
    {
        transform.position = Vector3.Lerp(transform.position, _smoothMove, Time.deltaTime);
    }

    private void ProcessInputs()
    {
        transform.position += _move * _moveSpeed * Time.deltaTime;

        PlayerTurn();
    }

    private void PlayerTurn()
    {
        if (_move.x == 1)
        {
            _spriteRenderer.flipX = false;
            _photonView.RPC("OnDirectionChanged_RIGHT", RpcTarget.Others);
        }
        else if (_move.x == -1)
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
