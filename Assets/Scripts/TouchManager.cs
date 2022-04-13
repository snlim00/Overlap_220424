using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    [SerializeField] private LevelPlayer levelPlayer;
    [SerializeField] private ParticleManager particleMgr;

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

    [SerializeField] private List<Note> hitNoteArr;
    private List<Note> clearedNoteArr;
    private void Touch()
    {
        inputCount = Input.inputString.Length; //�Էµ� ��ġ �� Ȯ��

        //�ֺ� ��Ʈ ��������
        GetAroundNote();

        //������ ��Ʈ�� 0����� �Լ� ����
        if (hitNoteArr.Count <= 0)
            return;

        //inputCount�� �ִ�ġ�� hitNoteArr�� ���� �Ͽ� �迭 �ʰ� ���� 
        //if(inputCount > hitNoteArr.Count)
        //{
        //    inputCount = hitNoteArr.Count;
        //}

        //����Ʈ ������ ��Ʈ�� �ִ��� Ȯ��
        if (CheckJudg(JUDG.PERFECT) == true)
        {
            ClearNote(JUDG.PERFECT);
        }
        else if(CheckJudg(JUDG.GOOD) == true)
        {
            ClearNote(JUDG.GOOD);
        }
        else if(CheckJudg(JUDG.MISS) == true)
        {
            ClearNote(JUDG.MISS);
        }
    }

    private void GetAroundNote()
    {
        Collider2D[] hitObjectArr = Physics2D.OverlapCircleAll(Vector2.zero, Level.S.noteSpeed * (Level.S.judgRange[JUDG.MISS] * 0.001f * 1.1f));
        hitNoteArr.Clear();
        
        hitNoteArr.Sort((Note x, Note y) => x.timing.CompareTo(y.num));

        clearedNoteArr.Clear();

        //������ ������Ʈ �� ��Ʈ�� ����
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
        bool isClear = false;

        for (int i = 0; i < hitNoteArr.Count; ++i)
        {
            if (Mathf.Abs(hitNoteArr[i].timing - (float)levelPlayer.t) <= Level.S.judgRange[judg] * 0.001f)
            {
                clearedNoteArr.Add(hitNoteArr[i]);
                //Debug.Log(hitNoteArr[i].timing - levelPlayer.t);
                isClear = true;
            }
        }

        return isClear;
    }

    private void ClearNote(int judg)
    {
        for (int i = 0; i < inputCount && i < clearedNoteArr.Count; ++i)
        {
            clearedNoteArr[i].Clear(judg);
        }
        particleMgr.ParticleGeneration(judg);
    }
}
