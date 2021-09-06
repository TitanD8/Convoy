using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BuildingManager : UnitManager
{
    private BoxCollider _collider;

    private Building _building;
    private int _nCollisions = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Intialize(Building building)
    {
        _collider = GetComponent<BoxCollider>();
        _building = building;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrain") return;
        _nCollisions++;
        CheckPlacement();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Terrain") return;
        _nCollisions--;
        CheckPlacement();
    }
    
    public bool CheckPlacement()
    {
        if (_building == null) return false;
        if (_building.IsFixed) return false;
        bool validPlacement = HasValidPlacement();
        if(!validPlacement)
        {
            _building.SetMaterials(BuildingPlacement.INVALID);
        }
        else
        {
            _building.SetMaterials(BuildingPlacement.VALID);
        }
        return validPlacement;
    }

    public bool HasValidPlacement()
    {
        if (_nCollisions > 0) return false;

        //get 4 bottom corner positions
        Vector3 position = transform.position;
        Vector3 center = _collider.center;
        Vector3 e = _collider.size / 2f;
        float bottomHeight = center.y - e.y + 0.5f;
        Vector3[] bottomCorners = new Vector3[]
        {
            new Vector3(center.x - e.x, bottomHeight, center.z - e.z),
            new Vector3(center.x - e.x, bottomHeight, center.z - e.z),
            new Vector3(center.x - e.x, bottomHeight, center.z - e.z),
            new Vector3(center.x - e.x, bottomHeight, center.z - e.z),
        };
        // cast a small ray beneath the corner to check for a close ground
        // (if at least two are not valid, then placement is invalid)
        int invalidCornersCount = 0;
        foreach (Vector3 corner in bottomCorners)
        {
            if (!Physics.Raycast(
                position + corner,
                Vector3.up * -1f,
                2f,
                Globals.TERRAIN_LAYER_MASK
            ))
                invalidCornersCount++;
        }
        return invalidCornersCount < 3;

    }

    protected override bool IsActive()
    {
        return _building.IsFixed;
    }
}