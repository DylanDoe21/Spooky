using Terraria.ModLoader.Config;
using System.ComponentModel;

using Spooky.Core;

namespace Spooky
{
    [BackgroundColor(35, 10, 85)]
    public class SpookyConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("$Mods.Spooky.Configs.HalloweenEnabled.Label")]
        [Tooltip("$Mods.Spooky.Configs.HalloweenEnabled.Tooltip")]
        [BackgroundColor(242, 115, 0)]
        [DefaultValue(true)]
        public bool HalloweenEnabled { get; set; }

        [Label("$Mods.Spooky.Configs.SpookyForestSpawn.Label")]
        [Tooltip("$Mods.Spooky.Configs.SpookyForestSpawn.Tooltip")]
        [BackgroundColor(0, 195, 0)]
        [DefaultValue(true)]
        public bool SpookyForestSpawn { get; set; }

        [BackgroundColor(130, 0, 195)]
		[Range(0f, 2f)]
        [Increment(0.25f)]
        [DefaultValue(1f)]
        [Slider]
		public float ScreenShakeIntensity;
    }
}