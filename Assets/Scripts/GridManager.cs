using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class GridManager : MonoBehaviour
{
    private EditorManager editorMgr;

    //������ �׸��� ������
    [SerializeField] private GameObject gridPrefab;

    //�׸����� �θ� ������Ʈ
    private Transform gridParents;

    //���� ������ ��Ʈ �������� ǥ���ϴ� UI
    [SerializeField] private Slider beatSlider;
    [SerializeField] private Text beatText;

    //��Ʈ ������ ���� ����
    public int selectedBeat = 2;
    public float[] beat = { 1, 2, 4, 8, 16, 32 };
    public const float minBeat = 1f;
    public const float maxBeat = 32f;
    public const float defaultBeat = 4f;

    void Start()
    {        
        editorMgr = EditorManager.S;

        editorMgr.InitEvent.AddListener(Init);
    }

    void Init()
    {
        gridParents = editorMgr.timeLine.transform.FindChild("Grids");

        beatSlider.minValue = 0;
        beatSlider.maxValue = beat.Length - 1;

        GridInit();
    }

    void Update()
    {
        if(editorMgr.editingMode == true)
        {
            SetBeat();
        }
    }

    public void GridInit()
    {
        AllGridGeneration();
        SetAllGridPosition();

        ShowGrid(defaultBeat);
    }

    #region �׸��� ���� ���� �Լ�
    private void AllGridGeneration()
    {
        //���� ���� ��Ʈ�� ���ǿ����� ���̸� �����ϰ�, �ش� ���̸� ���� ������ �׸��� �� ����
        float minBeatLength = (60 / Level.S.bpm) * (1 / maxBeat);
        int gridCount = (int)(Level.S.songLength / minBeatLength) + 1;

        for(int i = 0; i < gridCount; ++i)
        {
            editorMgr.gridList.Add(InstantiateGrid(i));
        }
    }

    private GameObject InstantiateGrid(int num)
    {
        GameObject grid = Instantiate(gridPrefab) as GameObject;

        grid.transform.SetParent(gridParents);
        grid.transform.localScale = Vector3.one;

        return grid;
    }

    public void SetAllGridPosition()
    {
        for(int i = 0; i < editorMgr.gridList.Count; ++i)
        {
            SetGridPosition(editorMgr.gridList[i], i);
        }
    }
    #endregion



    private void SetGridPosition(GameObject grid, int num)
    {
        float gridPosition = ((60f / Level.S.bpm) * (1 / maxBeat) * editorMgr.interval) * num;

        grid.transform.localPosition = new Vector2(gridPosition, 0);
    }

    private void ShowGrid(float beat)
    {
        for(int i = 0; i < editorMgr.gridList.Count; ++i)
        {
            GameObject grid = editorMgr.gridList[i];

            //�ش� ��Ʈ�� ��Ʈ�� �ش����� �ʴ´ٸ� ��Ȱ��ȭ, �ش��Ѵٸ� Ȱ��ȭ
            if(i % (32 / beat) != 0)
            {
                grid.SetActive(false);
            }
            else
            {
                grid.SetActive(true);
            }
        }

        //ù��° �׸���� �׻� Ȱ��ȭ
        editorMgr.gridList[0].SetActive(true);
    }



    #region ��Ʈ ������ ���� ���� �Լ�
    private void SetBeat()
    {
        if (_SetBeat() == false)
            return;

        ShowGrid(beat[selectedBeat]);
        RenewalBeatUI();
    }

    private bool _SetBeat()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedBeat = SetBeatIndex(selectedBeat + 1);

            return true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedBeat = SetBeatIndex(selectedBeat - 1);

            return true;
        }
        else return false;
    }

    private int SetBeatIndex(int index)
    {
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
}
