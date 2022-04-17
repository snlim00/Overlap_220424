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

    //������ ��� Ȱ��ȭ ���� ����
    public bool editingMode = false;

    //��Ʈ�� �׸��� ������ ���� ������
    [SerializeField] private GameObject tlNotePrefab;

    //Ÿ�Ӷ��� ������Ʈ
    public GameObject timeLine;

    //������ ��Ʈ�� �׸��带 ��Ƶδ� ����Ʈ
    public List<TimeLineNote> tlNoteList = new List<TimeLineNote>();
    public List<GameObject> gridList = new List<GameObject>();

    //�׸���� ����Ʈ�� ���� ����
    public float interval;
    public float intervalSensivisity = 100;

    //��Ʈ ����, ��ġ, ���� � ���õ� ����
    [SerializeField] private List<TimeLineNote> selectedNoteList = new List<TimeLineNote>();
    [SerializeField] private TimeLineNote standardNote = null;

    //��Ʈ ���� ������ ���õ� ����
    [SerializeField] private GameObject infoPanel;

    [SerializeField] private GameObject infoDropdownPref;
    [SerializeField] private GameObject infoInputFieldPref;
    private Dropdown[] infoArrDropdown = new Dropdown[KEY.COUNT]; //�� �� �迭�� 0��° KEY�� TIMING�� ������� ������ ���� 1��° �迭���� ������ ��.
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



    //�ʱ�ȭ ���� �Լ�
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



    //��Ʈ ���� ���� �Լ�
    #region
    private void TLNoteGeneration()
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

    //�� ���Ͽ� �°� ��Ʈ ��ġ ����
    private void SetNotePosition()
    {
        //��Ʈ�� �θ� timeLine���� �����ϰ� ��Ʈ�� timing�� �°� ��ġ ����
        for (int row = 0; row < tlNoteList.Count; ++row)
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
    #endregion



    //��Ʈ ���� ���� �Լ���
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
        //selectedNoteList�� ������ ��Ʈ�� �����Ѵٸ� �ش� ��Ʈ ���� ����, �ƴ϶�� �ش� ��Ʈ selectedNoteList�� �߰�
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
        //���� ���� ��Ʈ ���� ����
        if (standardNote != null)
            standardNote.image.color = TimeLineNote.subSelectColor;

        standardNote = selectedNoteList[index];
        standardNote.image.color = TimeLineNote.stdSelectColor;
    }

    //�ش� ��Ʈ�� selectedNoteList�� �̹� �����ϴ� �� Ȯ���ϰ�, �ߺ����� �ʴ´ٸ� ����Ʈ�� �߰���.
    private void AddSelectedNoteList(TimeLineNote tlNote)
    {
        if (selectedNoteList.IndexOf(tlNote) == -1)
        {
            AddSelectedNote(tlNote);
        }
    }

    //��� ��Ʈ ���� ����
    private void DeselectAll()
    {
        for (int i = 0; i < selectedNoteList.Count; ++i)
        {
            selectedNoteList[i].Deselect();
        }

        selectedNoteList.Clear();
    }

    //�ش� ��Ʈ ����
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

    //�ش� ��Ʈ ���� ����
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

    //�ش� ��Ʈ�� ���� ���� ����
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
    #endregion ��Ʈ ���� ���� �Լ���



    //��Ʈ ���� ���� ���� �Լ���
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

    //���� UI ����
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

    delegate string FindName(int value); //InfoDorpdown�� Options�� ������
    private void SetInfoOptions(int index, int count, FindName findName)
    {
        infoArrDropdown[index].ClearOptions();

        //0��° �ɼǿ� null�� �߰������� ���� �ٸ� �ɼ��� ������ �� +1�� �Ͽ� ������ ��.
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


    //UI���� ������ �����Ǹ� ȣ��Ǵ� �Լ�
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
