using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class EditorManager : MonoBehaviour
{
    public static EditorManager S;

    public UnityEvent InitEvent; //EditorManager�� �ڽĵ��� Init�� ���� ���

    //������ ��� Ȱ��ȭ ���� ����
    public bool editingMode = false;

    //Ÿ�Ӷ��� ������Ʈ
    public GameObject timeLine;

    //������ ��Ʈ�� �׸��带 ��Ƶδ� ����Ʈ
    public List<TimeLineNote> tlNoteList = new List<TimeLineNote>();
    public List<GameObject> gridList = new List<GameObject>();

    //�׸���� ����Ʈ�� ���� ����
    public float interval;
    public float intervalSensivisity = 100;

    //��Ʈ ����, ��ġ, ���� � ���õ� ����
    public List<TimeLineNote> selectedNoteList = new List<TimeLineNote>();
    public TimeLineNote standardNote = null;

    void Awake()
    {
        S = this;
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void Init()
    {
        InitEvent.Invoke();
    }
}
