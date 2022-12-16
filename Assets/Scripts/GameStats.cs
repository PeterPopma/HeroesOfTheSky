using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerWon_
{
    Red,
    Blue,
    Draw
}

public static class GameStats
{
    public static string GameOverReason;
    public static PlayerWon_ PlayerWon;
    public static int HealthRed;
    public static int HealthBlue;
    public static int NumPlanesRed;
    public static int NumPlanesBlue;
    public static float DistanceRed;
    public static float DistanceBlue;
}
