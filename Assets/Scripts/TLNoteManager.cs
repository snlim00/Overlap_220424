using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TLNoteManager : MonoBehaviour
{
    private EditorManager editorMgr;

    [SerializeField] private GameObject tlNotePrefab;

    //��Ʈ�� �θ� ������Ʈ
    private Transform noteParents;

    void Start()
    {
        editorMgr = EditorManager.S;

        editorMgr.InitEvent.AddListener(Init);
    }

    private void Init()
    {
        noteParents = editorMgr.timeLine.transform.FindChild("Notes");

        TLNoteInit();
    }

    private void TLNoteInit()
    {
        AllTLNoteGeneration();
        SetAllTLNotePosition();
        SortNoteNum();
    }

    #region ��Ʈ ���� ���� �Լ�
    private void AllTLNoteGeneration()
    {
        for(int row = 0; row < Level.S.level.Count; ++row)
        {
            editorMgr.tlNoteList.Add(TLNoteGeneration(row));
        }
    }

    private TimeLineNote TLNoteGeneration(int row)
    {
        TimeLineNote tlNote = InstantiateTLNote();

        tlNote.Setting(Level.S.level[row]);

        tlNote.gameObject.name = row.ToString();

        return tlNote;
    }

    private TimeLineNote InstantiateTLNote()
    {
        TimeLineNote tlNote = Instantiate(tlNotePrefab).GetComponent<TimeLineNote>();

        tlNote.transform.SetParent(noteParents);

        tlNote.transform.localScale = Vector3.one;

        tlNote.GetComponent<Button>().onClick.AddListener(NoteSelect);

        return tlNote;
    }

    private void SetAllTLNotePosition()
    {
        for(int row = 0; row < editorMgr.tlNoteList.Count; ++row)
        {
            TimeLineNote tlNote = editorMgr.tlNoteList[row];

            SetNotePosition(tlNote);
        }
    }

    private void SetNotePosition(TimeLineNote tlNote)
    {
        tlNote.transform.localPosition = new Vector2(tlNote.info[KEY.TIMING] * 0.001f * editorMgr.interval, 0);
    }

    private void SortNoteNum()
    {
        //��Ʈ�� �ε����� num�� x���� �������� �Ͽ� �������� ����
        editorMgr.tlNoteList.Sort((A, B) => A.transform.position.x.CompareTo(B.transform.position.x));

        for (int i = 0; i < editorMgr.tlNoteList.Count; ++i)
        {
            editorMgr.tlNoteList[i].num = i;
        }
    }
    #endregion




    #region ��Ʈ ���� ���� �Լ�
    //��Ʈ�� OnClick�� �Ҵ�Ǵ� �̺�Ʈ
    private void NoteSelect()
    {
        if(Input.GetKey(KeyCode.LeftControl) == true)
        {
            SingleNoteToggle();
        }
        else if(Input.GetKey(KeyCode.LeftAlt) == true)
        {
            ChangeStandardNote();
        }
        else if(Input.GetKey(KeyCode.LeftShift) == true)
        {
            MultiNoteSelect();
        }
        else
        {
            SingleNoteSelect();
        }
    }

    //���� ��Ʈ ����
    private void SingleNoteSelect()
    {
        DeselectAll();


        TimeLineNote tlNote = GetCurrentSelectedNote();

        AddSelectedNote(tlNote);

        SetStandardNote(tlNote); //��Ʈ �ΰ��� ���õǴ°� ���Ⱑ ������. ������ ��.
    }

    //���� ��Ʈ ���
    private void SingleNoteToggle()
    {
        TimeLineNote tlNote = GetCurrentSelectedNote();

        NoteToggle(tlNote);
    }

    //���� ��Ʈ ����
    private void MultiNoteSelect()
    {
        TimeLineNote tlNote = GetCurrentSelectedNote();

        int stdNoteNum = editorMgr.standardNote.num;

        if (tlNote.num > stdNoteNum)
        {
            for(int i = stdNoteNum; i <= tlNote.num; ++i)
            {
                AddSelectedNote(editorMgr.tlNoteList[i]);
            }
        }
        else
        {
            for (int i = stdNoteNum; i >= tlNote.num; --i)
            {
                AddSelectedNote(editorMgr.tlNoteList[i]);
            }
        }
    }

    //��Ʈ ���
    private void NoteToggle(TimeLineNote tlNote)
    {
        //standardNote�� ���� ������ �� �����Ƿ� �Լ� ����.
        if (tlNote == editorMgr.standardNote)
            return;

        if (tlNote.isSelected == true)
            Deselect(tlNote);
        else
            AddSelectedNote(tlNote);
    }

    //��� ��Ʈ ���� ���� (���� ��Ʈ ���� �ÿ��� ���, �� �ܿ� ��� �� ���� ��Ʈ�� ������� �һ�� �߻�)
    private void DeselectAll()
    {
        int selectedNoteCount = editorMgr.selectedNoteList.Count;
        //for (int i = 0; i < selectedNoteCount; ++i)
        for (int i = 0; i < editorMgr.selectedNoteList.Count; ++i)
        {
            //Debug.Log("i: " + i);
            //Debug.LogWarning("name: " + editorMgr.selectedNoteList[i].gameObject.name);
            //Debug.LogError("count: " + editorMgr.selectedNoteList.Count);

            //editorMgr.selectedNoteList[i].Deselect();
            Deselect(editorMgr.selectedNoteList[i], true);
        }

        //editorMgr.selectedNoteList.Clear();
    }

    //��Ʈ ���� ����
    private void Deselect(TimeLineNote tlNote, bool calledByDeselectAll = false)
    {
        //if(calledByDeselectAll == true && tlNote == editorMgr.standardNote)
        //{
        //    return;
        //}

        editorMgr.selectedNoteList.Remove(tlNote);

        tlNote.Deselect();
    }

    //selectedNoteList�� ��Ʈ �߰�
    private void AddSelectedNote(TimeLineNote tlNote)
    {
        if (editorMgr.selectedNoteList.IndexOf(tlNote) != -1)
            return;

        tlNote.Select();
        editorMgr.selectedNoteList.Add(tlNote);
    }

    //���� ��Ʈ ����
    private void SetStandardNote(TimeLineNote tlNote)
    {
        //���� ���� ��Ʈ ���� ����
        if(editorMgr.standardNote != null)
        {
            editorMgr.standardNote.UnsetStandardNote();
        }

        //�� �ڵ�� �� �������� �� �𸣰���.
        //int index = editorMgr.tlNoteList.IndexOf(tlNote);

        //if (index == -1)
        //    return;

        editorMgr.standardNote = tlNote;
        editorMgr.standardNote.SetStandardNote();
    }

    //���� ��Ʈ ����
    private void ChangeStandardNote()
    {
        TimeLineNote tlNote = GetCurrentSelectedNote();

        AddSelectedNote(tlNote);
        SetStandardNote(tlNote);
    }

    //Event.current.currentSelectedGameObject�� �������� �Լ� (���� ������ ���� �ش� �Լ��� ����� ��)
    private TimeLineNote GetCurrentSelectedNote()
    {
        return EventSystem.current.currentSelectedGameObject.GetComponent<TimeLineNote>();
    }
    #endregion
}
