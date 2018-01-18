using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Hpbar : MonoBehaviour {

    Slider _slider;

    // Use this for initialization
    void Start () {
		_slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    //void Update () {

    //   }

    public void SetHp(float _value)
    {
        _slider.value = _value / 20;
    }
}
