using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class WarPlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static TurnManager turnManager;
    public static GameObject LocalPlayerInstance;
    
    public GameObject cellPrefab;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
            turnManager = GetComponent<TurnManager>();
            turnManager.TurnManagerListener = new WarPlayerListener(this);

            PhotonNetwork.Instantiate("Prefabs/"+cellPrefab.name, Vector3.zero, Quaternion.identity, 0);
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(gameObject);

        if (PhotonNetwork.IsMasterClient)
        {
            turnManager.ResetTurnCount();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MakeMove(GameObject sender, object move, bool finished)
    {
        turnManager.SendMove(move, finished);
    }

    public void CompleteTurn(int turn)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            turnManager.BeginTurn();
        }
    }
    public void BeginTurn(int turn)
    {
        Debug.Log("BeginTurn called.");
        
    }
}

public class WarPlayerListener : ITurnManagerCallbacks
{
    WarPlayerManager manager;
    public WarPlayerListener(WarPlayerManager playerManager)
    {
        manager = playerManager;
    }

    public void OnPlayerFinished(Player player, int turn, object move)
    {

    }

    public void OnPlayerMove(Player player, int turn, object move)
    {

    }

    public void OnTurnBegins(int turn)
    {
        Debug.Log("OnTurnBegins called.");
        manager.BeginTurn(turn);
    }

    public void OnTurnCompleted(int turn)
    {
        manager.CompleteTurn(turn);
    }

    public void OnTurnTimeEnds(int turn)
    {

    }
}
