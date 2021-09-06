using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;

    public void Awake()
    {
        cam = GameObject.Find("ParentCamera").transform;
    }

    void LateUpdate()
    {
        transform.LookAt(cam.position);
    }
}
