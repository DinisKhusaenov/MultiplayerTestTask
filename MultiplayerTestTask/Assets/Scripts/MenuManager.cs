using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField _createInput;
    [SerializeField] private TMP_InputField _joinInput;

    public void CreateRoom()
    {
        RoomOptions _roomOptions = new();
        _roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(_createInput.text, _roomOptions);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(_joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene(2);
    }
}
