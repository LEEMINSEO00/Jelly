using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    public int id;
    public int level;
    public float exp;
    public Vector3 pos;

    public Data(Vector3 pos, int id, int level, float exp)
    {
        this.pos = pos;
        this.id = id;
        this.level = level;
        this.exp = exp;

    }
}
