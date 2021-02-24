using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataReader : MonoBehaviour
{

    public string FileLocation = @"C:\Sviluppo\Youtube\UnityInteractiveMap\Data\Data.csv";
    public float reloadInterval = 2;
    private float currentTiming = 0;

    public int currentType = 0;

    public Material defaultMaterial;
    public GameObject ModelRoot;
    public List<Material> ToSkipMaterials = new List<Material>();
    private List<Material> oldMaterials=new List<Material>();

    private void Start()
    {
        var renderer = ModelRoot.GetComponentsInChildren<MeshRenderer>();

        oldMaterials.Clear();
        foreach (var r in renderer)
        {
            oldMaterials.Add(r.sharedMaterial);
        }
    }

    public void SetType(int selection)
    {
        this.currentType = selection;
        this.currentTiming = this.reloadInterval;
        SetMaterials();
    }

    // Update is called once per frame
    void Update()
    {

        if(this.currentTiming >= this.reloadInterval)
        {
            this.currentTiming = 0;

            var fs = new FileStream(this.FileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using (var sr = new StreamReader(fs))
            {
                var outputData = CSVReader.Read(sr.ReadToEnd());
                foreach (var d in outputData)
                {
                    var c = this.transform.Find(d["ID"].ToString());
                    if (c != null)
                    {
                        var v = c.GetComponent<DataViewer>();
                        switch (this.currentType)
                        {
                            case (0):
                                v.ViewData(-1);
                                break;
                            case 1:
                                v.ViewData((int)d["occupancy"]);
                                break;
                            case 2:
                                v.ViewData((int)d["air quality"]);
                                break;
                        }
                        
                    }
                }
            }

            
        }
        currentTiming += Time.deltaTime;
    }

    void SetMaterials()
    {
        if (this.currentType == 0)
        {
            var renderer = ModelRoot.GetComponentsInChildren<MeshRenderer>();

            for (int i=0; i<renderer.Length; i++)
            {
                if (!ToSkipMaterials.Contains(renderer[i].sharedMaterial))
                    renderer[i].sharedMaterial = this.oldMaterials[i];
            }
        }
        else
        {
            var renderer = ModelRoot.GetComponentsInChildren<MeshRenderer>();

            for (int i = 0; i < renderer.Length; i++)
            {
                if(!ToSkipMaterials.Contains(renderer[i].sharedMaterial))
                    renderer[i].sharedMaterial = this.defaultMaterial;
            }
        }
    }
}
