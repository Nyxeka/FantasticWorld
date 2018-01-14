using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PublicLoadingBar : MonoBehaviour {

    public static PublicLoadingBar instance = null;

    public Image fillImage;

    private int numStuffToLoad;
    private int progress;
    private Queue<int> newStuffToAdd;
    private int newProgress = 0;

    private float maxTime = 0;
    private float timeAlive = 0;
    
    [SerializeField]
    private Text updateText;

    public static void AddNewStuff(int numNewStuff, float maxTime)
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(true);
            instance.newStuffToAdd.Enqueue(numNewStuff);
            instance.maxTime += maxTime;
            //Debug.Log("Added new set to loading bar!: " + numNewStuff);
        }
    }

    public static void UpdateProgress(int newProgress)
    {
        if (instance != null)
            instance.newProgress += newProgress;
    }

    private void FinishLoading()
    {
        if (instance != null)
        {
            //Debug.Log("The loading bar is done!");
            instance.progress = 0;
            instance.maxTime = 0;
            instance.timeAlive = 0;
            instance.SetLoadingBarFill(0);
            instance.gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        if (PublicLoadingBar.instance == null)
        {
            instance = this;
            instance.newStuffToAdd = new Queue<int>();
            //Debug.Log("Loaded loading bar!");
            instance.gameObject.SetActive(false);
        } else
        {
            GameObject.Destroy(this);
        }
    }

    private void UpdateLoadingBarFill()
    {
        if (instance != null)
        {
            SetLoadingBarFill(Mathf.Clamp01(instance.progress / instance.numStuffToLoad));
            if (instance.updateText != null)
            {
                instance.updateText.text = "Loading... (" + instance.progress.ToString() + "/" +  instance.numStuffToLoad + "), Time Loading: " + timeAlive.ToString("F2");
                
            }
        }
    }

    private void SetLoadingBarFill(float newAmount)
    {
        if (instance != null)
        {
            if (instance.fillImage != null) {
                instance.fillImage.fillAmount = newAmount;
            }
        }
    }

    public void Update()
    {
        
        if (instance.newStuffToAdd.Count > 0)
        {
            instance.numStuffToLoad += instance.newStuffToAdd.Dequeue();
        }
        if (instance.newProgress > 0)
        {
            instance.progress += instance.newProgress;
            newProgress = 0;
        }
        if (instance.progress >= instance.numStuffToLoad)
        {
            instance.FinishLoading();
        }

        instance.timeAlive += Time.deltaTime;

        if (instance.timeAlive >= instance.maxTime)
        {
            instance.FinishLoading();
        }

        instance.UpdateLoadingBarFill();
    }
}
