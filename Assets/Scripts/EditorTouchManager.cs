using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorTouchManager : MonoBehaviour
{
    //
    private GridManager gridMgr;

    //현재 설정된 비트 나눗수를 표시하는 UI
    [SerializeField] private Slider beatSlider;
    [SerializeField] private Text beatText;

    //
    [SerializeField] private Toggle editingToggle;

    //스크롤 구현에 필요한 변수
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

    #region 에디트 모드 On/Off 관련 함수
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
        //우클릭이 시작되면 스크롤을 활성화
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

        //마우스의 마지막 위치와 현재 위치를 비교하여 변한 값 만큼 타임라인 위치 이동
        if(isScrolling == true)
        {
            tlPos.x -= lastMousePos - Input.mousePosition.x;
            lastMousePos = Input.mousePosition.x;

            EditorManager.S.timeLine.transform.position = tlPos;
        }
    }



    #region 비트 나눗수 설정 관련 함수
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
    ///EditorManager.S.beat[index] 를 반환함.
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



    #region 그리드 및 노트 간격 설정 관련 함수
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
