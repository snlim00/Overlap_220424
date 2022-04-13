using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private GameObject CircleEffect;

    public Vector2 perfectTargetSize = new Vector2(0.35f, 0.35f); //����Ʈ �� ��Ÿ���� ����Ʈ�� ��ǥ ������
    public Vector2 goodTargetSize = new Vector2(0.3f, 0.3f); //����Ʈ �� ��Ÿ���� ����Ʈ�� ��ǥ ������
    public Vector2 missTargetSize = new Vector2(0.25f, 0.25f); //����Ʈ �� ��Ÿ���� ����Ʈ�� ��ǥ ������

    [SerializeField] private Color perfectEffectColor = new Color(0.5f, 0.5f, 1);
    [SerializeField] private Color goodEffectColor = new Color(0.5f, 1, 0.5f);
    [SerializeField] private Color missEffectColor = new Color(1, 0.2f, 0.2f);
    private Color slideEffectColor;

    public void Start()
    {
        slideEffectColor = perfectEffectColor;
    }

    public void ParticleGeneration(int judg)
    {
        GameObject go = Instantiate(CircleEffect) as GameObject;
        CircleEffectController goCtrl = go.GetComponent<CircleEffectController>();

        switch (judg)
        {
            case JUDG.PERFECT:
                goCtrl.targetSize = perfectTargetSize;
                go.GetComponent<SpriteRenderer>().color = perfectEffectColor;

                break;


            case JUDG.GOOD:
                goCtrl.targetSize = goodTargetSize;
                go.GetComponent<SpriteRenderer>().color = goodEffectColor;

                break;


            case JUDG.MISS:
                goCtrl.targetSize = missTargetSize;
                go.GetComponent<SpriteRenderer>().color = missEffectColor;

                break;
        }
    }
}
