using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowingTarget : MonoBehaviour {
    public Transform Target;
    public float followSpeed = 0.8f;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //print((Target.position - this.transform.position) * followSpeed);
        this.transform.Translate((Target.position - this.transform.position) * followSpeed);
    }
}
