using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CounterManager : MonoBehaviour
{
    public Image time100;
    public Image time10;
    public Image time1;
    public Image mine100;
    public Image mine10;
    public Image mine1;

    public float time;
    public int mine;

    private float beginTime;

    public static CounterManager instance;
    void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one instance of CounterManager found!");
            return;
        }
        instance = this;
    }

    public void Reset()
    {
        time = 0f;
        beginTime = Time.time;
        mine = GridGenerator.instance.mines;
        SetTimerUI();
        SetMineUI();
    }

    private void Update()
    {
        if (GridGenerator.instance.state == GameState.RUNNING)
        {
            time = Time.time - beginTime;
        }
        SetTimerUI();
    }

    private void SetTimerUI()
    {
        int timer = Mathf.FloorToInt(time);

        if (timer > 999)
        {
            time100.sprite = Resources.Load<Sprite>("Images/d9");
            time10.sprite = Resources.Load<Sprite>("Images/d9");
            time1.sprite = Resources.Load<Sprite>("Images/d9");
            return;
        }

        int hundreds = Mathf.FloorToInt(timer / 100f);
        int tens = Mathf.FloorToInt(timer / 10f) % 10;
        int units = timer % 10;
        time100.sprite = Resources.Load<Sprite>($"Images/d{hundreds}");
        time10.sprite = Resources.Load<Sprite>($"Images/d{tens}");
        time1.sprite = Resources.Load<Sprite>($"Images/d{units}");
    }

    public void SetMineUI()
    {
        if (mine > 999)
        {
            mine100.sprite = Resources.Load<Sprite>("Images/d9");
            mine10.sprite = Resources.Load<Sprite>("Images/d9");
            mine1.sprite = Resources.Load<Sprite>("Images/d9");
            return;
        }

        if (mine < 0)
        {
            mine100.sprite = Resources.Load<Sprite>("Images/d-");
            if (mine < -99)
            {
                mine10.sprite = Resources.Load<Sprite>("Images/d9");
                mine1.sprite = Resources.Load<Sprite>("Images/d9");
                return;
            }

            int abs_mine = Mathf.Abs(mine);

            int tens_neg = Mathf.FloorToInt(abs_mine / 10f) % 10;
            int units_neg = abs_mine % 10;
            mine10.sprite = Resources.Load<Sprite>($"Images/d{tens_neg}");
            mine1.sprite = Resources.Load<Sprite>($"Images/d{units_neg}");
            return;
        }

        int hundreds = Mathf.FloorToInt(mine / 100f);
        int tens = Mathf.FloorToInt(mine / 10f) % 10;
        int units = mine % 10;
        mine100.sprite = Resources.Load<Sprite>($"Images/d{hundreds}");
        mine10.sprite = Resources.Load<Sprite>($"Images/d{tens}");
        mine1.sprite = Resources.Load<Sprite>($"Images/d{units}");
    }
}
