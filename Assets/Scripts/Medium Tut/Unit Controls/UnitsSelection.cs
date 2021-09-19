using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitsSelection : MonoBehaviour
{
    public UIManager uiManager;

    private bool _isDraggingMouseBox = false;
    private Vector3 _dragStartPosition;

    private Dictionary<int, List<UnitManager>> _selectionGroups = new Dictionary<int, List<UnitManager>>();

    Ray _ray;
    RaycastHit _raycastHit;


    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { return; }
        if(Input.GetMouseButtonDown(0))
        {
            _isDraggingMouseBox = true;
            _dragStartPosition = Input.mousePosition;
        }

        if(Input.GetMouseButtonUp(0))
        { _isDraggingMouseBox = false; }

        if(_isDraggingMouseBox && _dragStartPosition !=Input.mousePosition)
        {
            _SelectUnitsInDraggingBox();
        }

        if (Globals.SELECTED_UNITS.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                _DeselectAllUnits();
            if (Input.GetMouseButtonDown(0))
            {
                _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(
                    _ray,
                    out _raycastHit,
                    1000f
                ))
                {
                    if (_raycastHit.transform.tag == "Terrain")
                        _DeselectAllUnits();
                }
            }
        }

        // Manage Selection Groups with Alphanumeric Keys
        if(Input.anyKeyDown)
        {
            int alphaKey = Utils.GetAlphaKeyValue(Input.inputString);
            Debug.Log(alphaKey);
            if (alphaKey != -1)
            {

                //NOTE: I have had to make the creation key Enter as Ctrl + 1 opens
                //      Sceen View.

                if(Input.GetKey(KeyCode.Return)
                    //(Input.GetKey(KeyCode.LeftControl) ||
                    //Input.GetKey(KeyCode.RightControl)
                  )
                { _CreateSelectionGroup(alphaKey); Debug.Log("Create"); }
                else
                { _reselectGroup(alphaKey); }
            }
        }
    }

    public void SelectsUnitsGroup(int groupIndex)
    {
        _reselectGroup(groupIndex);
    }

    private void _CreateSelectionGroup(int groupIndex)
    {
        // Check there are units currently selected
        if (Globals.SELECTED_UNITS.Count == 0)
        {
            if (_selectionGroups.ContainsKey(groupIndex))
            { _RemoveSelectionGroup(groupIndex); }
            return;
        }
        List<UnitManager> groupUnits = new List<UnitManager>(Globals.SELECTED_UNITS);
        _selectionGroups[groupIndex] = groupUnits;
        uiManager.ToggleSelectionGroupButton(groupIndex, true);
    }

    private void _RemoveSelectionGroup(int groupIndex)
    {
        _selectionGroups.Remove(groupIndex);
        uiManager.ToggleSelectionGroupButton(groupIndex, false);
    }

    private void _reselectGroup(int groupIndex)
    {
        // Check if group is actually defined
        if(!_selectionGroups.ContainsKey(groupIndex)) { return; }
        _DeselectAllUnits();
        foreach(UnitManager um in _selectionGroups[groupIndex])
        {
            um.Select();
        }
    }

    private void _SelectUnitsInDraggingBox()
    {
        Bounds selectionBounds = Utils.GetViewportBounds(
            Camera.main,
            _dragStartPosition,
            Input.mousePosition
        );
        GameObject[] selectableUnits = GameObject.FindGameObjectsWithTag("Unit");
        bool inBounds;
        foreach (GameObject unit in selectableUnits)
        {
            inBounds = selectionBounds.Contains(Camera.main.WorldToViewportPoint(unit.transform.position));

            if (inBounds)
            {
                unit.GetComponent<UnitManager>().Select();
                
            }
            else
            {
                unit.GetComponent<UnitManager>().Deselect();      
                
            }
        }
    }

    private void _DeselectAllUnits()
    {
        List<UnitManager> selectedUnits = new List<UnitManager>(Globals.SELECTED_UNITS);
        foreach (UnitManager um in selectedUnits)
        {
            um.Deselect();
        }
    }

    private void OnGUI()
    {
        if(_isDraggingMouseBox)
        {
            // create a rect from both mouse positions
            var rect = Utils.GetScreenRect(_dragStartPosition, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.5f, 1f, 0.4f, 0.2f));
            Utils.DrawScreenRectBorder(rect, 1, new Color(0.5f, 1f, 0.4f));
        }
    }
}
