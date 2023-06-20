using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backgraund_Scaling : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.width);//Настройка фона по размеру экрана
    }
}
