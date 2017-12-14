using System;
using System.Collections;
using System.Collections.Generic;
// using Engine.Data.Json;
using Engine.Utility;
using UnityEngine;

public enum GamePlayerType {
    PLAYER_HUMAN = 0,
    PLAYER_AI = 1,
    PLAYER_NETWORK = 9
}

public class GameRaceResultPlayer : IComparable {
    public string udid;
    public string levelName;
    public string playerName;
    public double totalTime;
    public List<double> lapTimes;
    public GamePlayerType playerType = GamePlayerType.PLAYER_HUMAN;
    public int place;

    public GameRaceResultPlayer() {
        Reset();
    }

    public double totalTimeAccumulated {
        get {
            double total = 0;
            foreach (float itemValue in lapTimes) {
                total += itemValue;
            }
            return total;
        }
    }

    public bool isHuman {
        get { return playerType == GamePlayerType.PLAYER_HUMAN ? true : false; }
    }

    public bool isAi {
        get { return playerType == GamePlayerType.PLAYER_AI ? true : false; }
    }

    public bool isNetworked {
        get { return playerType == GamePlayerType.PLAYER_NETWORK ? true : false; }
    }

    public bool isWinner {
        get { return place == 1; }
    }

    public void Reset() {
        udid = UniqueUtil.Instance.currentUniqueId;
        levelName = "";
        playerName = "";
        totalTime = 0;
        lapTimes = new List<double>();
        place = 1;
    }

    public int CompareTo(object comparedTo) {
        if (comparedTo == null)
            return 0;

        if (this.place == ((GameRaceResultPlayer)comparedTo).place)
            return 0;
        else
            return this.place > ((GameRaceResultPlayer)comparedTo).place ? 1 : -1;
    }
}

public class GameRaceResultsData {
    public List<GameRaceResultPlayer> players;

    public GameRaceResultsData() {
        Reset();
    }

    public GameRaceResultPlayer GetHumanPlayer() {
        foreach (GameRaceResultPlayer player in players) {
            if (player.isHuman) {
                return player;
            }
        }
        return null;
    }

    public void Reset() {
        players = new List<GameRaceResultPlayer>();
    }

    public string ToJson() {
        return this.ToJson();
    }
}