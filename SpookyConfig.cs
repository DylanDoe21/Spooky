using Terraria.ModLoader.Config;
using System.ComponentModel;

using Spooky.Core;
using Spooky.Content.UI;

namespace Spooky
{
    public class SpookyConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [BackgroundColor(125, 62, 0, 125)]
        [DefaultValue(true)]
        public bool HalloweenEnabled { get; set; }

        [BackgroundColor(0, 125, 0, 125)]
        [DefaultValue(true)]
        public bool SpookyForestSpawn { get; set; }

        [BackgroundColor(120, 85, 60, 125)]
        [DefaultValue(true)]
        public bool OldHunterDramaticLight { get; set; }

        [BackgroundColor(60, 0, 125, 125)]
		[Range(0f, 5f)]
        [Increment(0.25f)]
        [DefaultValue(1f)]
        [Slider]
		public float ScreenShakeIntensity;
    }
}