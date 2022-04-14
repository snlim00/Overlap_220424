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
    private List<GameObject> grid = new List<GameObject>();

    public float interval;
    public float beat;

    [SerializeField] private bool edtingMode = false;

    private float lastMousePos;
    private bool isScrolling = false;
    private Vector2 tlPos;

    // Start is called before the first frame update
    void Start()
    {
        tlPos = timeLine.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            edtingMode = !edtingMode;
        }

        if(edtingMode == true)
        {
            SetInterval();

            Scroll();
        }
    }

    public void DrawGrid()
    {
        //1000개(임시)의 그리드 그리기
        for(int i = 0; i < 1000; ++i)
        {
            GameObject grid = Instantiate(gridPrefab) as GameObject;

            grid.transform.SetParent(timeLine.transform);
            grid.transform.localScale = Vector3.one; 

            grid.transform.localPosition = new Vector2(60 / Level.S.bpm * (1 / beat) * interval * i, 0);

            //4번째 그리드 붉은 색으로 강조
            if (i % 4 == 0)
                grid.GetComponent<Image>().color = Color.red;
            else if (i % 2 == 0)
                grid.GetComponent<Image>().color = Color.yellow;
        }

        TLNoteGeneration();
    }

    public void TLNoteGeneration()
    {
        //노트 생성하고 tlNoteList에 추가. 노트의 값 전달
        for (int row = 0; row < Level.S.level.Count; ++row)
        {
            TimeLineNote note = Instantiate(tlNotePrefab).GetComponent<TimeLineNote>();

            note.Setting(Level.S.level[row]);


            tlNoteList.Add(note);
        }

        SetNotePosition();
    }

    public void SetNotePosition()
    {
        //노트의 부모를 timeLine으로 설정하고 노트의 timing에 맞게 위치 설정
        for(int row = 0; row < tlNoteList.Count; ++row)
        {
            tlNoteList[row].transform.SetParent(timeLine.transform);
            tlNoteList[row].transform.localScale = Vector3.one;
            tlNoteList[row].transform.localPosition = new Vector2(tlNoteList[row].info[KEY.TIMING] * 0.001f * interval, 0);
        }

        SetNoteNum();
    }
     
    private void SetNoteNum()
    {
        //노트의 인덱스와 num을 x축을 기준으로 하여 오름차순 정렬
        tlNoteList.Sort((A, B) => A.transform.position.x.CompareTo(B.transform.position.x));

        for (int i = 0; i < tlNoteList.Count; ++i)
        {
            tlNoteList[i].num = i;
        }
    }

    private void Scroll()
    {
        //우클릭이 시작되면 스크롤을 활성화
        if (Input.GetMouseButtonDown(1) == true)
        {
            isScrolling = true;
            lastMousePos = Input.mousePosition.x;
            tlPos = timeLine.transform.position;
        }
        else if (Input.GetMouseButtonUp(1) == true)
            isScrolling = false;

        //마우스의 마지막 위치와 현재 위치를 비교하여 변한 값 만큼 타임라인 위치 이동
        if (isScrolling == true)
        {
            tlPos.x -= lastMousePos - Input.mousePosition.x;
            lastMousePos = Input.mousePosition.x;

            timeLine.transform.position = tlPos;
        }
    }

    private void SetInterval()
    {
        //마우스 휠을 통해 interval 값 증가, 감소 (추후 해당 값을 반영하여 Grid와 note의 위치를 조정하도록 할 것)
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            interval += Input.GetAxis("Mouse ScrollWheel") * 50;
            Debug.Log(interval);
        }
    }
}
