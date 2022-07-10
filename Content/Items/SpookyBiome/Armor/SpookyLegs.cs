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
			Tooltip.SetDefault("5% increased movement speed");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 2;
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