using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerCoins : MonoBehaviourPun
{
    [SerializeField] private TMP_Text _coinsCointText;
    [SerializeField] private GameObject _coin;

    private int _coinsCount = 0;

    private void Start()
    {
        if (photonView.IsMine)
        {
            _coin.SetActive(true);
        }
    }

    private void Update()
    {
        _coinsCointText.text = _coinsCount.ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Coin>(out Coin _coin))
        {
            _coinsCount++;
        }
    }
}
