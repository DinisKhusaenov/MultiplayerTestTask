using Photon.Pun;
using UnityEngine;

public class WinPanel : MonoBehaviour
{
    [SerializeField] private GameObject _winPanel;
    private void Start()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            _winPanel.SetActive(false);
        }
    }
}
