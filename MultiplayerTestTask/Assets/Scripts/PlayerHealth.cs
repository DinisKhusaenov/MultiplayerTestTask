using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviourPun
{
    [SerializeField] private Player _player;
    [SerializeField] private Image _healthBarFilling;
    [SerializeField] private Image _health;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            _health.gameObject.SetActive(true);
        }
        _player.EnemyHealthChanged += OnHealthEnemyChanged;
    }

    private void OnDestroy()
    {
        _player.EnemyHealthChanged -= OnHealthEnemyChanged;
    }

    private void OnHealthEnemyChanged(float valueInPercentage)
    {
        _healthBarFilling.fillAmount = valueInPercentage;
    }
}
