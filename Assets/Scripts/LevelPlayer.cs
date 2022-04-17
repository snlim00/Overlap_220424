using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlayer : MonoBehaviour
{
    [SerializeField] private GameObject[] notePref;

    private EditorManager editorMgr;

    public double t;

    private AudioSource audioSource;

    private bool didEditorMgrInit = false;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        //ReadLevel�� ���� �̸����� ������ �����Ͽ� string���� ��ȯ, ���� �ش� ���ڿ��� �� ���� Ž��
        audioSource.clip = Resources.Load<AudioClip>(Level.S.ReadLevel("MeteorStream", DIF.X));

        Level.S.songLength = audioSource.clip.length;

        editorMgr = EditorManager.S;

        NoteGeneration();

        GameStart();
    }

    void Start()
    {
        
    }

    private void GameStart()
    {
        StartCoroutine(SongPlay());

        StartCoroutine(NoteTimer());
    }

    // Update is called once per frame
    void Update()
    {
        //�ݵ�� ������ �� (EditorManager�� Init�� GridManger, TLNoteMnager���� Init�� Start�� �ִ� ����)
        if(didEditorMgrInit == false)
        {
            didEditorMgrInit = true;

            if (PlayerSetting.S.editerMode == true)
            {
                editorMgr.Init();
            }
            else
            {
                editorMgr.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator NoteTimer()
    {
        //startDelay ��� �� Ÿ�̸� ���� (offset�� �ش� Ÿ�̸ӿ��� �����ϸ� ���Ӱ� �̺�Ʈ�� Ÿ�ֿ̹��� ������ ��ħ)
        yield return new WaitForSeconds(Level.S.startDelay);

        audioSource.Play();

        t = 0;
        int row = 0;
        float lastNoteTiming = Level.S.level[Level.S.level.Count - 1][KEY.TIMING] * 0.001f;
        Dictionary<int, int> thisRow = Level.S.level[row];

        while (t < lastNoteTiming)
        {
            t += Time.deltaTime;

            if(t >= thisRow[KEY.TIMING] * 0.001) //Ÿ�̸Ӱ� ���� ���� TIMING�� �����ϸ� ����
            {
                if(thisRow[KEY.TYPE] == TYPE.EVENT) //���� ���� EVENT��� ����
                {
                    EventExecute(thisRow);
                }

                ++row;
                thisRow = Level.S.level[row];
            }

            yield return null;
        }
    }

    IEnumerator SongPlay()
    {
        yield return new WaitForSeconds(PlayerSetting.S.offset + Level.S.offset);

        audioSource.Play();
    }

    void NoteGeneration()
    {
        for(int row = 0; row < Level.S.level.Count; ++row)
        {
            Dictionary<int, int> thisRow = Level.S.level[row];
            
            if (thisRow[KEY.TYPE] == TYPE.NOTE) //��Ʈ ����
            {
                Note note = null;

                switch (thisRow[KEY.NOTE_TYPE])
                {
                    case NOTE_TYPE.TAP:
                        {
                            note = Instantiate(notePref[NOTE_TYPE.TAP]).GetComponent<Note>();

                            int angle = thisRow[KEY.ANGLE];
                            float timing = (thisRow[KEY.TIMING] * 0.001f) + Level.S.startDelay;
                            float spawnDis = Level.S.noteSpeed * timing;

                            note.num = row;
                            note.Execute(angle, thisRow[KEY.TIMING] * 0.001f, spawnDis);
                        }
                        break;
                }

                Level.S.noteList.Add(note);
            }
        }
    }

    void EventExecute(Dictionary<int, int> thisRow)
    {
        switch (thisRow[KEY.EVENT_TYPE]) //�� �̺�Ʈ ȣ��
        {
            case EVENT_TYPE.SET_SPEED:
                {
                    float speed = thisRow[KEY.VALUE[0]];
                    SetSpeed(speed);
                }
                break;
        }
    }

    void SetSpeed(float speed)
    {
        Level.S.noteSpeed = speed;
    }
}
