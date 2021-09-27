using System;
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

    [Header("Unit Info Panel")]
    public GameObject infoPanel;
    public Color invalidTextColor;
    private Text _infoPanelTitleText;
    private Text _infoPanelDescriptionText;
    private Transform _infoPanelResourcesCostParent;
    public GameObject gameResourceCostPrefab;

    [Header("Selected Unit Panel")]
    public Transform selectedUnitsListParent;
    public GameObject selectedUnitDisplayPrefab;

    [Header("Selected Group Indicators")]
    public Transform selectionGroupsParent;

    public GameObject selectedUnitMenu;
    private RectTransform _selectedUnitContentRectTransorm;
    private RectTransform _selectedUnitButtonsRectTransform;
    private Text _selectedUnitTitleText;
    private Text _selectedUnitLevelText;
    private Transform _selectedUnitResourcesProductionParent;
    private Transform _selectedUnitActionButtonsParent;
    public GameObject unitSkillButtonPrefab;
    private Unit _selectedUnit;

    private Dictionary<string, Text> _resourceTexts;
    private Dictionary<string, Button> _buildingButtons;

    private void OnEnable()
    {
        EventManager.AddListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.AddListener("CheckBuildingButtons", _OnCheckBuildingButtons);
        EventManager.AddListener("UnhoverBuildingButton", _OnUnhoverBuildingButton);

        EventManager.AddListener("HoverBuildingButton", _OnHoverBuildingButton);
        EventManager.AddListener("SelectUnit", _OnSelectUnit);
        EventManager.AddListener("DeselectUnit", _OnDeselectUnit);
        
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.RemoveListener("CheckBuildingButtons", _OnCheckBuildingButtons);
        EventManager.RemoveListener("UnhoverBuildingButton", _OnUnhoverBuildingButton);
        
        EventManager.RemoveListener("HoverBuildingButton", _OnHoverBuildingButton);
        EventManager.RemoveListener("SelectUnit", _OnSelectUnit);
        EventManager.RemoveListener("DeselectUnit", _OnDeselectUnit);
    }

    // Start is called before the first frame update
    void Awake()
    {
        _buildingPlacer = GetComponent<BuildingPlacer>();

        // Creates text for each in-game resource (info panel)
        Transform infoPanelTransform = infoPanel.transform;
        _infoPanelTitleText = infoPanelTransform.Find("Content/Title").GetComponent<Text>();
        _infoPanelDescriptionText = infoPanelTransform.Find("Content/Description").GetComponent<Text>();
        _infoPanelResourcesCostParent = infoPanelTransform.Find("Content/ResourcesCost");
        ShowInfoPanel(false);

        //Create texts for each in-game resource (gold, wood, stone)
        _resourceTexts = new Dictionary<string, Text>();
        foreach(KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            GameObject display = Instantiate(gameResourceDisplayPrefab);
            display.name = pair.Key;
            display.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/GameResources/{pair.Key}");
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
            button.GetComponent<BuildingButton>().Initialize(Globals.BUILDING_DATA[i]);
        }

        // hide all selection group buttons
        for(int i = 1; i <= 9; i++)
        {
            ToggleSelectionGroupButton(i, false);
        }

        Transform selectedUnitMenuTransform = selectedUnitMenu.transform;
        _selectedUnitContentRectTransorm = selectedUnitMenuTransform.Find("Content").GetComponent<RectTransform>();
        _selectedUnitButtonsRectTransform = selectedUnitMenuTransform.Find("Buttons").GetComponent<RectTransform>();
        _selectedUnitTitleText = selectedUnitMenuTransform.Find("Content/Title").GetComponent<Text>();
        _selectedUnitTitleText = selectedUnitMenuTransform.Find("Content/Title").GetComponent<Text>();
        _selectedUnitLevelText = selectedUnitMenuTransform.Find("Content/Level").GetComponent<Text>();
        _selectedUnitResourcesProductionParent = selectedUnitMenuTransform.Find("Content/ResourcesProduction");
        _selectedUnitActionButtonsParent = selectedUnitMenuTransform.Find("Buttons/SpecificActions");

        _ShowSelectedUnitMenu(false);

        
    }
    private void _SetSelectedUnitMenu(Unit unit)
    {
        _selectedUnit = unit;
        //adapt content panel heights to match info display
        int contentHeight = 60 + unit.Production.Count * 16;
        _selectedUnitContentRectTransorm.sizeDelta = new Vector2(60, contentHeight);
        _selectedUnitButtonsRectTransform.anchoredPosition = new Vector2(0, -contentHeight - 20);
        _selectedUnitButtonsRectTransform.sizeDelta = new Vector2(60, Screen.height - contentHeight -/*TEST: was 20*/ 50);
        // update texts
        _selectedUnitTitleText.text = unit.Data.unitName;
        _selectedUnitLevelText.text = $"Level{unit.Level}";
        // clear resource production and reinstantiate a new one
        foreach (Transform child in _selectedUnitResourcesProductionParent)
        {
            Destroy(child.gameObject);
        }
            
        if (unit.Production.Count > 0)
        {
            GameObject g; Transform t;
            foreach (ResourceValue resource in unit.Production)
            {
                g = Instantiate(gameResourceCostPrefab) as GameObject;
                t = g.transform;
                t.Find("Text").GetComponent<Text>().text = $"+{resource.amount}";
                t.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/GameResources/{resource.code}");
                t.SetParent(_selectedUnitResourcesProductionParent);
            }
        }
        // clear skills and reinstatiate new ones
        foreach (Transform child in _selectedUnitActionButtonsParent)
        {
            Destroy(child.gameObject);
        }
        if (unit.SkillManagers.Count > 0)
        {
            GameObject g; Transform t; Button b;
            for (int i = 0; i < unit.SkillManagers.Count; i++)
            {
                g = Instantiate(unitSkillButtonPrefab) as GameObject;
                t = g.transform;
                b = g.GetComponent<Button>();
                unit.SkillManagers[i].SetButton(b);
                t.Find("Text").GetComponent<Text>().text = unit.SkillManagers[i].skill.skillName;
                t.SetParent(_selectedUnitActionButtonsParent);
                _AddUnitSkillButtonListener(b, i);
            }
        }
    }

    private void _ShowSelectedUnitMenu(bool show)
    {
        selectedUnitMenu.SetActive(show);
        BuildingMenu.gameObject.SetActive(!show);
    }

    public void ToggleSelectionGroupButton(int groupIndex, bool on)
    {
        selectionGroupsParent.Find(groupIndex.ToString()).gameObject.SetActive(on);
    }

    private void _OnSelectUnit(object data)
    {
        Unit unit = (Unit)data;
        _AddSelectedUnitToUIList(unit);
        _SetSelectedUnitMenu(unit);
        _ShowSelectedUnitMenu(true);
    }

    private void _OnDeselectUnit(object data)
    {
        Unit unit = (Unit)data;
        _RemoveSelectedUnitFromUIList(unit.Code);
        if(Globals.SELECTED_UNITS.Count == 0)
        {
            _ShowSelectedUnitMenu(false);
        }
        else
        {
            _SetSelectedUnitMenu(Globals.SELECTED_UNITS[Globals.SELECTED_UNITS.Count - 1].Unit);
        }
    }

    public void _AddSelectedUnitToUIList(Unit unit)
    {
        // if there is another unit of the same type already selected,
        // increase the counter
        Transform alreadyInstantiatedChild = selectedUnitsListParent.Find(unit.Code);
        if(alreadyInstantiatedChild != null)
        {
            Text t = alreadyInstantiatedChild.Find("Count").GetComponent<Text>();
            int count = int.Parse(t.text);
            t.text = (count + 1).ToString();
        }

        // else create a brand new counter initialized with a count of 1
        else
        {
            GameObject g = Instantiate(selectedUnitDisplayPrefab) as GameObject;
            g.name = unit.Code;
            Transform t = g.transform;
            t.Find("Count").GetComponent<Text>().text = "1";
            t.Find("Name").GetComponent<Text>().text = unit.Data.unitName;
            t.SetParent(selectedUnitsListParent);
        }
    }

    public void _RemoveSelectedUnitFromUIList(string code)
    {
        Transform listItem = selectedUnitsListParent.Find(code);
        if(listItem == null) { return; }
        Text t = listItem.Find("Count").GetComponent<Text>();
        int count = int.Parse(t.text);
        count -= 1;
        if (count == 0)
        {
            DestroyImmediate(listItem.gameObject);
        }
        else
        {
            t.text = count.ToString();
        }
    }

    public void _SetInfoPanel(UnitData data)
    {
        // update texts
        if (data.code != "") { _infoPanelTitleText.text = data.unitName; }
        if (data.description != "") { _infoPanelDescriptionText.text = data.description; }

        //clear resource costs and reinstantiate new ones
        foreach(Transform child in _infoPanelResourcesCostParent) { Destroy(child.gameObject); }
        if(data.cost.Count > 0)
        {
            GameObject g;
            Transform t;
            foreach(ResourceValue resource in data.cost)
            {
                g = Instantiate(gameResourceCostPrefab) as GameObject;
                t = g.transform;
                t.Find("Text").GetComponent<Text>().text = resource.amount.ToString();
                t.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/GameResources/{resource.code}");
                t.SetParent(_infoPanelResourcesCostParent);

                // checks to see if resource requirments is not 
                // currently met - in that case, turn the text into the 'invalid"
                // color.
                if (Globals.GAME_RESOURCES[resource.code].Amount < resource.amount)
                {
                    t.Find("Text").GetComponent<Text>().color = invalidTextColor;
                }
            }
        }

        
    }
    public void ShowInfoPanel(bool show)
    {
        infoPanel.SetActive(show);
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

    private void _OnHoverBuildingButton(object data)
    {
        _SetInfoPanel((UnitData)data);
        ShowInfoPanel(true);
    }

    private void _OnUnhoverBuildingButton()
    {
        ShowInfoPanel(false);
    }
    private void _OnUpdateResourceTexts()
    {
        foreach(KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            _SetResourceText(pair.Key, pair.Value.Amount);
        }
    }

    private void _AddUnitSkillButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => _selectedUnit.TriggerSkill(i));
    }

    private void _AddBuildingButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => _buildingPlacer.SelectPlacedBuilding(i));
    }


}
