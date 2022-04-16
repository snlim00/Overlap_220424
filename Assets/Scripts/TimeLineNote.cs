using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLineNote : MonoBehaviour
{
    public bool isSelected = false;

    public int num;

    public Dictionary<int, int> info = new Dictionary<int, int>();

    [HideInInspector] public Image image;

    public static Color32 subSelectColor = new Color32(255, 83, 83, 180);
    public static Color32 stdSelectColor = Color.red;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Setting(Dictionary<int, int> info)
    {
        this.info = info;
    }

    public void Select()
    {
        isSelected = true;

        image.color = subSelectColor;
    }

    public void Deselect()
    {
        isSelected = false;

        image.color = Color.white;
    }
}
