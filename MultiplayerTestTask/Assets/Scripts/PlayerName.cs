using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private Button _setNameButton;

    public void OnNameChanged()
    {
        if (_nameInput.text.Length > 2) _setNameButton.interactable = true;
        else _setNameButton.interactable = false;
    }

    public void OnCLickSetNameButton()
    {
        PhotonNetwork.NickName = _nameInput.text;
    }
}
