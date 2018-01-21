using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PublicLoadingBar : MonoBehaviour {

    public static PublicLoadingBar instance = null;

    public Image fillImage;

    private int numStuffToLoad = 0;
    private int progress = 0;
    private Queue<int> newStuffToAdd;
    private int newProgress = 0;

    private float maxTime = 0;
    private float timeAlive = 0;

    private string loadingText = "Loading";
    
    [SerializeField]
    private Text updateText;

    public static void AddNewStuff(int numNewStuff, float maxTime, string loadingText = "Loading")
    {
        if (instance != null)
        {
            instance.gameObject.SetActive(true);
            instance.newStuffToAdd.Enqueue(numNewStuff);
            instance.maxTime += maxTime;
            instance.loadingText = loadingText;
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
        progress = 0;
        maxTime = 0;
        timeAlive = 0;
        SetLoadingBarFill(0);
        newStuffToAdd.Clear();
        numStuffToLoad = 0;
        gameObject.SetActive(false);
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            newStuffToAdd = new Queue<int>();
            //Debug.Log("Loaded loading bar!");
            gameObject.SetActive(false);
        } else
        {
            GameObject.Destroy(this);
        }
    }
    float progressPercent;
    float timePercent;
    private void UpdateLoadingBarFill()
    {
        if (numStuffToLoad == 0)
            return;
        progressPercent = progress / numStuffToLoad;
        timePercent = timeAlive / maxTime;

        SetLoadingBarFill(progressPercent > timePercent ? progressPercent : timePercent);

        if (updateText != null)
        {
            updateText.text = loadingText + "... (" + progress.ToString() + "/" +  numStuffToLoad + "), Time " + loadingText + ": " + timeAlive.ToString("F2");
            
        }
    }

    private void SetLoadingBarFill(float newAmount)
    {
        if (fillImage != null) {
            fillImage.fillAmount = newAmount;
        }
}

    public void Update()
    {
        
        if (newStuffToAdd.Count > 0)
        {
            numStuffToLoad += newStuffToAdd.Dequeue();
        }
        if (newProgress > 0)
        {
            progress += newProgress;
            newProgress = 0;
        }
        if (progress >= numStuffToLoad)
        {
            FinishLoading();
        }

        timeAlive += Time.deltaTime;

        if (timeAlive >= maxTime)
        {
            FinishLoading();
        }

        UpdateLoadingBarFill();
    }
}
