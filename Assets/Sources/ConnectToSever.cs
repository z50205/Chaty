using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectToSever : MonoBehaviourPunCallbacks
{
    public Button btn;
    [SerializeField] private bool _connectstate;

    // Start is called before the first frame update
    void Start()
    {
        _connectstate = false;
        // btn.interactable=false;
        // PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // btn.GetComponentInChildren<Text>().text="Connect!";
        // btn.interactable=true;
        btn.GetComponentInChildren<Text>().text = "Connecting！";
        btn.interactable = true;
        _connectstate=true;
    }
    public void ClickToConnect()
    {
        if (!_connectstate)
        {
            btn.interactable = false;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
            SceneManager.LoadScene("Lobby");
    }
}
