using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public int num;

    public int angle;
    public float timing;

    protected bool doMove = false;

    public void Execute(int angle, float timing, float spawnDis, float num)
    {
        this.angle = angle + 90;
        this.timing = timing;

        transform.position = Vector3.zero;
        transform.eulerAngles = new Vector3(0, 0, this.angle);
        transform.Translate(-spawnDis, 0, 0);

        doMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (doMove == false)
            return;

        transform.Translate(Level.S.noteSpeed * Time.deltaTime, 0, 0);
    }

    public void Clear(int judg)
    {
        switch(judg)
        {
            case JUDG.PERFECT:

                Debug.Log("perfect");
                Destroy(this.gameObject);
                break;

            case JUDG.GOOD:

                Debug.Log("good");
                Destroy(this.gameObject);
                break;

            case JUDG.MISS:

                Debug.Log("miss");
                Destroy(this.gameObject);
                break;
        }
    }
}
