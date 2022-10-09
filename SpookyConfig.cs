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
    }
}