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
        //1000��(�ӽ�)�� �׸��� �׸���
        for(int i = 0; i < 1000; ++i)
        {
            GameObject grid = Instantiate(gridPrefab) as GameObject;

            grid.transform.SetParent(timeLine.transform);
            grid.transform.localScale = Vector3.one; 

            grid.transform.localPosition = new Vector2(60 / Level.S.bpm * (1 / beat) * interval * i, 0);

            //4��° �׸��� ���� ������ ����
            if (i % 4 == 0)
                grid.GetComponent<Image>().color = Color.red;
            else if (i % 2 == 0)
                grid.GetComponent<Image>().color = Color.yellow;
        }

        TLNoteGeneration();
    }

    public void TLNoteGeneration()
    {
        //��Ʈ �����ϰ� tlNoteList�� �߰�. ��Ʈ�� �� ����
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
        //��Ʈ�� �θ� timeLine���� �����ϰ� ��Ʈ�� timing�� �°� ��ġ ����
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
        //��Ʈ�� �ε����� num�� x���� �������� �Ͽ� �������� ����
        tlNoteList.Sort((A, B) => A.transform.position.x.CompareTo(B.transform.position.x));

        for (int i = 0; i < tlNoteList.Count; ++i)
        {
            tlNoteList[i].num = i;
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
        }
    }

    private void SetInterval()
    {
        //���콺 ���� ���� interval �� ����, ���� (���� �ش� ���� �ݿ��Ͽ� Grid�� note�� ��ġ�� �����ϵ��� �� ��)
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            interval += Input.GetAxis("Mouse ScrollWheel") * 50;
            Debug.Log(interval);
        }
    }
}
