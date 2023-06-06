using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace Spooky
{
    public class SpookyConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("$Mods.Spooky.Configs.HalloweenEnabled.Label")]
        [Tooltip("$Mods.Spooky.Configs.HalloweenEnabled.Tooltip")]
        [DefaultValue(true)]
        public bool HalloweenEnabled { get; set; }

        [Label("$Mods.Spooky.Configs.SpookyForestSpawn.Label")]
        [Tooltip("$Mods.Spooky.Configs.SpookyForestSpawn.Tooltip")]
        [DefaultValue(true)]
        public bool SpookyForestSpawn { get; set; }

        [Label("$Mods.Spooky.Configs.ScreenShakeEnabled.Label")]
        [Tooltip("$Mods.Spooky.Configs.ScreenShakeEnabled.Tooltip")]
        [DefaultValue(true)]
        public bool ScreenShakeEnabled { get; set; }
    }
}