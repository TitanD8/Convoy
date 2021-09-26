using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    public float translationSpeed = 60f;
    public float altitude = 40f;
    public float zoomSpeed = 30f;

    private Camera _camera;
    private RaycastHit _hit;
    private Ray _ray;

    private Vector3 _forwardDir;

    private int _mouseOnScreenBorder;
    private Coroutine _mouseOnScreenCoroutine;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _forwardDir = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        _mouseOnScreenBorder = -1;
        _mouseOnScreenCoroutine = null;
    }

    private void Update()
    {
        if (_mouseOnScreenBorder >= 1)
        {
            _TranslateCamera(_mouseOnScreenBorder);
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                _TranslateCamera(1);
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                _TranslateCamera(2);
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                _TranslateCamera(3);
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                _TranslateCamera(4);
            }
        }

        if(Mathf.Abs(Input.mouseScrollDelta.y) > 0)
        {
            _Zoom(Input.mouseScrollDelta.y > 0f ? 1 : -1);
        }
    }

    private void _TranslateCamera(int dir)
    {
        if (dir == 1) // top
        {
            transform.Translate(_forwardDir * Time.deltaTime * translationSpeed, Space.World);
        }
        else if (dir == 2) // right
        {
            transform.Translate(transform.right * Time.deltaTime * translationSpeed, Space.World);
        }
        else if (dir == 3) // bottom
        {
            transform.Translate(-_forwardDir * Time.deltaTime * translationSpeed, Space.World);
        }
        else if (dir == 4) // left
        {
            transform.Translate(-transform.right * Time.deltaTime * translationSpeed, Space.World);
        }

        // Translate the camera at proper altitude: cast ray to ground
        // and move up the hit point.
        _ray = new Ray(transform.position, Vector3.up * -1000f);
        if(Physics.Raycast(_ray, out _hit, 1000f, Globals.TERRAIN_LAYER_MASK))
        {
            transform.position = _hit.point + (Vector3.up * altitude);
        }
    }

    public void OnMouseEnterScreenBorder(int borderIndex)
    {
        _mouseOnScreenCoroutine = StartCoroutine(_SetMouseOnScreenBorder(borderIndex));
    }

    public void OnMouseExitScreenBorder()
    {
        StopCoroutine(_mouseOnScreenCoroutine);
        _mouseOnScreenBorder = -1;
    }

    private IEnumerator _SetMouseOnScreenBorder(int borderIndex)
    {
        yield return new WaitForSeconds(0.3f);
        _mouseOnScreenBorder = borderIndex;
    }

    private void _Zoom(int zoomDir)
    {
        //apply Zoom
        _camera.orthographicSize += zoomDir * Time.deltaTime * zoomSpeed;
        //clamp camera distance
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, 8f, 26f);
    }
}
