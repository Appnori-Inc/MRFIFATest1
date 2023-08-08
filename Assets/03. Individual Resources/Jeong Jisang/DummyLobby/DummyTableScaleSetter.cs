using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BallPool;
using Billiards;


public class DummyTableScaleSetter : MonoBehaviour
{
    [SerializeField]
    private Transform Table;

    void Start()
    {
        switch(LevelLoader.CurrentMatchInfo.gameType)
        {
            case GameType.PocketBilliards:
                Table.localScale = Vector3.one;
                break;

            case GameType.CaromThree:
                Table.localScale = new Vector3(1.117f, 1.117f, 1);
                break;
            case GameType.CaromFour:
                Table.localScale = Vector3.one;
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
