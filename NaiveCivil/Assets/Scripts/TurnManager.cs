using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TurnManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public int Turn
    {
        get { return PhotonNetwork.CurrentRoom.GetTurn(); }
        private set
        {
            PhotonNetwork.CurrentRoom.SetTurn(value, true);
        }
    }

    bool mIsFinishedByMe = false;
    public bool IsFinishedByMe
    {
        get { return mIsFinishedByMe; }
    }

    public void OnEvent(EventData photonEvent)
    {
        this.ProcessOnEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender);
    }

    void ProcessOnEvent(byte eventCode, object content, int senderId)
    {
        if (senderId == -1)
        {
            return;
        }

        Player sender = PhotonNetwork.CurrentRoom.GetPlayer(senderId);

    }

    public void SendMove(object move, bool finished)
    {
        if (IsFinishedByMe)
        {
            Debug.LogWarning("Can't SendMove. Turn is finished by this player.");
            return;
        }
    }

    public void BeginTurn()
    {
        Turn = this.Turn + 1; // note: this will set a property in the room, which is available to the other players.
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public interface IPunTurnManagerCallbacks
{
    /// <summary>
    /// Called the turn begins event.
    /// </summary>
    /// <param name="turn">Turn Index</param>
    void OnTurnBegins(int turn);

    /// <summary>
    /// Called when a turn is completed (finished by all players)
    /// </summary>
    /// <param name="turn">Turn Index</param>
    void OnTurnCompleted(int turn);

    /// <summary>
    /// Called when a player moved (but did not finish the turn)
    /// </summary>
    /// <param name="player">Player reference</param>
    /// <param name="turn">Turn Index</param>
    /// <param name="move">Move Object data</param>
    void OnPlayerMove(Player player, int turn, object move);

    /// <summary>
    /// When a player finishes a turn (includes the action/move of that player)
    /// </summary>
    /// <param name="player">Player reference</param>
    /// <param name="turn">Turn index</param>
    /// <param name="move">Move Object data</param>
    void OnPlayerFinished(Player player, int turn, object move);


    /// <summary>
    /// Called when a turn completes due to a time constraint (timeout for a turn)
    /// </summary>
    /// <param name="turn">Turn index</param>
    void OnTurnTimeEnds(int turn);
}


public static class TurnExtensions
{
    /// <summary>
    /// currently ongoing turn number
    /// </summary>
    public static readonly string TurnPropKey = "Turn";

    /// <summary>
    /// start (server) time for currently ongoing turn (used to calculate end)
    /// </summary>
    public static readonly string TurnStartPropKey = "TStart";

    /// <summary>
    /// Finished Turn of Actor (followed by number)
    /// </summary>
    public static readonly string FinishedTurnPropKey = "FToA";
    public static void SetTurn(this Room room, int turn, bool setStartTime = false)
    {
        if (room == null || room.CustomProperties == null)
        {
            return;
        }

        Hashtable turnProps = new Hashtable();
        turnProps[TurnPropKey] = turn;
        if (setStartTime)
        {
            turnProps[TurnStartPropKey] = PhotonNetwork.ServerTimestamp;
        }

        room.SetCustomProperties(turnProps);
    }

    /// <summary>
    /// Gets the current turn from a RoomInfo
    /// </summary>
    /// <returns>The turn index </returns>
    /// <param name="room">RoomInfo reference</param>
    public static int GetTurn(this RoomInfo room)
    {
        if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnPropKey))
        {
            return 0;
        }

        return (int)room.CustomProperties[TurnPropKey];
    }


    /// <summary>
    /// Returns the start time when the turn began. This can be used to calculate how long it's going on.
    /// </summary>
    /// <returns>The turn start.</returns>
    /// <param name="room">Room.</param>
    public static int GetTurnStart(this RoomInfo room)
    {
        if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnStartPropKey))
        {
            return 0;
        }

        return (int)room.CustomProperties[TurnStartPropKey];
    }

    /// <summary>
    /// gets the player's finished turn (from the ROOM properties)
    /// </summary>
    /// <returns>The finished turn index</returns>
    /// <param name="player">Player reference</param>
    public static int GetFinishedTurn(this Player player)
    {
        Room room = PhotonNetwork.CurrentRoom;
        if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnPropKey))
        {
            return 0;
        }

        string propKey = FinishedTurnPropKey + player.ActorNumber;
        return (int)room.CustomProperties[propKey];
    }

    /// <summary>
    /// Sets the player's finished turn (in the ROOM properties)
    /// </summary>
    /// <param name="player">Player Reference</param>
    /// <param name="turn">Turn Index</param>
    public static void SetFinishedTurn(this Player player, int turn)
    {
        Room room = PhotonNetwork.CurrentRoom;
        if (room == null || room.CustomProperties == null)
        {
            return;
        }

        string propKey = FinishedTurnPropKey + player.ActorNumber;
        Hashtable finishedTurnProp = new Hashtable();
        finishedTurnProp[propKey] = turn;

        room.SetCustomProperties(finishedTurnProp);
    }
}