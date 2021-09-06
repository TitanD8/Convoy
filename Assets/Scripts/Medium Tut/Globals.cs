using System.Collections.Generic;

public class Globals
{
    public static int TERRAIN_LAYER_MASK = 1 << 8;

    public static BuildingData[] BUILDING_DATA;
    

    public static Dictionary<string, GameResource> GAME_RESOURCES = new Dictionary<string, GameResource>()
    {
        {"gold", new GameResource("Gold", 1000) },
        {"wood", new GameResource("Wood", 1000) },
        {"stone", new GameResource("Stone", 1000) }
    };

    public static List<UnitManager> SELECTED_UNITS = new List<UnitManager>();
}
