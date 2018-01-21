using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicToastHandler : MonoBehaviour {

    public static PublicToastHandler instance;

    protected Toast[] toastList;

    public Toast toastPrefab;

    public int maxToasts = 10;

    private int curIndex = 0;

    public float defaultToastLifetime = 3.2f;

    private void Start()
    {
        //Debug.Log("Inialize the TOAST!");
        toastList = new Toast[maxToasts];
        for (int i = 0; i < toastList.Length; i++)
        {
            toastList[i] = Instantiate(toastPrefab, transform);
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }else
        {
            GameObject.Destroy(this);
        }
    }

    private void GiveMessageThis(string newMessage, float lifetime)
    {
        if (curIndex < toastList.Length)
        {
            toastList[curIndex].Init(newMessage, lifetime);
            curIndex = (curIndex + 1) % toastList.Length; //this should work since curIndex should never be negative.
        }
    }

    public static void GiveMessage(string newMessage, float lifeTime = 3.2f)
    {
        if (instance != null)
            instance.GiveMessageThis(newMessage, lifeTime);
    }
}
