using UnityEngine;

public class Healthbar : MonoBehaviour
{
    public RectTransform rectTransform;

    private Transform _target;
    private Transform _camera;
    private bool _reupdate;
    private Vector3 _lastCameraPosition;
    private Vector3 _lastTargetPosition;
    private Vector2 _pos;
    private float _yOffset;

    private void Awake()
    {
        _camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
       if(_lastCameraPosition == _camera.position && (!_reupdate || _target && _lastTargetPosition == _target.position))
        {
            return;
        }
        SetPosition();
    }

    public void Initialize(Transform target, bool reupdate, float yOffset)
    {
        _target = target;
        _reupdate = reupdate;
        _yOffset = yOffset;
    }
    public void SetPosition()
    {
        if(!_target)
        {
            return;
        }

        _pos = Camera.main.WorldToScreenPoint(_target.position);
        _pos.y += _yOffset;
        rectTransform.anchoredPosition = _pos;
        _lastTargetPosition = _target.position;
    }
}
