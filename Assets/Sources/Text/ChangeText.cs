using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChangeText : MonoBehaviourPun,IPunObservable
{
    private void Start() {
        this.transform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(GetComponent<Text>().text);
        }
        else if(stream.IsReading)
        {
            GetComponent<Text>().text=(string)stream.ReceiveNext();
        }
    }
}
