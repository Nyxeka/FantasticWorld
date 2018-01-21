using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour {

    private float lifeTime;
    private float fadeTime = 0.5f;
    float _fadeTime = 0;
    float _time = 0;
    [SerializeField] Image toastImage;
    [SerializeField] Text toastText;

    private Color fadeColor = new Color(0, 0, 0, 0);
    /// <summary>
    /// image colour.
    /// </summary>
    private Color iCol;
    /// <summary>
    /// Text colour.
    /// </summary>
    private Color tCol;

    private float lifeTimePercent = 0;

	public Toast Init(string text, float lifetime)
    {
        toastText.text = text;
        this.lifeTime = lifetime;
        toastText.color = tCol;
        toastImage.color = iCol;
        this.gameObject.SetActive(true);
        return this;
    }

    private void Start()
    {
        tCol = toastText.color;
        iCol = toastImage.color;
        gameObject.SetActive(false);
    }

    private void EndToast()
    {
        _time = 0;
        _fadeTime = 0;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        
        if (_time >= lifeTime)
        {
            _fadeTime += Time.deltaTime;
            toastText.color = Color.Lerp(tCol, fadeColor, _fadeTime / fadeTime);
            toastImage.color = Color.Lerp(iCol, fadeColor, _fadeTime / fadeTime);
            if (_fadeTime >= fadeTime)
                EndToast();
        }
        else
        {
            _time += Time.deltaTime;
        }
        
        


    }
}
