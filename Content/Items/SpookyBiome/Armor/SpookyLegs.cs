using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class SpookyLegs : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Spooky Horseman's Pants");
			Tooltip.SetDefault("5% increased movement speed"
			+ "\nEnemies are more likely to target you");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.buyPrice(gold: 2);
			Item.rare = 1;
		}

		public override void UpdateEquip(Player player) 
		{
			player.aggro += 100;
			player.moveSpeed += 0.05f;
		}
	}
}
