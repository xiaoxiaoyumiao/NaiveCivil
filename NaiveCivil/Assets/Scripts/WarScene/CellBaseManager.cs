using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class CellBaseManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public WarPlayerManager manager;
    public Color clientColor;
    public int maxStamina = 3;
    
    public int stamina = -1;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // just an example of sync
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(stamina);
        }
        else
        {
            // Network player, receive data
            stamina = (int)stream.ReceiveNext();
        }
    }

    void Start()
    {
        manager = WarPlayerManager.LocalPlayerInstance.GetComponent<WarPlayerManager>();
        if (PhotonNetwork.MasterClient != photonView.Controller)
        {
            GetComponent<SpriteRenderer>().color = clientColor;
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInputs();
        }
    }

    Dictionary<KeyCode, Vector3> moveDict = new Dictionary<KeyCode, Vector3>
    {
        { KeyCode.UpArrow, new Vector3(0, 1, 0) },
        { KeyCode.DownArrow, new Vector3(0, -1, 0) },
        { KeyCode.RightArrow, new Vector3(1, 0, 0) },
        { KeyCode.LeftArrow, new Vector3(-1, 0, 0) }
    };


    void ProcessInputs()
    {
        // if (stamina <= 0) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("End turn");
            // stamina = 0;
            // manager.MakeMove(gameObject, KeyCode.Space, true);
            return;
        }
        foreach (var pair in moveDict)
        {
            if (Input.GetKeyDown(pair.Key))
            {
                this.transform.position += pair.Value;
                // stamina -= 1;
                // Debug.Log($"stamina: {stamina}");
                // manager.MakeMove(gameObject, pair.Key, stamina <= 0);
                break;
            }
        }
    }
    // Start is called before the first frame update
    
}
