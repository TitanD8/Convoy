using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{

    private UIManager _uIManager;
    private Building _placedBuilding = null;

    //raycast variables
    private Ray _ray;
    private RaycastHit _raycastHit;
    private Vector3 _lastPlacementPosition;

    private void Awake()
    {
        _uIManager = GetComponent<UIManager>();
    }

    public void SelectPlacedBuilding(int buildingDataIndex)
    {
        _PreparePlacedBuilding(buildingDataIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (_placedBuilding != null)
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                _CancelPlacedBuilding();
                return;
            }

            // ... do the raycast
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(
                _ray,
                out _raycastHit,
                1000f,
                Globals.TERRAIN_LAYER_MASK
            ))
            {
                _placedBuilding.SetPosition(_raycastHit.point);
                if(_lastPlacementPosition != _raycastHit.point)
                {
                    _placedBuilding.CheckValidPlacement();
                }

                _lastPlacementPosition = _raycastHit.point;
            }

            if (_placedBuilding.HasValidPlacement && Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            { 
                    _PlaceBuilding();
            }
        }
    }

    void _PreparePlacedBuilding(int buildingDataIndex)
    {
        // Destroy the previous 'phantom' if there is one
        if(_placedBuilding != null && !_placedBuilding.IsFixed)
        {
            Destroy(_placedBuilding.Transform.gameObject);
        }
        Building building = new Building(
            Globals.BUILDING_DATA[buildingDataIndex]
        );

        //Link the data into the manager
        building.Transform.GetComponent<BuildingManager>().Intialize(building);
        _placedBuilding = building;
        _lastPlacementPosition = Vector3.zero;
    }

    void _PlaceBuilding()
    {
        _placedBuilding.Place();
        if (_placedBuilding.CanBuy())
        {
            _PreparePlacedBuilding(_placedBuilding.DataIndex);
        }    
        else
        {
            _placedBuilding = null;
        }
        //_uIManager.UpdateResourceTexts();
        //_uIManager.CheckBuildingButtons();
        EventManager.TriggerEvent("UpdateResourceTexts");
        EventManager.TriggerEvent("CheckBuildingButtons");
    }

    void _CancelPlacedBuilding()
    {
        //Destroy the 'phantom' building
        Destroy(_placedBuilding.Transform.gameObject);
        _placedBuilding = null;
    }
}
