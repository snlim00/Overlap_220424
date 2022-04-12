using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private LevelPlayer levelPlayer;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        hitNoteArr = new List<Note>();
        clearedNoteArr = new List<Note>();
    }

    [SerializeField] private int inputCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.inputString.Length > 0)
        {
            Touch();
        }
    }

    private List<Note> hitNoteArr;
    private List<Note> clearedNoteArr;
    private void Touch()
    {
        inputCount = Input.inputString.Length; //입력된 터치 수 확인

        //주변 노트 가져오기
        GetAroundNote();

        //가져온 노트가 0개라면 함수 종료
        if (hitNoteArr.Count <= 0)
            return;

        //퍼펙트 판정인 노트가 있는지 확인
        CheckJudg(JUDG.PERFECT);
    }

    private void GetAroundNote()
    {
        Collider2D[] hitObjectArr = Physics2D.OverlapCircleAll(Vector2.zero, Level.S.noteSpeed * (Level.S.judgRange[JUDG.MISS] * 0.001f * 1.1f));
        hitNoteArr.Clear();
        clearedNoteArr.Clear();

        //가져온 오브젝트 중 노트만 저장
        for (int i = 0; i < hitObjectArr.Length; ++i)
        {
            if (hitObjectArr[i].tag == TAG.NOTE)
            {
                hitNoteArr.Add(hitObjectArr[i].GetComponent<Note>());
            }
        }
    }

    private bool CheckJudg(int judg)
    {
        for (int i = 0; i < hitNoteArr.Count; ++i)
        {
            if ((hitNoteArr[i].timing - levelPlayer.t) <= Level.S.judgRange[judg])
            {
                clearedNoteArr.Add(hitNoteArr[i]);
            }
        }

        if (clearedNoteArr.Count > 0)
            return true;

        else
            return false;
    }
}
