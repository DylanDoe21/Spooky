using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class SporeBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 18;
			Item.width = 34;
			Item.height = 20;
			Item.rare = ItemRarityID.LightRed;
		}

		public override void UpdateEquip(Player player) 
		{
        }

		/*
		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RootWoodItem>(), 30)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
		*/
	}
}