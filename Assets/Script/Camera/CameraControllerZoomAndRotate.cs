using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerZoomAndRotate : MonoBehaviour {
    public Vector2 ZoomMinMax;
    public Transform cameraRotateObject;
    public Transform cameraZoomObject;
    public float ZoomSpeed = 30f;
    public float RotateSpeed = 180f;
    
    public KeyCode ZoomIn;
    public KeyCode ZoomOut;
    public KeyCode RotatePlus;
    public KeyCode RotateMinus;

    public static Quaternion CameraRotate;

    // Use this for initialization
    void Start () {
		
	}


	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(ZoomIn))       cameraZoomObject.Translate(new Vector3(0, 0, -ZoomSpeed) * Time.deltaTime);
        if (Input.GetKey(ZoomOut))      cameraZoomObject.Translate(new Vector3(0, 0, ZoomSpeed) * Time.deltaTime);
        if (cameraZoomObject.localPosition.z < ZoomMinMax.x) cameraZoomObject.localPosition = new Vector3(0, 0, ZoomMinMax.x);
        if (cameraZoomObject.localPosition.z > ZoomMinMax.y) cameraZoomObject.localPosition = new Vector3(0, 0, ZoomMinMax.y);

        if (Input.GetKey(RotatePlus))   cameraRotateObject.Rotate(new Vector3(0, RotateSpeed, 0) * Time.deltaTime);
        if (Input.GetKey(RotateMinus))  cameraRotateObject.Rotate(new Vector3(0, -RotateSpeed, 0) * Time.deltaTime);

        CameraRotate = cameraRotateObject.rotation;
    }
}
