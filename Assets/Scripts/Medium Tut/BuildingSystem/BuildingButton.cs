using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private BuildingData _buildingData;

    public void Initialize(BuildingData buildingData)
    {
        _buildingData = buildingData;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("On");
        EventManager.TriggerEvent("HoverBuildingButton", _buildingData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Off");
        EventManager.TriggerEvent("UnhoverBuildingButton");
    }
}
