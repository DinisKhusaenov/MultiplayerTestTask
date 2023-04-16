using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviourPun
{
    [SerializeField] private float _speed;
    [SerializeField] private float _destroyTime;

    private bool _shootLeft = false;

    public static readonly int Damage = 2;

    IEnumerator DestroyBullet()
    {
        yield return new WaitForSeconds(_destroyTime);
        this.GetComponent<PhotonView>().RPC("DestroyBulletRPC", RpcTarget.AllBuffered);
    }

    private void Start()
    {
        StartCoroutine(DestroyBullet());
    }

    private void Update()
    {
        if (!_shootLeft) transform.Translate(Vector3.right * Time.deltaTime * _speed);
        else transform.Translate(Vector3.left * Time.deltaTime * _speed);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out Player _player))
        {
            DestroyBulletRPC();
        }
    }

    [PunRPC]
    public void DestroyBulletRPC()
    {
        Destroy(this.gameObject);
    }

    [PunRPC]
    public void ChangedDirection()
    {
        _shootLeft = true;
    }

    
}
