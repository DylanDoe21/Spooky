using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace Spooky
{
    public class SpookyConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Halloween Season")]
        [Tooltip("The spooky mod makes it so the halloween seasonal event is always active"
        + "\nTurn off to disable the halloween seasonal event being active, enabled by default"
        + "\nThe halloween seasonal event will still occur during October 10th-31st regardless"
        + "\nYou will need to leave and re-enter the world for this to take effect")]
        [DefaultValue(true)]
        public bool HalloweenEnabled { get; set; }

        [Label("Spooky Forest Spawnpoint")]
        [Tooltip("Changes the position where the spooky forest is placed during world generation"
        + "\nKeeping this option on will make it placed at spawn, which is the default option"
        + "\nTurning this off will cause the spooky forest to generate away from spawn"
        + "\nWARNING: Playing on small worlds with this option off may destroy parts of other biomes!")]
        [DefaultValue(true)]
        public bool SpookyForestSpawn { get; set; }
    }
}