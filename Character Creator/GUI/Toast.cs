using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour {

    private float lifetime;
    float _time = 0;
    private string text;

	public Toast Init(string text, float lifetime)
    {
        this.text = text;
        this.lifetime = lifetime;
        return this;
    }

    private void Update()
    {
        _time += Time.deltaTime;

    }

}
