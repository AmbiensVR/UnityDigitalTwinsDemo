using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OrbitCamera : MonoBehaviour
{

    [SerializeField]public Transform target;
    private Transform defaultTarget;

    [Header("Speed")]
    [SerializeField] float moveSpeed = 300f;
    [SerializeField] float MobileMoveSpeed = 300f;
    [SerializeField] float zoomSpeed = 100f;

    [Header("Zoom")]
    [SerializeField] public float minDistance = 2f;
    [SerializeField] public float maxDistance = 5f;

    public float minY = 5f;
    public float maxY = 30f;


    private float currentInputX = 0;
    private float currentInputY = 0;

    public float inertia = 0.1f;

    private float defaultMinDistance = 0f;
    private float defaultMaxDistance = 0f;
    private float defaultMinY = 0f;

    private void Start()
    {
        if (this.target != null)
        {
            this.Init();
        }
        
    }
    public void Init()
    {
        this.defaultTarget = this.target.transform;

        this.defaultMinDistance = this.minDistance;
        this.defaultMaxDistance = this.maxDistance;
        this.defaultMinY = this.minY;

        this.transform.position = this.target.position + this.target.forward * this.defaultMinDistance + Vector3.up * 5;

    }
    void Update()
    {
        if (this.target != null)
        {
            this.CalculateInput();
            CameraControl();
        }
    }

    Vector2 firstTouch = new Vector2();

    private bool MustCheckDistance = true;
    private bool hasReachedForcedPosition = true;
    public Vector3 targetPosition = Vector3.zero;

    void CalculateInput()
    {
        if (Input.touchCount > 0 /*&& Input.GetTouch(0).phase == TouchPhase.Moved*/)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
                this.firstTouch = Input.GetTouch(0).position;
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 delta = Input.GetTouch(0).position - this.firstTouch;

                this.currentInputX = delta.x / Screen.width * MobileMoveSpeed * Time.deltaTime;
                this.currentInputY = delta.y / Screen.height * MobileMoveSpeed * Time.deltaTime;
            }
        }
        else if(Input.GetMouseButtonDown(0)|| Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.firstTouch = Input.mousePosition;
            }
            else if(Input.GetMouseButton(0))
            {
                Vector2 delta = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - this.firstTouch;

                this.currentInputX = delta.x / Screen.width * MobileMoveSpeed * Time.deltaTime;
                this.currentInputY = delta.y / Screen.height * MobileMoveSpeed * Time.deltaTime;
            }
        }
        else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            this.currentInputX = (Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime);
            this.currentInputY = (Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime);
        }
        else
        {
            this.currentInputX = Mathf.Lerp(this.currentInputX, 0, inertia);
            this.currentInputY = Mathf.Lerp(this.currentInputY, 0, inertia);
        }

    }

    void CameraControl()
    {
        if (this.currentInputX != 0 || this.currentInputY != 0)
        {
            transform.RotateAround(target.transform.position, Vector3.up, ((this.currentInputX)));
            if(Vector3.Angle(Vector3.up, -transform.forward)>10) //FIX TO AVOID PROBLEM AROUND THE TOP OF THE "SPHERE"
            {
                transform.RotateAround(target.transform.position, transform.right, -((this.currentInputY)));
            }
            else{
                transform.RotateAround(target.transform.position, transform.right, -1);
            }

        }
        var distance = Vector3.Distance(this.transform.position, this.targetPosition);
        var distFromTarget = Vector3.Distance(this.transform.position, this.target.position);

        this.transform.LookAt(target.transform.position);

        if (this.MustCheckDistance)
        {
            if (this.transform.position.y < minY)
            {
                this.transform.position = Vector3.Lerp(this.transform.position,new Vector3(this.transform.position.x, minY, this.transform.position.z),0.3f);
            }
            if (this.transform.position.y > maxY)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(this.transform.position.x, maxY, this.transform.position.z),0.3f);
            }
        }

        if (!this.hasReachedForcedPosition)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, this.targetPosition, 0.05f);
            
            if (distance < 0.3f)
            {
                this.hasReachedForcedPosition = true;

            }
        }

        ZoomCamera();
        DefaultMovement();
        SetDofDistance(distFromTarget);
    }

    void DefaultMovement()
    {
        this.currentInputX += 0.002f;
    }

    void ZoomCamera()
    {
        /* If we are already close enough for the min distance and we try to zoom in, dont, return instead */
        /* Similarly for zooming out */
        var direction = transform.position - target.transform.position;
        var magnitude = Vector3.Distance(transform.position, target.transform.position);

        if (magnitude <= minDistance && (Input.GetAxis("Mouse ScrollWheel") > 0f ))
        {
            return;
        }
        if (magnitude >= maxDistance && (Input.GetAxis("Mouse ScrollWheel") < 0f ))
        {
            return;
        }
        if (this.MustCheckDistance)
        {
            if (magnitude < minDistance)
            {
                var dist = Mathf.Lerp(magnitude, minDistance, 0.05f);
                this.transform.position = target.transform.position + direction.normalized * dist;
            }
            if (magnitude > maxDistance)
            {
                var dist = Mathf.Lerp(magnitude, maxDistance, 0.05f);
                this.transform.position = target.transform.position + direction.normalized * dist;
            }
        }


        /* Only move in the Z relative to the Camera (so forward and back) */
        transform.Translate(
            0f,
            0f,
            (Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime) * zoomSpeed,
            Space.Self
        );
    }
    DepthOfField dofComponent;

    void SetDofDistance(float dist)
    {
        if (dofComponent == null)
        {
            Volume volume = FindObjectOfType<Volume>();
            DepthOfField tmp;
            if (volume.profile.TryGet<DepthOfField>(out tmp))
            {
                dofComponent = tmp;
            }
        }
        else
        {
            dofComponent.focusDistance.value = dist;

        }
    }
}
