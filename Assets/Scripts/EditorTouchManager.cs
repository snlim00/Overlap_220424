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

    //��ũ�� ������ �ʿ��� ����
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

    #region ����Ʈ ��� On/Off ���� �Լ�
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
        //��Ŭ���� ���۵Ǹ� ��ũ���� Ȱ��ȭ
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

        //���콺�� ������ ��ġ�� ���� ��ġ�� ���Ͽ� ���� �� ��ŭ Ÿ�Ӷ��� ��ġ �̵�
        if(isScrolling == true)
        {
            tlPos.x -= lastMousePos - Input.mousePosition.x;
            lastMousePos = Input.mousePosition.x;

            editorMgr.timeLine.transform.position = tlPos;
        }
    }



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
