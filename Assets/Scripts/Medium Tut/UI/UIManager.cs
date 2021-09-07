using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private BuildingPlacer _buildingPlacer;

    [Header("Building")]
    public Transform BuildingMenu;
    public GameObject buildingButtonPrefab;
    
    [Header("Game Resources")]
    public Transform resourcesUIParent;
    public GameObject gameResourceDisplayPrefab;

    private Dictionary<string, Text> _resourceTexts;
    private Dictionary<string, Button> _buildingButtons;

    private void OnEnable()
    {
        EventManager.AddListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.AddListener("CheckBuildingButtons", _OnCheckBuildingButtons);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.RemoveListener("CheckBuildingButtons", _OnCheckBuildingButtons);
    }

    // Start is called before the first frame update
    void Awake()
    {
        _buildingPlacer = GetComponent<BuildingPlacer>();

        //Create texts for each in-game resource (gold, wood, stone)
        _resourceTexts = new Dictionary<string, Text>();
        foreach(KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            GameObject display = Instantiate(gameResourceDisplayPrefab);
            display.name = pair.Key;
            _resourceTexts[pair.Key] = display.transform.Find("Text").GetComponent<Text>();
            _SetResourceText(pair.Key, pair.Value.Amount);
            display.transform.SetParent(resourcesUIParent);
        }

        // create buttons for each building type
        _buildingButtons = new Dictionary<string, Button>();
        for (int i = 0; i < Globals.BUILDING_DATA.Length; i++)
        {
            
            BuildingData data = Globals.BUILDING_DATA[i];
            GameObject button = Instantiate(buildingButtonPrefab);
            Button b = button.GetComponent<Button>();
            button.name = data.unitName;
            button.transform.Find("Text").GetComponent<Text>().text = data.unitName;
            _buildingButtons[data.code] = b;
            
            _AddBuildingButtonListener(b, i);
            button.transform.SetParent(BuildingMenu);
            if(!Globals.BUILDING_DATA[i].CanBuy())
            {
                b.interactable = false;
            }
        }
        
    }

    private void _OnCheckBuildingButtons()
    {
        foreach(BuildingData data in Globals.BUILDING_DATA)
        {
            _buildingButtons[data.code].interactable = data.CanBuy();
        }
    }

    private void _SetResourceText(string resource, int value)
    {
        _resourceTexts[resource].text = value.ToString();
    }

    private void _OnUpdateResourceTexts()
    {
        foreach(KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            _SetResourceText(pair.Key, pair.Value.Amount);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void _AddBuildingButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => _buildingPlacer.SelectPlacedBuilding(i));
    }


}
