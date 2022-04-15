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

    //�׸��� ����
    private void GridGeneration()
    {
        //�׷��� �� �׸��� ���� ����
        float minBeat = (60 / Level.S.bpm) * (1 / maxBeat);
        int gridCount = (int)(Level.S.songLength / minBeat) + 1;

        //�׸��� ����
        for(int i = 0; i < gridCount; ++i)
        {
            GameObject grid = Instantiate(gridPrefab) as GameObject;
            
            grid.transform.SetParent(timeLine.transform);
            grid.transform.localScale = Vector3.one; 

            grid.transform.localPosition = new Vector2(60 / Level.S.bpm * (1 / maxBeat) * interval * i, 0);

            gridList.Add(grid);
        }
    }

    //�׸��� ��ġ ����
    private void SetGridPosition()
    {
        for(int i = 0; i < gridList.Count; ++i)
        {
            gridList[i].transform.localPosition = new Vector2(60 / Level.S.bpm * (1 / maxBeat) * interval * i, 0);
        }
    }

    //��Ʈ�� �ش��ϴ� �׸��常 ���̱�
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

    //��Ʈ ����
    private void NoteGeneration()
    {
        //��Ʈ �����ϰ� tlNoteList�� �߰�. ��Ʈ�� �� ����
        for (int row = 0; row < Level.S.level.Count; ++row)
        {
            TimeLineNote note = Instantiate(tlNotePrefab).GetComponent<TimeLineNote>();

            note.Setting(Level.S.level[row]);

            tlNoteList.Add(note);
        }
    }

    //�� ���Ͽ� �°� ��Ʈ ��ġ ����
    private void SetNotePosition()
    {
        //��Ʈ�� �θ� timeLine���� �����ϰ� ��Ʈ�� timing�� �°� ��ġ ����
        for(int row = 0; row < tlNoteList.Count; ++row)
        {
            tlNoteList[row].transform.SetParent(timeLine.transform);
            tlNoteList[row].transform.localScale = Vector3.one;
            tlNoteList[row].transform.localPosition = new Vector2(tlNoteList[row].info[KEY.TIMING] * 0.001f * interval, 0);
        }
    }
     
    //��Ʈ�� ������� ����
    private void SortNoteNum()
    {
        //��Ʈ�� �ε����� num�� x���� �������� �Ͽ� �������� ����
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
        //��Ŭ���� ���۵Ǹ� ��ũ���� Ȱ��ȭ
        if (Input.GetMouseButtonDown(1) == true)
        {
            isScrolling = true;
            lastMousePos = Input.mousePosition.x;
            tlPos = timeLine.transform.position;
        }
        else if (Input.GetMouseButtonUp(1) == true)
            isScrolling = false;

        //���콺�� ������ ��ġ�� ���� ��ġ�� ���Ͽ� ���� �� ��ŭ Ÿ�Ӷ��� ��ġ �̵�
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
        //���콺 ���� ���� interval �� ����, ���� (���� �ش� ���� �ݿ��Ͽ� Grid�� note�� ��ġ�� �����ϵ��� �� ��)
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
