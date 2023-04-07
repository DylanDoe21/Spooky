using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace Spooky
{
    public class SpookyConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("$Mods.Spooky.Config.HalloweenEnabled.Label")]
        [Tooltip("$Mods.Spooky.Config.HalloweenEnabled.Tooltip")]
        [DefaultValue(true)]
        public bool HalloweenEnabled { get; set; }

        [Label("$Mods.Spooky.Config.SpookyForestAtSpawn.Label")]
        [Tooltip("$Mods.Spooky.Config.SpookyForestAtSpawn.Tooltip")]
        [DefaultValue(true)]
        public bool SpookyForestSpawn { get; set; }

        [Label("$Mods.Spooky.Config.ScreenShakingEffect.Label")]
        [Tooltip("$Mods.Spooky.Config.ScreenShakingEffect.Tooltip")]
        [DefaultValue(true)]
        public bool ScreenShakeEnabled { get; set; }
    }
}