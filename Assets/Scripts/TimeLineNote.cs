using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TimeLineNote : MonoBehaviour
{
    public int num;

    public Dictionary<int, int> info;

    void Awake()
    {
        info = new Dictionary<int, int>();
    }

    public void Setting(Dictionary<int, int> info)
    {
        this.info = info;
    }
}
