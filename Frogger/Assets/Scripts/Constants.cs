using System;
using UnityEngine;

public static class Constants
{

    public const string movingSurfaceTag = "MovingSurface";
    public const string goalTag = "Goal";

    public const int LAYER_GROUND = 8;
    public const int LAYER_ENEMY = 9;
    public const int LAYER_WALL = 12;
    //public const int LAYER_KILLER = 12;

    static int groundMask = LayerMask.GetMask(LayerMask.LayerToName(LAYER_GROUND));
    static int enemyMask = LayerMask.GetMask(LayerMask.LayerToName(LAYER_ENEMY));
    static int wallMask = LayerMask.GetMask(LayerMask.LayerToName(LAYER_WALL));
    //static int killerMask = LayerMask.GetMask(LayerMask.LayerToName(LAYER_KILLER));
    public static int GroundMask { get => groundMask; }
    public static int EnemyMask { get => enemyMask; }
    public static int WallMask { get => wallMask; }
    //public static int KillerMask { get => killerMask; }
}