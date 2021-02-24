using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataViewer : MonoBehaviour
{
    public MeshRenderer rend;
    public Material instantiatedMaterial;
    public static Color color1 = new Color(0, 1f, 0, 1f);
    public static Color color2 = new Color(1f, 0, 0, 1f);

    private Color targetColor;

    public void Start()
    {
        this.rend = this.GetComponent<MeshRenderer>();
        instantiatedMaterial = this.rend.material;
    }


    public void ViewData(int perc)
    {
        if (perc == -1)
        {
            targetColor = new Color(0, 0, 0, 0);
        }
        else
        {
            targetColor = Color.Lerp(color1, color2, (float)perc / 100f);
        }
    }

    private void Update()
    {
        this.instantiatedMaterial.SetColor("_Color", 
            Color.Lerp(this.instantiatedMaterial.GetColor("_Color"), 
            targetColor, 
            Time.deltaTime));
    }
}
