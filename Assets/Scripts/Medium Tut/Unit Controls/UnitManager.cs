using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class UnitManager : MonoBehaviour
{
    private bool _hovered = false;

    private Transform _canvas;
    private GameObject _healthbar;
    public BoxCollider _collider;
    public virtual Unit Unit { get; set; }

    public void Awake()
    {
        _canvas = GameObject.Find("Canvas").transform;
    }

    private void OnMouseEnter()
    {
        _hovered = true;
    }

    private void OnMouseExit()
    {
        _hovered = false;
    }

    public void Intialize(Unit unit)
    {
        _collider = GetComponent<BoxCollider>();
        Unit = unit;
    }

    // Update is called once per frame
    void Update()
    {
        if (_hovered && Input.GetMouseButtonDown(0) && IsActive())
        {
            Select(
                true,
                Input.GetKey(KeyCode.LeftShift) ||
                Input.GetKey(KeyCode.RightShift)
             );
        }
    }

    protected virtual bool IsActive() { return true; }

    protected virtual bool IsMovable() { return true; }

    private void _SelectUtil()
    {
        if (Globals.SELECTED_UNITS.Contains(this)) return;
        Globals.SELECTED_UNITS.Add(this);

        if(_healthbar == null)
        {
            _healthbar = GameObject.Instantiate(Resources.Load("Prefabs/UI/Healthbar")) as GameObject;
            _healthbar.transform.SetParent(_canvas);
            Healthbar h = _healthbar.GetComponent<Healthbar>();
            Rect boundingBox = Utils.GetBoundingBoxOnScreen(
                transform.Find("Mesh").GetComponent<Renderer>().bounds,
                Camera.main
            );
            h.Initialize(transform, IsMovable(), boundingBox.height);
            h.SetPosition();

            
        }

        EventManager.TriggerEvent("SelectUnit", Unit);
    }

    public void Select() { Select(false, false); }

    public void Select(bool singleClick, bool holdingShift )
    {
        //basic case: using the selection box.
        if(!singleClick)
        {
            _SelectUtil();
            return;
        }

        //single click: check for shift key.
        if (!holdingShift)
        {
            List<UnitManager> selectedUnits = new List<UnitManager>(Globals.SELECTED_UNITS);
            foreach (UnitManager um in selectedUnits)
            {
                um.Deselect();
            }
            _SelectUtil();
        }
        else
        {
            if (!Globals.SELECTED_UNITS.Contains(this))
            {
                _SelectUtil();
            }
            else
            {
                Deselect();
            }
        }
        
    }

    public void Deselect()
    {
        if (!Globals.SELECTED_UNITS.Contains(this)) return;
        Globals.SELECTED_UNITS.Remove(this);
        Destroy(_healthbar);
        _healthbar = null;

        EventManager.TriggerEvent("DeselectUnit", Unit);
    }
}
