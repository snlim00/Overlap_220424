using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLineManager : MonoBehaviour
{
    [SerializeField] private GameObject tlNotePrefab;
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private GameObject timeLine;

    private List<TimeLineNote> tlNoteList = new List<TimeLineNote>();

    public float interval;
    public float beat;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            interval += Input.GetAxis("Mouse ScrollWheel") * 50;
            Debug.Log(interval);
        }
    }

    public void DrawGrid()
    {
        for(int i = 0; i < 1000; ++i)
        {
            GameObject grid = Instantiate(gridPrefab) as GameObject;

            grid.transform.SetParent(timeLine.transform);
            grid.transform.localScale = Vector3.one; 

            //grid.transform.localPosition = new Vector2(60 / Level.S.bpm * i * interval * (1 / beat), 0); 
            grid.transform.localPosition = new Vector2(60 / Level.S.bpm * (1 / beat) * interval * i, 0);
        }

        TLNoteGeneration();
    }

    void TLNoteGeneration()
    {
        for (int row = 0; row < Level.S.level.Count; ++row)
        {
            TimeLineNote note = Instantiate(tlNotePrefab).GetComponent<TimeLineNote>();

            tlNoteList.Add(note);
        }

        SetNotePosition();
    }

    public void SetNotePosition()
    {
        for(int row = 0; row < tlNoteList.Count; ++row)
        {
            Dictionary<int, int> thisRow = Level.S.level[row];

            tlNoteList[row].Setting(thisRow);

            tlNoteList[row].transform.SetParent(timeLine.transform);
            tlNoteList[row].transform.localScale = Vector3.one;
        }

        SetNoteNum();
    }

    private void SetNoteNum()
    {
        tlNoteList.Sort((A, B) => A.transform.position.x.CompareTo(B.transform.position.x));

        for (int i = 0; i < tlNoteList.Count; ++i)
        {
            tlNoteList[i].num = i;
        }
    }
}
