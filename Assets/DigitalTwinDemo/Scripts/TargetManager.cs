using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetManager : MonoBehaviour
{
    public Transform UIContainer;

    public OrbitCamera cam;

    public Transform targetLocationParent;
    public GameObject buttonsPrefab;

    public List<Transform> TargetLocations { get; private set; }

    public void Start()
    {
        this.TargetLocations = new List<Transform>();
        foreach (Transform c in this.targetLocationParent)
        {
            if (c != this.targetLocationParent) this.TargetLocations.Add(c);
        }
        this.InitUI();
    }

    void InitUI()
    {
        foreach (var t in this.TargetLocations)
        {
            var b = GameObject.Instantiate(buttonsPrefab, UIContainer);
            b.GetComponentInChildren<TextMeshProUGUI>().text = t.gameObject.name;
            b.GetComponent<Button>().onClick.AddListener(() => { this.NavigateTo(t); });
            b.SetActive(true);  
        }
    }

    private void NavigateTo(Transform t)
    {
        var st = t.GetComponent<SwitchTarget>();
        if (st != null) st.Switch();
        else this.cam.target = t;
    }
}
