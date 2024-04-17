using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ClickToJoin : MonoBehaviour
{
[SerializeField] private Text _text;
    public void CreateRoom()
    {
        PhotonNetwork.JoinOrCreateRoom(_text.text, new RoomOptions { IsVisible = true },null);
    }
}
