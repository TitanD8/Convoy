using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public GameObject healthBar;
    private bool _hovered = false;

    private void Awake()
    {
        healthBar.SetActive(false);
    }
    private void OnMouseEnter()
    {
        _hovered = true;
    }

    private void OnMouseExit()
    {
        _hovered = false;
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

    private void _SelectUtil()
    {
        if (Globals.SELECTED_UNITS.Contains(this)) return;
        Globals.SELECTED_UNITS.Add(this);
        healthBar.SetActive(true);
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
        healthBar.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


}