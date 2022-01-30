using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameObject LocalPlayerInstance;
    public Material clientMaterial;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);

        if (!PhotonNetwork.IsMasterClient)
        {
            GetComponent<MeshRenderer>().material = clientMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInputs();
        }
    }

    Dictionary<KeyCode, Vector3> moveDict = new Dictionary<KeyCode, Vector3>
    {
        { KeyCode.UpArrow, new Vector3(-1, 0, 0) },
        { KeyCode.DownArrow, new Vector3(1, 0, 0) },
        { KeyCode.RightArrow, new Vector3(0, 0, 1) },
        { KeyCode.LeftArrow, new Vector3(0, 0, -1) }
    };
    void ProcessInputs()
    {
        foreach (var pair in moveDict)
        {
            if (Input.GetKeyDown(pair.Key))
            {
                this.transform.position += pair.Value;
                break;
            }
        }
    }

    
}
