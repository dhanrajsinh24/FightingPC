using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRotator : MonoBehaviour
{
    // Start is called before the first frame update
    public float RotateSpeedX;
    public float RotateSpeedY;
    public float RotateSpeedZ;
    public float GeneralRotateSpeed;

    // Use this for initialization
    void Start()
    {
        RotateSpeedX *= GeneralRotateSpeed;
        RotateSpeedY *= GeneralRotateSpeed;
        RotateSpeedZ *= GeneralRotateSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(+RotateSpeedX * Time.smoothDeltaTime, +RotateSpeedY * Time.smoothDeltaTime, +RotateSpeedZ * Time.smoothDeltaTime);
    }

    public void InteractAction()
    {
        RotateSpeedX = -RotateSpeedX;
        RotateSpeedY = -RotateSpeedY;
        RotateSpeedZ = -RotateSpeedZ;
    }
}
