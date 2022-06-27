using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class OldWoodLegs : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Old Wood Leggings");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 1;
			Item.width = 22;
			Item.height = 16;
			Item.value = Item.buyPrice(silver: 75);
			Item.rare = ItemRarityID.Blue;
		}
	}
}
