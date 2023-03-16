using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class OldWoodBody : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Old Wood Breastplate");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 2;
			Item.width = 34;
			Item.height = 20;
			Item.rare = ItemRarityID.White;
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 30)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}