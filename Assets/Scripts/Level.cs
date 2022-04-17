using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public static class PLAY_INFO
{
    public const int STOPPED = 0;
    public const int PLAYING = 1;
    public const int WAIT = 2;
}

public class Level : MonoBehaviour
{
    public static Level S;
    private void Awake()
    {
        Level.S = this;

        judgRange = new float[3];
    }


    public List<Dictionary<int, int>> level = new List<Dictionary<int, int>>(); //Timing�� ������� �ϴ� ���� ������ ��Ƶ�
    public List<Dictionary<int, int>> levelInfo = new List<Dictionary<int, int>>(); //������ �⺻ �������� ��Ƶ� (������, ������, BPM ��)

    public List<Note> noteList = new List<Note>();

    //���õ� ������ �̸��� ���̵�
    public string levelName;
    public int levelDifficulty;

    //level�� �������� �����Ǵ� ��ġ
    public int noteCount;

    //levelInfo�� �������� �����Ǵ� ��ġ
    public float offset;
    public float startDelay;
    public float bpm;

    public float songLength;

    public float noteSpeed;

    public float[] judgRange;

    public int isPlaying = PLAY_INFO.STOPPED;

    //������ �̸��� ���̵��� �޾� ������ �о��
    public string ReadLevel(string levelName, int dif)
    {
        //�������� ���� ����
        this.levelName = Regex.Replace(levelName, @"\s", "");
        this.levelDifficulty = dif;

        //������_���̵� �� ���� Ž��
        List<Dictionary<string, object>> tempLevel = CSVReader.Read(this.levelName + "_" + DIF.FindName(dif));

        //���� ���� ��ȯ
        ConvertLevel(tempLevel);

        //���� ���Ͽ��� �ʿ��� �� ����
        //��Ʈ ���� ����
        noteCount = 0;
        for(int i = 0; i < level.Count; ++i)
        {
            switch(level[i][KEY.NOTE_TYPE])
            {
                case NOTE_TYPE.TAP:
                case NOTE_TYPE.SLIDE:
                    noteCount += 1;
                    break;

                case NOTE_TYPE.DOUBLE:
                    noteCount += 2;
                    break;
            }
        }

        ReadLevelInfo();

        this.offset = levelInfo[0][INFO_KEY.OFFSET] * 0.001f;
        this.startDelay = levelInfo[0][INFO_KEY.START_DELAY] * 0.001f;
        this.bpm = levelInfo[0][INFO_KEY.BPM];
        this.judgRange[JUDG.PERFECT] = levelInfo[0][INFO_KEY.JUDG_RANGE] * 0.001f; //���� ������ perfect�� �������� �Ͽ� ���� ������ �ٸ� ���� ������ ������.
        this.judgRange[JUDG.GOOD] = judgRange[JUDG.PERFECT] * 2;
        this.judgRange[JUDG.MISS] = judgRange[JUDG.PERFECT] * 3f;

        return this.levelName;
    }

    //List<Dictionary<string, object>>�� ������ List<Dictionary<int, int>> �� ����
    private void ConvertLevel(List<Dictionary<string, object>> tempLevel)
    {
        //�� ���� ��Ƶ� ���� temp ����
        Dictionary<int, int> temp;

        for (int i = 0; i < tempLevel.Count; ++i)
        {
            //���ο� ���� ������ �� ���� temp�� ���ο� �޸� �Ҵ� (���� ���� ����)
            temp = new Dictionary<int, int>();

            for (int j = 0; j < tempLevel[0].Count; ++j)
            {
                //�ش� ���� ����ٸ� -1�� ��ȯ�Ͽ� ���� ����. (���� ����)
                string value = Convert.ToString(tempLevel[i][KEY.FindName(j)]);
                if (value == "")
                    value = "-1";

                temp[j] = Convert.ToInt32(value);
            }
            this.level.Add(temp);
        }
    }

    private void ReadLevelInfo()
    {
        //���� ���� ���� ��������
        List<Dictionary<string, object>> tempLevelInfo = CSVReader.Read(this.levelName + "_" + DIF.FindName(DIF.I));

        //���� ���� ���� �ڷ��� ��ȯ List<Dictionary<string, object>> -> List<Dictionary<int,int>>
        ConvertLevelInfo(tempLevelInfo);
    }

    private void ConvertLevelInfo(List<Dictionary<string, object>> tempLevelInfo)
    {
        //�� ���� ��Ƶ� ���� temp ����
        Dictionary<int, int> temp;

        for (int i = 0; i < tempLevelInfo.Count; ++i)
        {
            //���ο� ���� ������ �� ���� temp�� ���ο� �޸� �Ҵ� (���� ���� ����)
            temp = new Dictionary<int, int>();

            for (int j = 0; j < tempLevelInfo[0].Count; ++j)
            {
                //�ش� ���� ����ٸ� -1�� ��ȯ�Ͽ� ���� ����. (���� ����)
                string value = Convert.ToString(tempLevelInfo[i][INFO_KEY.FindName(j)]);
                if (value == "")
                    value = "-1";

                temp[j] = Convert.ToInt32(value);
            }
            this.levelInfo.Add(temp);
        }
    }

    public void WriteLevel()
    {
        Debug.Log("Write");
        using(var writer = new CsvFileWriter("Assets/Levels/" + levelName + "/Resources/" + levelName + "_" + DIF.FindName(levelDifficulty)))
        {
            List<string> colums = new List<string>();
            string[] keyList = new string[KEY.COUNT];
            for(int i = 0; i < KEY.COUNT; ++i)
            {
                keyList[i] = i.ToString();
            }

            colums.AddRange(keyList);

            writer.WriteRow(colums);
            colums.Clear();

            for (int i = 0; i < level.Count; ++i)
            {
                for(int j = 0; j < KEY.COUNT; ++j)
                {
                    colums.Add(level[i][j].ToString());
                }

                writer.WriteRow(colums);
                colums.Clear();
            }
        }
    }
}
