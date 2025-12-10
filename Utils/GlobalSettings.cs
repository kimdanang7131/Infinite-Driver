
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalSettings
{
    static float[] carLanes = new float[4]{-20f, -7f, 7f, 20f};
    public static float[] CarLanes => carLanes;

    static float mapPosY = -35f;
    public static float MapPosY => mapPosY;

    public static int maxFrontAdCount = 4;
}