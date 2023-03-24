using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.Cemetery.Armor
{
	[LegacyName("SpookyLegs")]
	[AutoloadEquip(EquipType.Legs)]
	public class SpiritHorsemanLegs : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Spirit Horseman's Greaves");
			Tooltip.SetDefault("5% increased movement speed");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 22;
			Item.height = 18;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.moveSpeed += 0.05f;
		}
	}
}