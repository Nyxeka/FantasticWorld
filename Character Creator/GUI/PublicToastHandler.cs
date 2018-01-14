using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicToastHandler : MonoBehaviour {

    public static PublicToastHandler instance;

    protected List<Toast> toastList;

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
}
