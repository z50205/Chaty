using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameHolder : MonoBehaviourPunCallbacks
{
    public GameObject ev_pivot;
    [SerializeField] public Button _endturnbtn;
    [SerializeField] public Button _newgamebtn;
    public int gamephase;
    [SerializeField] public Text _psslog;
    [SerializeField] int[] _playersactor = new int[20];
    [SerializeField] string[] _playerresult = new string[20];
    public int turn;
    //1:決定開始新遊戲
    //2:完成遞交階段
    //3:完成分析階段
    //4:完成重新設定局階段
    //5:完成重新設定新遊戲階段
    Player[] players = PhotonNetwork.PlayerList;
    [SerializeField] int[] paper;
    [SerializeField] int[] scissor;
    [SerializeField] int[] stone;
    [SerializeField] int[] result;
    [SerializeField] private ToggleGroup _togglegroup;
    // [SerializeField] public bool _endturn;
    [SerializeField] public bool _over;
    [SerializeField] ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
    private void Start()
    {
        turn = 1;
        gamephase = 0;
        _endturnbtn.interactable = false;
        ev_pivot.GetComponent<Text>().enabled = false;
    }
    private void Update()
    {
        switch (this.gamephase)
        {
            case 0:
                WaitNewGame();
                RenewGamePhase();
                break;
            case 1:
                Endturn();
                RenewGamePhase();
                break;
            case 2:
                Evaluate();
                RenewGamePhase();
                break;
            case 3:
                RenewGamePhase();
                PSSRenewturn();
                break;
            case 5:
                break;
        }
    }
    public void RenewGamePhase()
    {
        if (PhotonNetwork.MasterClient.CustomProperties.ContainsKey("GamePhase") && !PhotonNetwork.IsMasterClient)//&& (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Phase");
            this.gamephase = (int)PhotonNetwork.MasterClient.CustomProperties["GamePhase"];
            this.hash["GamePhase"] = PhotonNetwork.MasterClient.CustomProperties["GamePhase"];
            PhotonNetwork.LocalPlayer.SetCustomProperties(this.hash);
            // Debug.Log(this.hash.ToString());
        }
        // Debug.Log("gamephase:" + gamephase);
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties.ToString());
        switch (gamephase)
        {
            case 1:
                if (PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("Ready") && (bool)p.CustomProperties["Ready"]))
                {
                    _newgamebtn.GetComponentInChildren<Text>().text = "Connecting and Play！";
                }
                break;
            case 2:
                if (PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("Endturn") && (bool)p.CustomProperties["Endturn"]))
                {
                    _psslog.text = (string)PhotonNetwork.MasterClient.CustomProperties["_log"];
                    _endturnbtn.GetComponentInChildren<Text>().text = "Evaluating！";
                }
                break;
            case 3:
                break;
        }

    }
    private void InterfaceChangeEvaluate()
    {
        int pivot = 0;
        this._playersactor = (int[])PhotonNetwork.MasterClient.CustomProperties["_playersactor"];
        this._playerresult = (string[])PhotonNetwork.MasterClient.CustomProperties["_playerresult"];
        for (int i = 0; i < this._playersactor.Length; i++)
        {
            if (this._playersactor[i] == PhotonNetwork.LocalPlayer.ActorNumber) pivot = i;
        }
        _endturnbtn.GetComponentInChildren<Text>().text = _playerresult[pivot];
        if (_playerresult[pivot] == "Lose!")
        {
            this._over = true;
            hash["_over"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
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
    // public void getstate(bool myturn)
    // {
    //     if (myturn)
    //         ev_pivot.GetComponent<Text>().enabled = true;
    //     else
    //         ev_pivot.GetComponent<Text>().enabled = false;
    // }
    // private void getallplayer()
    // {
    //     foreach (Player player in PhotonNetwork.PlayerList)
    //     {
    //         Debug.Log(player.ActorNumber);
    //     }
    // }
    //讓
    public void Onclick_WaitNewGame()
    {
        this.hash["Ready"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        _newgamebtn.interactable = false;
        _endturnbtn.interactable = true;
        _newgamebtn.GetComponentInChildren<Text>().text = "Ready To Play！";
    }
    public void WaitNewGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("Ready") && (bool)p.CustomProperties["Ready"])) return;
        this.hash["GamePhase"] = 1;
        this.gamephase = 1;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
    public void Onclick_Endturn()
    {
        //if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["Endturn"]==false)
        this.hash["Endturn"] = true;
        this.hash["PSS"] = GetToggleIndex(_togglegroup);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        _endturnbtn.interactable = false;
        _endturnbtn.GetComponentInChildren<Text>().text = "waiting other players！";
        Debug.Log("Endturn:" + PhotonNetwork.LocalPlayer.CustomProperties.ToString());
    }
    public void Endturn()
    {
        if (!PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("GamePhase") && ((int)p.CustomProperties["GamePhase"] == 1))) return;
        // Debug.Log("00");
        if (!PhotonNetwork.IsMasterClient) return;
        //Debug.Log("01:" + !PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("Endturn") && (bool)p.CustomProperties["Endturn"]));
        if (!PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("Endturn") && (bool)p.CustomProperties["Endturn"])) return;
        //Debug.Log("02:" + !PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("Endturn") && (bool)p.CustomProperties["Endturn"]));
        // Debug.Log(hash.ToString());
        hash["_log"] = string.Concat((string)PhotonNetwork.MasterClient.CustomProperties["_log"], this.turn, " turn\n");
        //hash["_turn"] = this.turn;
        this.hash["GamePhase"] = 2;
        this.gamephase = 2;
        //Debug.Log(hash.ToString());
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
    // public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    // {
    //     if (!PhotonNetwork.IsMasterClient) return;

    //     if (!changedProps.ContainsKey("Ready")) return;

    //     CheckAllPlayersReady();
    // }
    // private void CheckAllPlayersReady()
    // {
    //     var players = PhotonNetwork.PlayerList;
    //     // This is just using a shorthand via Linq instead of having a loop with a counter
    //     // for checking whether all players in the list have the key "Ready" in their custom properties
    //     if (players.All(p => p.CustomProperties.ContainsKey("Endturn") && (bool)p.CustomProperties["Ready"]))
    //     {
    //         _endturnphase = true;
    //         Debug.Log("All players are ready!");
    //     }
    // }
    private void Evaluate()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("GamePhase") && ((int)p.CustomProperties["GamePhase"] == 2))) return;
        PSSpreprocessing();
        PSSResult();
        string log = PSSSetResult();
        //Debug.Log(log);
        hash["_playersactor"] = _playersactor;
        hash["_playerresult"] = _playerresult;
        hash["GamePhase"] = 3;
        hash["_log"] = log;
        PhotonNetwork.SetPlayerCustomProperties(hash);
        if (!((int)PhotonNetwork.LocalPlayer.CustomProperties["GamePhase"] == 3)) return;
        this.gamephase = 3;
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
            //Debug.Log(players[i].CustomProperties.ToString());

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
        //Debug.Log("paper:" + paper[0]);
        //Debug.Log("stone" + stone[0]);
        //Debug.Log("1:" + _paper_empty + "2:" + _scissor_empty + "3:" + _stone_empty);
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
        for (int i = 0; i < players.Length; i++)
        {
            string[] temp = PSSResultInterpret((int)players[i].CustomProperties["PSS"], result);
            this._playersactor[i] = players[i].ActorNumber;
            this._playerresult[i] = temp[1];
            string temp_log = "Actor" + _playersactor[i] + ":出" + temp[0] + "結果為" + _playerresult[i] + "\n";
            s = string.Concat(s, temp_log);
            //Debug.Log(s);
            //Debug.Log(temp_log);
            //Debug.Log("Actor" + _playersactor[i] + ":" + _playerresult[i]);
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
    private void PSSRenewturn()
    {
        if (!PhotonNetwork.PlayerList.All(p => p.CustomProperties.ContainsKey("GamePhase") && ((int)p.CustomProperties["GamePhase"] == 3))) return;
        if (!PhotonNetwork.IsMasterClient)
        {
            hash["Endturn"] = false;
            this.gamephase = 1;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            _endturnbtn.interactable = true;
            return;
        }
        hash["Endturn"] = false;
        hash["GamePhase"] = 1;
        hash["_turn"] = this.turn;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        if (!((int)PhotonNetwork.LocalPlayer.CustomProperties["GamePhase"] == 1) && !(bool)PhotonNetwork.LocalPlayer.CustomProperties["Endturn"])return;
        this.gamephase = 1;
        this.turn = this.turn + 1;
        _endturnbtn.interactable = true;
        InterfaceChangeEvaluate();
        _psslog.text = (string)PhotonNetwork.MasterClient.CustomProperties["_log"];
        Debug.Log("checkrenew:" + ((int)PhotonNetwork.LocalPlayer.CustomProperties["GamePhase"] == 1));
    }
    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     if(stream.IsWriting)
    //     {
    //         stream.SendNext(this.endturn);
    //     }
    //     else if(stream.IsReading)
    //     {
    //         this.endturn=(int)stream.ReceiveNext();
    //     }
    // }

}
