using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class TimeLineManager : MonoBehaviour
{
    //������ ��� Ȱ��ȭ ���� ����
    [SerializeField] private bool editingMode = false;
    [SerializeField] private Toggle editingToggle;

    //��Ʈ�� �׸��� ������ ���� ������
    [SerializeField] private GameObject tlNotePrefab;
    [SerializeField] private GameObject gridPrefab;

    //Ÿ�Ӷ��� ������Ʈ
    [SerializeField] private GameObject timeLine;

    //���� ������ ��Ʈ �������� ǥ���ϴ� UI
    [SerializeField] private Slider beatSlider;
    [SerializeField] private Text beatText;

    //������ ��Ʈ�� �׸��带 ��Ƶδ� ����Ʈ
    private List<TimeLineNote> tlNoteList = new List<TimeLineNote>();
    private List<GameObject> gridList = new List<GameObject>();

    //�׸���� ����Ʈ�� ���� ����
    public float interval;
    private float intervalSensivisity = 100;

    //��Ʈ ������ ���� ����
    private int selectedBeat = 2;
    private float[] beat = { 1, 2, 4, 8, 16, 32 };
    private const float minBeat = 1;
    private const float maxBeat = 32f;

    //��ũ�� ������ �ʿ��� ����
    private float lastMousePos;
    private bool isScrolling = false;
    private Vector2 tlPos;

    //��Ʈ ����, ��ġ, ���� � ���õ� ����
    [SerializeField] private List<TimeLineNote> selectedNoteList = new List<TimeLineNote>();
    [SerializeField] private TimeLineNote standardNote = null;

    //��Ʈ ���� ������ ���õ� ����
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

    //�ʱ�ȭ ���� �Լ�
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

    //�׸��� ���� ���� �Լ�
    #region
    private void DrawGrid()
    {
        GridGeneration();
        SetGridPosition();
    }

    //�׸��� ����
    private void GridGeneration()
    {
        //�׷��� �� �׸��� ���� ����
        float minBeat = (60 / Level.S.bpm) * (1 / maxBeat);
        int gridCount = (int)(Level.S.songLength / minBeat) + 1;

        //�׸��� ����
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

    //�׸��� ��ġ ����
    private void SetGridPosition()
    {
        for (int i = 0; i < gridList.Count; ++i)
        {
            gridList[i].transform.localPosition = new Vector2(60 / Level.S.bpm * (1 / maxBeat) * interval * i, 0);
        }
    }

    //��Ʈ�� �ش��ϴ� �׸��常 ���̱�
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

    private void SwitchEditingMode()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            editingMode = !editingMode;

            editingToggle.isOn = editingMode;
        }
    }
    #endregion

    //���� �Է�, ������ ȯ�� ���� ���� �Լ���
    #region
    //OnValueChanged�� �� �̺�Ʈ
    public void EditingToggle()
    {
        editingMode = editingToggle.isOn;
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
        //SetBeatIndex�� ���� Index�� ������ ��, ����� �ε����� selectedBeat�� ����
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
        //�迭 �ʰ� ����
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
