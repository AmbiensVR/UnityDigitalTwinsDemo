using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTarget : MonoBehaviour
{
    public OrbitCamera cam;
    public Transform target;
    public float maxDistance;
    public float minY = 2;
    public void Start()
    {
        if ( cam == null ) cam = FindObjectOfType<OrbitCamera>();
        if (target == null) target = this.transform;
    }
    public void Switch()
    {
        this.cam.target = target;
        this.cam.maxDistance = this.maxDistance;
        this.cam.minY = this.target.position.y+this.minY;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, 1);
    }
}
