using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerPrefab : MonoBehaviourPunCallbacks
{
    float movespeed = 5f;
    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PhotonView>().IsMine)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
                transform.Translate(-movespeed * Time.deltaTime, 0, 0);
            else if (Input.GetKey(KeyCode.RightArrow))
                transform.Translate(movespeed * Time.deltaTime, 0, 0);
            else if (Input.GetKey(KeyCode.UpArrow))
                transform.Translate( 0,movespeed * Time.deltaTime, 0);
            else if (Input.GetKey(KeyCode.DownArrow))
                transform.Translate(0,-movespeed * Time.deltaTime, 0);
        }
    }
}
