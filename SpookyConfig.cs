using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace Spooky
{
    public class SpookyConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("Halloween Season")]
        [Tooltip("The spooky mod makes terraria's halloween seasonal event always active"
        + "\nTurn off to disable the halloween seasonal event being active"
        + "\nThe halloween seasonal event will still occur during October 10th-31st regardless"
        + "\nYou will need to leave and re-enter your world for changes to this option to take effect")]
        [DefaultValue(true)]
        public bool HalloweenEnabled { get; set; }

        [Label("Spooky Forest at Spawn")]
        [Tooltip("Changes the position where the spooky forest is placed during world generation"
        + "\nKeeping this option on will make it placed at spawn, which is the default option"
        + "\nTurning this off will cause the spooky forest to generate away from spawn"
        + "\nWARNING: Playing on small worlds with this option off may destroy parts of other biomes!")]
        [DefaultValue(true)]
        public bool SpookyForestSpawn { get; set; }

        [Label("Screen Shaking Effect")]
        [Tooltip("Turn off to disable the screen shaking effect caused by various things in the mod")]
        [DefaultValue(true)]
        public bool ScreenShakeEnabled { get; set; }
    }
}