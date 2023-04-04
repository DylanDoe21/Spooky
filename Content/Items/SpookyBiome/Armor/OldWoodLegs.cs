using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class OldWoodLegs : ModItem
	{
		public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Old Wood Greaves");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 1;
			Item.width = 22;
			Item.height = 18;
			Item.rare = ItemRarityID.White;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 25)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}