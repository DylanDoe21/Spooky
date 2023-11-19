using Terraria.ModLoader.Config;
using System.ComponentModel;

using Spooky.Core;

namespace Spooky
{
    [BackgroundColor(54, 48, 57)]
    public class SpookyConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Label("$Mods.Spooky.Configs.HalloweenEnabled.Label")]
        [Tooltip("$Mods.Spooky.Configs.HalloweenEnabled.Tooltip")]
        [BackgroundColor(251, 111, 45)]
        [DefaultValue(true)]
        public bool HalloweenEnabled { get; set; }

        [Label("$Mods.Spooky.Configs.SpookyForestSpawn.Label")]
        [Tooltip("$Mods.Spooky.Configs.SpookyForestSpawn.Tooltip")]
        [BackgroundColor(137, 224, 49)]
        [DefaultValue(true)]
        public bool SpookyForestSpawn { get; set; }

        [BackgroundColor(255, 194, 65)]
		[Range(0f, 2f)]
        [Increment(0.25f)]
        [DefaultValue(1f)]
        [Slider]
		public float ScreenShakeIntensity;
    }
}