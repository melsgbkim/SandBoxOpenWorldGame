using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePlayerMoveDirection : MonoBehaviour {
    public Transform CameraRotate;
    //public PlayerMove Player;
    public PlayerInput Player;

    private Vector3 moveTo = new Vector3(0, 0, 1);
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Player.Forword = CameraRotate.rotation * moveTo;

    }
}
