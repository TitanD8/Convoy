using System.Collections.Generic;

public class Globals
{
    public static int TERRAIN_LAYER_MASK = 1 << 8;

    public static BuildingData[] BUILDING_DATA;
    

    public static Dictionary<string, GameResource> GAME_RESOURCES = new Dictionary<string, GameResource>()
    {
        {"gold", new GameResource("gold", 1000) },
        {"wood", new GameResource("wood", 1000) },
        {"stone", new GameResource("stone", 1000) }
    };

    public static List<UnitManager> SELECTED_UNITS = new List<UnitManager>();
}
