using Terraria.ModLoader.Config;
using System.ComponentModel;

using Spooky.Core;

namespace Spooky
{
    [BackgroundColor(100, 32, 0, 175)]
    public class SpookyConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [BackgroundColor(250, 175, 0, 125)]
        [DefaultValue(true)]
        public bool SpookyForestSpawn { get; set; }

        [BackgroundColor(250, 175, 0, 125)]
        [DefaultValue(true)]
        public bool OldHunterDramaticLight { get; set; }

        [BackgroundColor(250, 175, 0, 125)]
        [DefaultValue(false)]
        public bool CanDragBloomBuffUI { get; set; }

        [BackgroundColor(250, 175, 0, 125)]
		[Range(0f, 5f)]
        [Increment(0.25f)]
        [DefaultValue(1f)]
        [Slider]
		public float ScreenShakeIntensity;
    }

    [BackgroundColor(100, 32, 0, 175)]
    public class SpookyServerConfig : ModConfig
	{
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [BackgroundColor(250, 175, 0, 125)]
        [DefaultValue(true)]
        public bool HalloweenEnabled { get; set; }
    }
}