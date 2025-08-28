using Terraria.ModLoader.Config;
using System.ComponentModel;

namespace Spooky
{
    [BackgroundColor(80, 12, 0, 150)]
    public class SpookyConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [BackgroundColor(250, 175, 0, 125)]
        [DefaultValue(true)]
        public bool OldHunterDramaticLight { get; set; }

        [BackgroundColor(250, 175, 0, 125)]
        [DefaultValue(true)]
        public bool DraggableUI { get; set; }

        [BackgroundColor(250, 175, 0, 125)]
		[Range(0f, 5f)]
        [Increment(0.25f)]
        [DefaultValue(1f)]
        [Slider]
		public float ScreenShakeIntensity;

		[BackgroundColor(250, 175, 0, 125)]
		[DefaultValue(DialogueSpeedEnum.Normal)]
		[DrawTicks]
		public DialogueSpeedEnum DialogueSpeed;
	}

	[BackgroundColor(80, 12, 0, 150)]
	public class SpookyWorldgenConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		[BackgroundColor(250, 175, 0, 125)]
		[DefaultValue(true)]
		public bool SpookyForestSpawn { get; set; }

		[BackgroundColor(250, 175, 0, 125)]
		[DefaultValue(EyeValleyPosEnum.JungleSide)]
		[DrawTicks]
		public EyeValleyPosEnum EyeValleyWorldSide;

		[BackgroundColor(250, 175, 0, 125)]
		[DefaultValue(ZombieBiomePosEnum.DungeonSide)]
		[DrawTicks]
		public ZombieBiomePosEnum ZombieBiomeWorldside;
	}

	[BackgroundColor(80, 12, 0, 150)]
    public class SpookyServerConfig : ModConfig
	{
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [BackgroundColor(250, 175, 0, 125)]
        [DefaultValue(true)]
        public bool HalloweenEnabled { get; set; }
    }

	public enum EyeValleyPosEnum
	{
		Random,
		JungleSide,
		DungeonSide
	}
	public enum ZombieBiomePosEnum
	{
		Random,
		JungleSide,
		DungeonSide
	}
	public enum DialogueSpeedEnum
	{
		Normal,
		Fast,
		Instant
	}
}