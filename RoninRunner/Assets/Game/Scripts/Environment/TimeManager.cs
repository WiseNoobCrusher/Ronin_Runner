using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Private Variables
    private static float player = 1.0f;
    private static float global = 1.0f;

    public float GetGlobalTimeScale()
    {
        float output = global;
        return output;
    }

    public float GetPlayerTimeScale()
    {
        float output = player;
        return output;
    }

    public void SetGlobalTimeScale(float input) { global = input; }

    public void SetPlayerTimeScale(float input) { player = input; }
}
