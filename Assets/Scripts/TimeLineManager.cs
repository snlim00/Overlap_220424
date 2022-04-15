using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLineManager : MonoBehaviour
{
    [SerializeField] private GameObject tlNotePrefab;
    [SerializeField] private GameObject gridPrefab;
    [SerializeField] private GameObject timeLine;

    [SerializeField] private Slider beatSlider;

    private List<TimeLineNote> tlNoteList = new List<TimeLineNote>();
    private List<GameObject> gridList = new List<GameObject>();

    public float interval;

    private enum SET_BEAT { UP, DOWN };
    [SerializeField] private float beat;
    private const float minBeat = 1;
    private const float maxBeat = 32f;

    [SerializeField] private bool edtingMode = false;

    private float lastMousePos;
    private bool isScrolling = false;
    private Vector2 tlPos;

    // Start is called before the first frame update
    void Start()
    {
        tlPos = timeLine.transform.position;

        beatSlider.minValue = minBeat;
        beatSlider.maxValue = maxBeat;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchEditingMode();

        if(edtingMode == true)
        {
            SetInterval();

            Scroll();

            SetBeat();
        }
    }

    
    public void DrawGrid()
    {
        GridGeneration();
        SetGridPosition();
        SortGrid(4);
    }

    //그리드 생성
    private void GridGeneration()
    {
        //그려야 할 그리드 개수 세기
        float minBeat = (60 / Level.S.bpm) * (1 / maxBeat);
        int gridCount = (int)(Level.S.songLength / minBeat) + 1;

        //그리드 생성
        for(int i = 0; i < gridCount; ++i)
        {
            GameObject grid = Instantiate(gridPrefab) as GameObject;
            
            grid.transform.SetParent(timeLine.transform);
            grid.transform.localScale = Vector3.one; 

            grid.transform.localPosition = new Vector2(60 / Level.S.bpm * (1 / maxBeat) * interval * i, 0);

            gridList.Add(grid);
        }
    }

    //그리드 위치 설정
    private void SetGridPosition()
    {
        for(int i = 0; i < gridList.Count; ++i)
        {
            gridList[i].transform.localPosition = new Vector2(60 / Level.S.bpm * (1 / maxBeat) * interval * i, 0);
        }
    }

    //비트에 해당하는 그리드만 보이기
    private void SortGrid(float beat)
    {
        this.beat = beat;

        for (int i = 0; i < gridList.Count; ++i)
        {
            gridList[i].transform.localPosition = new Vector2(60 / Level.S.bpm * (1 / maxBeat) * interval * i, 0);

            if(i % (32 * (1 / beat)) != 0)
            {
                gridList[i].SetActive(false);
            }
            else
            {
                gridList[i].SetActive(true);
            }
        }

        gridList[0].SetActive(true);
    }

    public void TLNoteGeneration()
    {
        NoteGeneration();
        SetNotePosition();
        SortNoteNum();
    }

    //노트 생성
    private void NoteGeneration()
    {
        //노트 생성하고 tlNoteList에 추가. 노트의 값 전달
        for (int row = 0; row < Level.S.level.Count; ++row)
        {
            TimeLineNote note = Instantiate(tlNotePrefab).GetComponent<TimeLineNote>();

            note.Setting(Level.S.level[row]);

            tlNoteList.Add(note);
        }
    }

    //맵 파일에 맞게 노트 위치 설정
    private void SetNotePosition()
    {
        //노트의 부모를 timeLine으로 설정하고 노트의 timing에 맞게 위치 설정
        for(int row = 0; row < tlNoteList.Count; ++row)
        {
            tlNoteList[row].transform.SetParent(timeLine.transform);
            tlNoteList[row].transform.localScale = Vector3.one;
            tlNoteList[row].transform.localPosition = new Vector2(tlNoteList[row].info[KEY.TIMING] * 0.001f * interval, 0);
        }
    }
     
    //노트를 순서대로 정렬
    private void SortNoteNum()
    {
        //노트의 인덱스와 num을 x축을 기준으로 하여 오름차순 정렬
        tlNoteList.Sort((A, B) => A.transform.position.x.CompareTo(B.transform.position.x));

        for (int i = 0; i < tlNoteList.Count; ++i)
        {
            tlNoteList[i].num = i;
        }
    }

    private void SwitchEditingMode()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            edtingMode = !edtingMode;
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

            SetNotePosition();
            SortNoteNum();
        }
    }

    private void SetInterval()
    {
        //마우스 휠을 통해 interval 값 증가, 감소 (추후 해당 값을 반영하여 Grid와 note의 위치를 조정하도록 할 것)
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            interval += Input.GetAxis("Mouse ScrollWheel") * 100;
            SetGridPosition();
            SetNotePosition();
        }
    }

    private void SetBeat()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            beat = beat * 2;
            if (beat > maxBeat) beat = maxBeat;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            beat = beat * 0.5f;
            if (beat < minBeat) beat = minBeat;
        }
        else return;

        SortGrid(beat);
        RenewalBeatSlider();
    }

    private void RenewalBeatSlider()
    {
        beatSlider.value = beat;
    }
}
