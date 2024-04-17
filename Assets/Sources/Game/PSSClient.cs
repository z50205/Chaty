using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.Linq;
using Photon.Pun;
using UnityEngine.UI;

public class PSSClient : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _newgamebtn;
    [SerializeField] private Button _endturnbtn;
    [SerializeField] private ToggleGroup _togglegroup;
    [SerializeField] private Text _psslog;
    [SerializeField] private bool[] _toggleclientphase;
    [SerializeField] private bool over;
    [SerializeField] ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
    // [SerializeField] ExitGames.Client.Photon.Hashtable masterhash = PhotonNetwork.MasterClient.CustomProperties;
    // Start is called before the first frame update
    void Start()
    {
        this.over = false;
        this._toggleclientphase = new bool[] { false, true, false, false };
        StartCoroutine("StartPSSClient");
    }
    IEnumerator StartPSSClient()
    {
        while (true)
        {
            if (PhotonNetwork.MasterClient.CustomProperties.ContainsKey("Server_GamePhase"))
            {
                if (PhotonNetwork.MasterClient.CustomProperties.ContainsKey("Allready") && (bool)PhotonNetwork.MasterClient.CustomProperties["Allready"] && _toggleclientphase[(int)PhotonNetwork.MasterClient.CustomProperties["Server_GamePhase"]])
                {
                    switch ((int)PhotonNetwork.MasterClient.CustomProperties["Server_GamePhase"])
                    {
                        case 1:
                            Debug.Log("gamephase:all click new game！");
                            this._newgamebtn.GetComponentInChildren<Text>().text = "Connecting and Playing！";
                            this._endturnbtn.interactable = true;
                            _toggleclientphase[1] = false;
                            _toggleclientphase[2] = true;
                            break;
                        case 2:
                            Debug.Log("gamephase:all click endturn！");
                            this._endturnbtn.GetComponentInChildren<Text>().text = "Evaluating！";
                            Debug.Log("gamephase:Evaluate phase completed");
                            InterfaceChangeEvaluate();
                            _toggleclientphase[2] = false;
                            _toggleclientphase[3] = true;
                            this._endturnbtn.interactable = true;
                            Debug.Log(PhotonNetwork.MasterClient.CustomProperties.ToString());
                            Debug.Log("case2");
                            break;
                        case 3:
                            setClientEndturnfalse(3);
                            _toggleclientphase[3] = false;
                            _toggleclientphase[2] = true;
                            Debug.Log("case3");
                            break;
                    }
                }
            }
            yield return null;
        }
    }
    public void newgamebtn_onclick()
    {
        Debug.Log("newgamebtn_onclick_first");
        this._newgamebtn.interactable = false;
        this._newgamebtn.GetComponentInChildren<Text>().text = "Ready To Play！";
        this.hash["Ready"] = true;
        this.hash["GamePhase"] = 0;
        PhotonNetwork.LocalPlayer.SetCustomProperties(this.hash);
    }
    public void endturnbtn_onclick()
    {
        this._endturnbtn.interactable = false;
        this._endturnbtn.GetComponentInChildren<Text>().text = "Waiting other players";
        this.hash["Endturn"] = true;
        this.hash["GamePhase"] = 1;
        this.hash["PSS"] = GetToggleIndex(_togglegroup);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        while ((int)PhotonNetwork.LocalPlayer.CustomProperties["GamePhase"]!=1) ;
    }
    private int GetToggleIndex(ToggleGroup _group)
    {
        int _index = -1;
        Toggle[] _toggles = _group.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < _toggles.Length; i++)
        {
            if (_toggles[i].isOn == true) _index = i;
        }
        return _index;
    }
    public void setClientEndturnfalse(int _gamephase)
    {
        this.hash["GamePhase"] = _gamephase;
        this.hash["Endturn"] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        while ((bool)PhotonNetwork.LocalPlayer.CustomProperties["Endturn"]) ;
    }
    private void InterfaceChangeEvaluate()
    {
        int pivot = 0;
        int[] _playersactor = (int[])PhotonNetwork.MasterClient.CustomProperties["_playersactor"];
        string[] _playerresult = (string[])PhotonNetwork.MasterClient.CustomProperties["_playerresult"];
        for (int i = 0; i < _playersactor.Length; i++)
        {
            if (_playersactor[i] == PhotonNetwork.LocalPlayer.ActorNumber) pivot = i;
        }
        this._endturnbtn.GetComponentInChildren<Text>().text = _playerresult[pivot];
        this._psslog.text = (string)PhotonNetwork.MasterClient.CustomProperties["_log"];
        setClientEndturnfalse(2);
        //setServerEndturnfalse();
    }
    IEnumerator Allendturn()
    {
        while (!PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("Endturn") && (int)p.CustomProperties["GamePhase"] == 1 && (bool)p.CustomProperties["Endturn"]))
        {
            yield return null;
        }
        this._endturnbtn.GetComponentInChildren<Text>().text = "Evaluating！";
        this._endturnbtn.interactable = true;
        _toggleclientphase[2] = true;
    }
}
