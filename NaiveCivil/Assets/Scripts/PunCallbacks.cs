using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunCallbacks : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        // PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.CreateRoom("MyMatch");
        PhotonNetwork.JoinRoom("");
    }

    // Update is called once per frame
    void Update()
    {

    }
    // ...
}

