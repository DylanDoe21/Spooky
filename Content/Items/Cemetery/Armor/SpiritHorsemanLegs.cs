using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.Cemetery.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class SpiritHorsemanLegs : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Spirit Horseman's Greaves");
			// Tooltip.SetDefault("12% increased movement speed");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 22;
			Item.height = 18;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.moveSpeed += 0.12f;
		}
	}
}