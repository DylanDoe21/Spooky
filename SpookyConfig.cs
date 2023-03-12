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
        + "\nTurning this config off will not disable halloween if it is meant to occur in vanilla"
        + "\n(For example, when you get one day of halloween after the pumpkin moon, or during october)")]
        [DefaultValue(true)]
        public bool HalloweenEnabled { get; set; }

        [Label("Spooky Forest at Spawn")]
        [Tooltip("Changes the position where the spooky forest is placed during world generation"
        + "\nKeeping this option on will make it placed at spawn, which is the default option"
        + "\nTurning this off will cause the spooky forest to generate away from spawn"
        + "\nThis option is applied when making new worlds, and will have no effect on existing worlds"
        + "\nWARNING: Playing on small worlds with this option off may destroy parts of other biomes!")]
        [DefaultValue(true)]
        public bool SpookyForestSpawn { get; set; }

        [Label("Screen Shaking Effect")]
        [Tooltip("Turn off to disable the screen shaking effect caused by various things in the mod")]
        [DefaultValue(true)]
        public bool ScreenShakeEnabled { get; set; }
    }
}