using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorTouchManager : MonoBehaviour
{
    //
    private GridManager gridMgr;

    //���� ������ ��Ʈ �������� ǥ���ϴ� UI
    [SerializeField] private Slider beatSlider;
    [SerializeField] private Text beatText;

    //
    [SerializeField] private Toggle editingToggle;

    //��ũ�� ������ �ʿ��� ����
    private float lastMousePos;
    private bool isScrolling = false;
    private Vector2 tlPos;

    // Start is called before the first frame update
    void Start()
    {
        InitVariable();
    }

    private void InitVariable()
    {
        gridMgr = FindObjectOfType<GridManager>();

        beatSlider.minValue = 0;
        beatSlider.maxValue = gridMgr.beat.Length - 1;

        tlPos = EditorManager.S.timeLine.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        SetEditingMode();

        if(EditorManager.S.editingMode == true)
        {
            SetBeat();

            Scroll();

            SetInterval();
        }
    }

    #region ����Ʈ ��� On/Off ���� �Լ�
    private void SetEditingMode()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            EditorManager.S.editingMode = !EditorManager.S.editingMode;

            editingToggle.isOn = EditorManager.S.editingMode;
        }
    }

    public void EditingToggle()
    {
        EditorManager.S.editingMode = editingToggle.isOn;
    }
    #endregion



    private void Scroll()
    {
        //��Ŭ���� ���۵Ǹ� ��ũ���� Ȱ��ȭ
        if (Input.GetMouseButtonDown(1) == true)
        {
            isScrolling = true;
            lastMousePos = Input.mousePosition.x;
            tlPos = EditorManager.S.timeLine.transform.position;
        }
        else if (Input.GetMouseButtonUp(1) == true)
        {
            isScrolling = false;
        }

        //���콺�� ������ ��ġ�� ���� ��ġ�� ���Ͽ� ���� �� ��ŭ Ÿ�Ӷ��� ��ġ �̵�
        if(isScrolling == true)
        {
            tlPos.x -= lastMousePos - Input.mousePosition.x;
            lastMousePos = Input.mousePosition.x;

            EditorManager.S.timeLine.transform.position = tlPos;
        }
    }



    #region ��Ʈ ������ ���� ���� �Լ�
    private void SetBeat()
    {
        if (_SetBeat() == false)
            return;

        gridMgr.ShowGrid(gridMgr.beat[gridMgr.selectedBeat]);
        RenewalBeatUI();
    }

    private bool _SetBeat()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            gridMgr.selectedBeat = SetBeatIndex(gridMgr.selectedBeat + 1);

            return true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            gridMgr.selectedBeat = SetBeatIndex(gridMgr.selectedBeat - 1);

            return true;
        }
        else return false;
    }

    /// <summary>
    ///EditorManager.S.beat[index] �� ��ȯ��.
    /// </summary>
    private int SetBeatIndex(int index)
    {
        if (index >= gridMgr.beat.Length)
        {
            index = gridMgr.beat.Length - 1;
        }
        else if (index < 0)
        {
            index = 0;
        }

        return index;
    }

    private void RenewalBeatUI()
    {
        beatSlider.value = gridMgr.selectedBeat;
        beatText.text = "1 / " + gridMgr.beat[gridMgr.selectedBeat];
    }
    #endregion



    #region �׸��� �� ��Ʈ ���� ���� ���� �Լ�
    private void SetInterval()
    {
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            EditorManager.S.interval += Input.GetAxis("Mouse ScrollWheel") * EditorManager.S.intervalSensivisity;

            if (EditorManager.S.interval < 50)
            {
                EditorManager.S.interval = 50;
            }

            gridMgr.SetAllGridPosition();
        }
    }
    #endregion
}
