using System;
using System.Collections.Generic;
using UnityEngine;

public class GameRaceResultContainer : GameObjectBehavior {

    //public List<Race.RaceResults> results;
    //public List<Race.RaceResults.LapDetails> lapDetails;
    public string trackName;

    public bool networked;

    private void Start() {
        networked = false;
        DontDestroyOnLoad(gameObject.transform);
    }

    public void ClearResults() {

        //results = new List<Race.RaceResults>();
        //lapDetails = new List<Race.RaceResults.LapDetails>();
    }
}