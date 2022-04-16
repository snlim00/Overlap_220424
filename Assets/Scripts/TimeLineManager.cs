using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class TimeLineManager : MonoBehaviour
{
    //에디팅 모드 활성화 관련 변수
    [SerializeField] private bool editingMode = false;
    [SerializeField] private Toggle editingToggle;

    //노트와 그리드 생성을 위한 프리팹
    [SerializeField] private GameObject tlNotePrefab;
    [SerializeField] private GameObject gridPrefab;

    //타임라인 오브젝트
    [SerializeField] private GameObject timeLine;

    //현재 설정된 비트 나눗수를 표시하는 UI
    [SerializeField] private Slider beatSlider;
    [SerializeField] private Text beatText;

    //생성된 노트와 그리드를 담아두는 리스트
    private List<TimeLineNote> tlNoteList = new List<TimeLineNote>();
    private List<GameObject> gridList = new List<GameObject>();

    //그리드와 리스트의 간격 배율
    public float interval;
    private float intervalSensivisity = 100;

    //비트 나눗수 관련 변수
    private int selectedBeat = 2;
    private float[] beat = { 1, 2, 4, 8, 16, 32 };
    private const float minBeat = 1;
    private const float maxBeat = 32f;

    //스크롤 구현에 필요한 변수
    private float lastMousePos;
    private bool isScrolling = false;
    private Vector2 tlPos;

    //노트 선택, 배치, 삭제 등에 관련된 변수
    [SerializeField] private List<TimeLineNote> selectedNoteList = new List<TimeLineNote>();
    [SerializeField] private TimeLineNote standardNote = null;

    //노트 정보 수정에 관련된 변수
    [SerializeField] private GameObject infoPanel;

    [SerializeField] private GameObject infoDropdownPref;
    [SerializeField] private GameObject infoInputFieldPref;
    private Dropdown[] infoArrDropdown = new Dropdown[11];
    private InputField[] infoArrInputField = new InputField[11];

    void Start()
    {
        InitVariable();
    }

    void Update()
    {
        SwitchEditingMode();

        if (editingMode == true)
        {
            SetInterval();

            Scroll();

            SetBeat();
        }
    }

    //초기화 관련 함수
    #region
    private void InitVariable()
    {
        tlPos = timeLine.transform.position;

        editingToggle.isOn = editingMode;

        beatSlider.minValue = 0;
        beatSlider.maxValue = beat.Length - 1;

        for(int i = 0; i < KEY.COUNT; ++i)
        {
            infoArrDropdown[i] = null;
            infoArrInputField[i] = null;
        }
    }

    public void Init()
    {
        DrawGrid();
        ShowGrid(4);

        TLNoteGeneration();

        InfoInit();
    }
    #endregion

    //그리드 생성 관련 함수
    #region
    private void DrawGrid()
    {
        GridGeneration();
        SetGridPosition();
    }

    //그리드 생성
    private void GridGeneration()
    {
        //그려야 할 그리드 개수 세기
        float minBeat = (60 / Level.S.bpm) * (1 / maxBeat);
        int gridCount = (int)(Level.S.songLength / minBeat) + 1;

        //그리드 생성
        for (int i = 0; i < gridCount; ++i)
        {
            GameObject grid = Instantiate(gridPrefab) as GameObject;

            grid.transform.SetParent(timeLine.transform);
            grid.transform.localScale = Vector3.one;

            grid.transform.localPosition = new Vector2(60 / Level.S.bpm * (1 / maxBeat) * interval * i, 0);

            gridList.Add(grid);
        }
    }

    private void GridColoring()
    {
        for (int i = 0; i < gridList.Count; ++i)
        {
            for (int beat = 2; beat <= maxBeat; ++beat)
            {
                if (i % beat == 0)
                {


                    break;
                }
            }
        }
    }

    //그리드 위치 설정
    private void SetGridPosition()
    {
        for (int i = 0; i < gridList.Count; ++i)
        {
            gridList[i].transform.localPosition = new Vector2(60 / Level.S.bpm * (1 / maxBeat) * interval * i, 0);
        }
    }

    //비트에 해당하는 그리드만 보이기
    private void ShowGrid(float beat)
    {
        for (int i = 0; i < gridList.Count; ++i)
        {
            gridList[i].transform.localPosition = new Vector2(60 / Level.S.bpm * (1 / maxBeat) * interval * i, 0);

            if (i % (32 / beat) != 0)
            {
                gridList[i].SetActive(false);
            }
            else
            {
                gridList[i].SetActive(true);
            }
        }

        gridList[0].SetActive(true);

        RenewalBeatUI();
    }
    #endregion

    //노트 생성 관련 함수
    #region
    private void TLNoteGeneration()
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
            NoteGeneration(row);
        }
    }

    private void NoteGeneration(int row)
    {
        TimeLineNote note = Instantiate(tlNotePrefab).GetComponent<TimeLineNote>();

        note.Setting(Level.S.level[row]);

        note.GetComponent<Button>().onClick.AddListener(NoteSelect);

        tlNoteList.Add(note);
    }

    //맵 파일에 맞게 노트 위치 설정
    private void SetNotePosition()
    {
        //노트의 부모를 timeLine으로 설정하고 노트의 timing에 맞게 위치 설정
        for (int row = 0; row < tlNoteList.Count; ++row)
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            editingMode = !editingMode;

            editingToggle.isOn = editingMode;
        }
    }
    #endregion

    //유저 입력, 에디터 환경 변경 관련 함수들
    #region
    //OnValueChanged에 들어갈 이벤트
    public void EditingToggle()
    {
        editingMode = editingToggle.isOn;
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
            interval += Input.GetAxis("Mouse ScrollWheel") * intervalSensivisity;

            if (interval < 50)
            {
                interval = 50;
            }

            SetGridPosition();
            SetNotePosition();
        }
    }

    private void SetBeat()
    {
        //SetBeatIndex를 통해 Index를 변경한 후, 변경된 인덱스를 selectedBeat에 전달
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedBeat = SetBeatIndex(selectedBeat + 1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedBeat = SetBeatIndex(selectedBeat - 1);
        }
        else return;

        ShowGrid(beat[selectedBeat]);
    }

    private int SetBeatIndex(int index)
    {
        //배열 초과 방지
        if (index >= beat.Length)
        {
            index = beat.Length - 1;
        }
        else if (index < 0)
        {
            index = 0;
        }

        return index;
    }

    private void RenewalBeatUI()
    {
        beatSlider.value = selectedBeat;
        beatText.text = "1 / " + beat[selectedBeat];
    }
    #endregion

    //노트 선택 관련 함수들
    #region 
    public void NoteSelect()
    {
        if (Input.GetKey(KeyCode.LeftControl) == true)
        {
            SingleNoteToggle();
        }
        else if (Input.GetKey(KeyCode.LeftShift) == true)
        {
            MultiNoteSelect();
        }
        else
        {
            SingleNoteSelect();
        }
    }

    private void MultiNoteSelect()
    {
        TimeLineNote selectedNote = EventSystem.current.currentSelectedGameObject.GetComponent<TimeLineNote>();

        if (selectedNote.num > standardNote.num)
        {
            for (int i = standardNote.num; i <= selectedNote.num; ++i)
            {
                AddSelectedNoteList(tlNoteList[i]);
            }
        }
        else
        {
            for (int i = standardNote.num; i >= selectedNote.num; --i)
            {
                AddSelectedNoteList(tlNoteList[i]);
            }
        }
    }

    private void SingleNoteToggle()
    {
        //selectedNoteList에 선택한 노트가 존재한다면 해당 노트 선택 해제, 아니라면 해당 노트 selectedNoteList에 추가
        ToggleSelectedNote();
    }

    private void SingleNoteSelect()
    {
        DeselectAll();

        AddSelectedNote();

        SetStandardNote(0);
    }

    private void SetStandardNote(int index)
    {
        //기존 기준 노트 강조 해제
        if (standardNote != null)
            standardNote.image.color = TimeLineNote.subSelectColor;

        standardNote = selectedNoteList[index];
        standardNote.image.color = TimeLineNote.stdSelectColor;
    }

    //해당 노트가 selectedNoteList에 이미 존재하는 지 확인하고, 중복되지 않는다면 리스트에 추가함.
    private void AddSelectedNoteList(TimeLineNote tlNote)
    {
        if (selectedNoteList.IndexOf(tlNote) == -1)
        {
            AddSelectedNote(tlNote);
        }
    }

    //모든 노트 선택 해제
    private void DeselectAll()
    {
        for (int i = 0; i < selectedNoteList.Count; ++i)
        {
            selectedNoteList[i].Deselect();
        }

        selectedNoteList.Clear();
    }

    //해당 노트 선택
    private void AddSelectedNote()
    {
        TimeLineNote selectedNote = EventSystem.current.currentSelectedGameObject.GetComponent<TimeLineNote>();

        selectedNote.Select();
        selectedNoteList.Add(selectedNote);
    }

    private void AddSelectedNote(TimeLineNote selectedNote)
    {
        selectedNote.Select();
        selectedNoteList.Add(selectedNote);
    }

    //해당 노트 선택 해제
    private void RemoveSelectedNote()
    {
        TimeLineNote selectedNote = EventSystem.current.currentSelectedGameObject.GetComponent<TimeLineNote>();

        selectedNote.Deselect();
        selectedNoteList.Remove(selectedNote);
    }

    private void RemoveSelectedNote(TimeLineNote selectedNote)
    {
        selectedNote.Deselect();
        selectedNoteList.Remove(selectedNote);
    }

    //해당 노트의 선택 여부 반전
    private void ToggleSelectedNote()
    {
        TimeLineNote selectedNote = EventSystem.current.currentSelectedGameObject.GetComponent<TimeLineNote>();

        if (selectedNote.isSelected == false)
        {
            AddSelectedNote();
            SetStandardNote(selectedNoteList.IndexOf(selectedNote));
        }
        else
        {
            RemoveSelectedNote();
        }
    }

    private void ToggleSelectedNote(TimeLineNote selectedNote)
    {
        if (selectedNote.isSelected == false)
        {
            AddSelectedNote();
        }
        else
        {
            RemoveSelectedNote();
        }
    }
    #endregion 노트 선택 관련 함수들

    //노트 정보 수정 관련 함수들
    #region
    private void InfoInit()
    {
        for(int i = 1; i < KEY.COUNT; ++i)
        {
            InfoGeneration(i, KEY.KEY_TYPE[i]);
        }

        SetInfoOptions(KEY.TYPE, TYPE.COUNT, TYPE.FindName);
        SetInfoOptions(KEY.NOTE_TYPE, NOTE_TYPE.COUNT, NOTE_TYPE.FindName);
        SetInfoOptions(KEY.EVENT_TYPE, EVENT_TYPE.COUNT, EVENT_TYPE.FindName);
    }

    //정보 UI 생성
    private void InfoGeneration(int num, int type)
    {
        if(type == 0)
        {
            GameObject info = Instantiate(infoInputFieldPref);
            info.transform.GetChild(0).GetComponent<Text>().text = Regex.Replace(KEY.FindName(num), "_", " ");
            infoArrInputField[num] = info.transform.GetChild(1).GetComponent<InputField>();

            SetInfoPosition(info.transform, num);
        }
        else if(type == 1)
        {
            GameObject info = Instantiate(infoDropdownPref);
            info.transform.GetChild(0).GetComponent<Text>().text = Regex.Replace(KEY.FindName(num), "_", " ");
            infoArrDropdown[num] = info.transform.GetChild(1).GetComponent<Dropdown>();

            SetInfoPosition(info.transform, num);
        }
    }

    private void SetInfoPosition(Transform trans, int num)
    {
        trans.transform.SetParent(infoPanel.transform);
        trans.localPosition = new Vector2(0, 170 - ((num - 1) * 37.5f));
        trans.localScale = new Vector3(0.9f, 0.9f, 0.9f);
    }

    delegate string FindName(int value);
    private void SetInfoOptions(int index, int count, FindName findName)
    {
        infoArrDropdown[index].ClearOptions();
        for (int i = 0; i < count; ++i)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = findName(i);
            infoArrDropdown[index].options.Add(option);
        }
    }

    #endregion
}
