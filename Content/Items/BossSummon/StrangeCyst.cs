using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;

using Spooky.Content.Events;

namespace Spooky.Content.Items.BossSummon
{
	public class StrangeCyst : ModItem
	{
		public static readonly SoundStyle EventBeginSound = new("Spooky/Content/Sounds/SpookyHell/EggEventBegin", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Strange Cyst");
			Tooltip.SetDefault("Begins the egg incursion event\nCan be used in the valley of eyes");
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 32;
			Item.consumable = true;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.maxStack = 20;
		}

		public override bool? UseItem(Player player)
		{
			SoundEngine.PlaySound(SoundID.Roar, player.Center);

			EggEventWorld.EggEventActive = true;
			
			return true;
		}
	}
}