using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

public class GridManager : MonoBehaviour
{
    //������ �׸��� ������
    [SerializeField] private GameObject gridPrefab;

    //��Ʈ ������ ���� ����
    public int selectedBeat = 2;
    public float[] beat = { 1, 2, 4, 8, 16, 32 };
    public const float minBeat = 1f;
    public const float maxBeat = 32f;
    public const float defaultBeat = 4f;

    void Start()
    {
        GridInit();
    }

    public void GridInit()
    {
        AllGridGeneration();
        SetAllGridPosition();

        ShowGrid(defaultBeat);
    }

    private void AllGridGeneration()
    {
        //���� ���� ��Ʈ�� ���ǿ����� ���̸� �����ϰ�, �ش� ���̸� ���� ������ �׸��� �� ����
        float minBeatLength = (60 / Level.S.bpm) * (1 / maxBeat);
        int gridCount = (int)(Level.S.songLength / minBeatLength) + 1;

        for(int i = 0; i < gridCount; ++i)
        {
            EditorManager.S.gridList.Add(InstantiateGrid(i));
        }
    }

    private GameObject InstantiateGrid(int num)
    {
        GameObject grid = Instantiate(gridPrefab) as GameObject;

        grid.transform.SetParent(EditorManager.S.timeLine.transform);
        grid.transform.localScale = Vector3.one;

        return grid;
    }

    public void SetAllGridPosition()
    {
        for(int i = 0; i < EditorManager.S.gridList.Count; ++i)
        {
            SetGridPosition(EditorManager.S.gridList[i], i);
        }
    }

    private void SetGridPosition(GameObject grid, int num)
    {
        float gridPosition = ((60f / Level.S.bpm) * (1 / maxBeat) * EditorManager.S.interval) * num;

        grid.transform.localPosition = new Vector2(gridPosition, 0);
    }

    public void ShowGrid(float beat)
    {
            Debug.Log(32 / beat);
        for(int i = 0; i < EditorManager.S.gridList.Count; ++i)
        {
            GameObject grid = EditorManager.S.gridList[i];

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
        EditorManager.S.gridList[0].SetActive(true);
    }
}
