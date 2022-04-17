using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class EditorManager : MonoBehaviour
{
    public static EditorManager S;

    //에디팅 모드 활성화 관련 변수
    public bool editingMode = false;

    //노트와 그리드 생성을 위한 프리팹
    [SerializeField] private GameObject tlNotePrefab;

    //타임라인 오브젝트
    public GameObject timeLine;

    //생성된 노트와 그리드를 담아두는 리스트
    public List<TimeLineNote> tlNoteList = new List<TimeLineNote>();
    public List<GameObject> gridList = new List<GameObject>();

    //그리드와 리스트의 간격 배율
    public float interval;
    public float intervalSensivisity = 100;

    //노트 선택, 배치, 삭제 등에 관련된 변수
    [SerializeField] private List<TimeLineNote> selectedNoteList = new List<TimeLineNote>();
    [SerializeField] private TimeLineNote standardNote = null;

    //노트 정보 수정에 관련된 변수
    [SerializeField] private GameObject infoPanel;

    [SerializeField] private GameObject infoDropdownPref;
    [SerializeField] private GameObject infoInputFieldPref;
    private Dropdown[] infoArrDropdown = new Dropdown[KEY.COUNT]; //이 두 배열은 0번째 KEY인 TIMING을 사용하지 않음에 따라 1번째 배열부터 접근할 것.
    private InputField[] infoArrInputField = new InputField[KEY.COUNT];

    void Awake()
    {
        S = this;
    }

    void Start()
    {
        InitVariable();
    }

    void Update()
    {

    }



    //초기화 관련 함수
    #region 
    private void InitVariable()
    {
        for(int i = 0; i < KEY.COUNT; ++i)
        {
            infoArrDropdown[i] = null;
            infoArrInputField[i] = null;
        }
    }

    public void Init()
    {
        TLNoteGeneration();

        InfoInit();
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

        //ShowNoteInfo();
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

            //infoArrInputField[num].onEndEdit.AddListener(delegate { ChangeValue(); });

            SetInfoPosition(info.transform, num);
        }
        else if(type == 1)
        {
            GameObject info = Instantiate(infoDropdownPref);
            info.transform.GetChild(0).GetComponent<Text>().text = Regex.Replace(KEY.FindName(num), "_", " ");
            infoArrDropdown[num] = info.transform.GetChild(1).GetComponent<Dropdown>();

            //infoArrDropdown[num].onValueChanged.AddListener(delegate { ChangeValue(); });

            SetInfoPosition(info.transform, num);
        }
    }

    private int GetInfoValue(int index)
    {
        if (infoArrInputField[index] != null)
        {
            return Convert.ToInt32(infoArrInputField[index].text);
        }
        else
        {
            if (infoArrDropdown[index].options[infoArrDropdown[index].value].text == "null")
            {
                return 0;
            }
            else
                return infoArrDropdown[index].value + 1;
        }
    }

    private bool CheckInfoType(int index)
    {
        if (infoArrInputField[index] != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetInfoPosition(Transform trans, int num)
    {
        trans.transform.SetParent(infoPanel.transform);
        trans.localPosition = new Vector2(0, 170 - ((num - 1) * 37.5f));
        trans.localScale = new Vector3(0.9f, 0.9f, 0.9f);
    }

    delegate string FindName(int value); //InfoDorpdown의 Options를 설정함
    private void SetInfoOptions(int index, int count, FindName findName)
    {
        infoArrDropdown[index].ClearOptions();

        //0번째 옵션에 null을 추가함으로 인해 다른 옵션을 가져올 땐 +1을 하여 가져올 것.
        Dropdown.OptionData a = new Dropdown.OptionData();
        a.text = "null";
        infoArrDropdown[index].options.Add(a);

        for (int i = 0; i < count; ++i)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = findName(i);
            infoArrDropdown[index].options.Add(option);
        }
    }

    private void ShowNoteInfo()
    {
        for(int i = 1; i < KEY.COUNT; ++i)
        {
            //if (CheckInfoType(i) == true)
            //{
            //    infoArrInputField[i].text = GetInfoValue(i).ToString();
            //}
            //else
            //{
            //    infoArrDropdown[i].value = GetInfoValue(i);
            //}

            if(CheckInfoType(i) == false)
                infoArrDropdown[i].value = GetInfoValue(i);
        }
    }


    //UI에서 정보가 수정되면 호출되는 함수
    private void ChangeValue()
    {
        for(int i = 0; i < selectedNoteList.Count; ++i)
        {
            for(int j = 0; j < KEY.COUNT; ++j)
            {
                Level.S.level[selectedNoteList[i].num][j] = GetInfoValue(j);
            }
        }

        Level.S.WriteLevel();
    }

    #endregion
}
