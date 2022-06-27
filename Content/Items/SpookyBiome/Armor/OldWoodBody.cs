using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class OldWoodBody : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Old Wood Chestplate");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 2;
			Item.width = 36;
			Item.height = 20;
			Item.value = Item.buyPrice(silver: 75);
			Item.rare = ItemRarityID.Blue;
		}
	}
}
