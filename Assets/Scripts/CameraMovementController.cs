using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementController : MonoBehaviour
{
    public float speed = 0.06f;
    public float zoomSpeed = 10f;
    public float yRotateSpeed = 0.1f;
    public float xRotateStep = 5f;

    public float maxHeight = 20.0f;
    public float minHeight = 4.0f;

    public float maxRotation = 45.0f;
    public float minRotation = 15.0f;

    public GameObject cameraObject;

    Vector2 p1;
    Vector2 p2;
    // Start is called before the first frame update
    void Start()
    {
        cameraObject = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // Fast Mode - moves camera faster when the Shift Key is held
        if (Input.GetKey(KeyCode.LeftShift))
        {
            
            speed = 0.06f;
            zoomSpeed = 20.0f;
        }
        else
        {
            speed = 0.035f;
            zoomSpeed = 10.0f;
        }
        //Fast Mode End */

        float hsp = transform.position.y * speed * Input.GetAxis("Horizontal") * Time.deltaTime;
        float vsp = transform.position.y * speed * Input.GetAxis("Vertical") * Time.deltaTime;
        float scrollSp = Mathf.Log(transform.position.y) * -zoomSpeed * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;

        //Height Limiter - Limits camera height between minHeight and max Height values.
        if((transform.position.y >= maxHeight) && (scrollSp > 0))
        {
            scrollSp = 0;
        }
        else if ((transform.position.y <= minHeight) && (scrollSp < 0))
        {
            scrollSp = 0;
        }

        if((transform.position.y + scrollSp) > maxHeight)
        {
            scrollSp = maxHeight - transform.position.y;
        }
        else if((transform.position.y + scrollSp) < minHeight)
        {
            scrollSp = minHeight - transform.position.y;
        }

        Vector3 verticalMove = new Vector3(0, scrollSp, 0);
        Vector3 lateralMove = hsp * transform.right;
        Vector3 forwardMove = transform.forward;

        //Vector Projection for forwardMove to stop the camera decending when it is moved forward.
        forwardMove.y = 0.0f;
        forwardMove.Normalize();
        forwardMove *= vsp;
        //Vector Projection Complete

        Vector3 move = verticalMove + lateralMove + forwardMove;

        transform.position += move;


        GetCameraTilt();
        GetCameraRotation();
    }

    void GetCameraTilt()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            float xRotation = cameraObject.transform.localEulerAngles.x;
            Debug.Log("Pressed Z");
            if (xRotation - xRotateStep < minRotation)
            {
                cameraObject.transform.localEulerAngles = new Vector3(xRotation, cameraObject.transform.localEulerAngles.y, cameraObject.transform.localEulerAngles.z);
            }
            else
            {
                xRotation -= xRotateStep;
                cameraObject.transform.localEulerAngles = new Vector3(xRotation, cameraObject.transform.localEulerAngles.y, cameraObject.transform.localEulerAngles.z);
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            float xRotation = cameraObject.transform.localEulerAngles.x;
            Debug.Log("Pressed X");
            if (xRotation + xRotateStep > maxRotation)
            {
                cameraObject.transform.localEulerAngles = new Vector3(xRotation, cameraObject.transform.localEulerAngles.y, cameraObject.transform.localEulerAngles.z);
            }
            else
            {
                xRotation += xRotateStep;
                cameraObject.transform.localEulerAngles = new Vector3(xRotation, cameraObject.transform.localEulerAngles.y, cameraObject.transform.localEulerAngles.z);
            }
        }
    }

    void GetCameraRotation()
    {
        if(Input.GetMouseButtonDown(2))
        {
            p1 = Input.mousePosition;
        }

        if(Input.GetMouseButton(2))
        {
            p2 = Input.mousePosition;
            float dx = (p2 - p1).x * yRotateSpeed * Time.deltaTime;
            float dy = (p2 - p1).y * yRotateSpeed * Time.deltaTime;

            transform.rotation *= Quaternion.Euler(new Vector3(0, dx, 0));
            //transform.GetChild(0).transform.rotation *= Quaternion.Euler(new Vector3(-dy, 0, 0));
        }

    }

}
