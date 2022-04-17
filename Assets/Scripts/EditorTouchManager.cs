using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorTouchManager : MonoBehaviour
{
    //
    private EditorManager editorMgr;
    private GridManager gridMgr;

    //
    [SerializeField] private Toggle editingToggle;

    //스크롤 구현에 필요한 변수
    private float lastMousePos;
    private bool isScrolling = false;
    private Vector2 tlPos;

    // Start is called before the first frame update
    void Start()
    {        
        editorMgr = EditorManager.S;

        editorMgr.InitEvent.AddListener(Init);
    }

    private void Init()
    {
        InitVariable();
    }

    private void InitVariable()
    {
        gridMgr = FindObjectOfType<GridManager>();

        tlPos = editorMgr.timeLine.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        SetEditingMode();

        if(editorMgr.editingMode == true)
        {
            Scroll();

            SetInterval();
        }
    }

    #region 에디트 모드 On/Off 관련 함수
    private void SetEditingMode()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            editorMgr.editingMode = !editorMgr.editingMode;

            editingToggle.isOn = editorMgr.editingMode;
        }
    }

    public void EditingToggle()
    {
        editorMgr.editingMode = editingToggle.isOn;
    }
    #endregion



    private void Scroll()
    {
        //우클릭이 시작되면 스크롤을 활성화
        if (Input.GetMouseButtonDown(1) == true)
        {
            isScrolling = true;
            lastMousePos = Input.mousePosition.x;
            tlPos = editorMgr.timeLine.transform.position;
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

            editorMgr.timeLine.transform.position = tlPos;
        }
    }



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
