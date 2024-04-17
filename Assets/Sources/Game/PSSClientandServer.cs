using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.Linq;
using Photon.Pun;
using UnityEngine.UI;

public class PSSClientandServer : MonoBehaviour
{

    [SerializeField] private Button _newgamebtn;
    [SerializeField] private Button _endturnbtn;
    [SerializeField] private ToggleGroup _togglegroup;
    [SerializeField] private Text _psslog;
    [SerializeField] private bool over;
    [SerializeField] ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;

    [SerializeField] int turn;
    //Evaluate variables
    [SerializeField] int[] _playersactor = new int[20];
    [SerializeField] string[] _playerresult = new string[20];
    Player[] players = PhotonNetwork.PlayerList;
    [SerializeField] int[] paper;
    [SerializeField] int[] scissor;
    [SerializeField] int[] stone;
    [SerializeField] int[] result;


    void Start()
    {
        this.over = false;
        StartCoroutine("StartPSSClient");
        if (!PhotonNetwork.IsMasterClient) return;
        turn = 1;
        StartCoroutine("StartPSSServer");
    }
    IEnumerator StartPSSClient()
    {
        while (true)
        {
            Debug.Log("1");
            if (PhotonNetwork.MasterClient.CustomProperties.ContainsKey("GamePhase"))
            {
                Debug.Log("2");
                Debug.Log(PhotonNetwork.MasterClient.CustomProperties.ToString());
                Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties.ToString());
                Debug.Log("1:" + PhotonNetwork.MasterClient.CustomProperties.ContainsKey("Allready"));
                if (PhotonNetwork.MasterClient.CustomProperties.ContainsKey("Allready") && (bool)PhotonNetwork.MasterClient.CustomProperties["Allready"])
                {
                    Debug.Log("3");
                    switch ((int)PhotonNetwork.MasterClient.CustomProperties["GamePhase"])
                    {
                        case 1:
                            Debug.Log("gamephase:all click new game！");
                            setServerAllreadyfalse();
                            this._newgamebtn.GetComponentInChildren<Text>().text = "Connecting and Playing！";
                            this._endturnbtn.interactable = true;
                            break;
                        case 2:
                            Debug.Log("gamephase:all click endturn！");
                            setServerAllreadyfalse();
                            this._endturnbtn.GetComponentInChildren<Text>().text = "Evaluating！";
                            break;
                        case 3:
                            Debug.Log("gamephase:Evaluate phase completed");
                            setServerAllreadyfalse();
                            InterfaceChangeEvaluate();
                            this._endturnbtn.interactable = true;
                            break;
                    }
                    yield return null;
                }
                yield return null;
            }
            yield return null;
        }
    }
    IEnumerator StartPSSServer()
    {
        StartCoroutine("StartnewgameServer");
        StartCoroutine("StartendturnServer");
        yield return null;
    }
    IEnumerator StartnewgameServer()
    {
        //check every client and server enter a new game!
        while (true)
        {
            while (!PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("GamePhase") && p.CustomProperties.ContainsKey("Ready") && (int)p.CustomProperties["GamePhase"] == 0 && (bool)p.CustomProperties["Ready"]))
            {
                yield return null;
                Debug.Log("SERVER");
            }
            this.hash["GamePhase"] = 1;
            this.hash["Allready"] = true;
            PhotonNetwork.MasterClient.SetCustomProperties(this.hash);
            setServerAllreadytrue();
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator StartendturnServer()
    {
        while (true)
        {
            //check every client and server enter turn and choose a choice!
            while (!PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("Endturn") && (int)p.CustomProperties["GamePhase"] == 1 && (bool)p.CustomProperties["Endturn"]))
            {
                yield return null;
                Debug.Log("SERVER");
            }
            this.hash["GamePhase"] = 2;
            PhotonNetwork.MasterClient.SetCustomProperties(hash);
            setServerAllreadytrue();
            //Evaluate the result!
            string log = Evaluate();
            this.hash["_playersactor"] = _playersactor;
            this.hash["_playerresult"] = _playerresult;
            this.hash["GamePhase"] = 3;
            this.hash["_log"] = log;
            PhotonNetwork.MasterClient.SetCustomProperties(hash);
            setServerAllreadytrue();
            this.turn = this.turn + 1;
        }
    }
    public void newgamebtn_onclick()
    {
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
    public void setServerAllreadyfalse()
    {
        this.hash["Allready"] = false;
        PhotonNetwork.MasterClient.SetCustomProperties(hash);
        while ((bool)PhotonNetwork.MasterClient.CustomProperties["Allready"]) ;
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
    }
    public void setServerAllreadytrue()
    {
        this.hash["Allready"] = true;
        PhotonNetwork.MasterClient.SetCustomProperties(hash);
        while (!(bool)PhotonNetwork.MasterClient.CustomProperties["Allready"]) ;
    }










    private string Evaluate()
    {
        PSSpreprocessing();
        PSSResult();
        return PSSSetResult();
    }
    private void PSSpreprocessing()
    {

        this.paper = new int[20];
        this.scissor = new int[20];
        this.stone = new int[20];
        int paper_i = 0;
        int scissor_i = 0;
        int stone_i = 0;
        players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            Debug.Log("第" + i + "個，玩家總長為" + players.Length);

            if (players[i].CustomProperties.ContainsKey("PSS"))
            {
                switch (players[i].CustomProperties["PSS"])
                {
                    case 0:
                        paper[paper_i] = players[i].ActorNumber;
                        paper_i++;
                        break;
                    case 1:
                        scissor[scissor_i] = players[i].ActorNumber;
                        scissor_i++;
                        break;
                    case 2:
                        stone[stone_i] = players[i].ActorNumber;
                        stone_i++;
                        break;
                }
            }
        }
    }
    private void PSSResult()
    {
        bool _paper_empty = this.paper.All(v => v.Equals(0));
        bool _scissor_empty = this.scissor.All(v => v.Equals(0));
        bool _stone_empty = this.stone.All(v => v.Equals(0));
        if (_paper_empty && !_scissor_empty && !_stone_empty)
        { Debug.Log("Scissor lose,Stone win"); this.result = new int[] { -1, 0, 1 }; }
        else if (!_paper_empty && _scissor_empty && !_stone_empty)
        { Debug.Log("Paper win,Stone lose"); this.result = new int[] { 1, -1, 0 }; }
        else if (!_paper_empty && !_scissor_empty && _stone_empty)
        { Debug.Log("Paper lose,Scissor win"); this.result = new int[] { 0, 1, -1 }; }
        else
        { Debug.Log("Draw"); this.result = new int[] { -1, -1, -1 }; }
    }
    private string PSSSetResult()
    {
        string s = "";
        if (PhotonNetwork.MasterClient.CustomProperties.ContainsKey("_log"))
            s = (string)PhotonNetwork.MasterClient.CustomProperties["_log"];
        s = string.Concat(s, this.turn + " turn\n");
        for (int i = 0; i < players.Length; i++)
        {
            string[] temp = PSSResultInterpret((int)players[i].CustomProperties["PSS"], result);
            this._playersactor[i] = players[i].ActorNumber;
            this._playerresult[i] = temp[1];
            string temp_log = "Actor" + _playersactor[i] + ":出" + temp[0] + "結果為" + _playerresult[i] + "\n";
            s = string.Concat(s, temp_log);
        }
        return s;

    }
    private string[] PSSResultInterpret(int local, int[] result)
    {
        string[] s = new string[2];
        switch (result[local])
        {
            case -1:
                s[1] = "Draw";
                break;
            case 0:
                s[1] = "Lose!";
                break;
            case 1:
                s[1] = "Win!";
                break;
        }
        switch (local)
        {
            case 0:
                s[0] = "Paper";
                break;
            case 1:
                s[0] = "Scissor";
                break;
            case 2:
                s[0] = "Stone";
                break;
        }
        return s;
    }
}
