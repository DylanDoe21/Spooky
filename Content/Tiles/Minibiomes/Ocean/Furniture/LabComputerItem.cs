using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Furniture
{
    public class LabComputerUnsafeItem : ModItem
    {
        public override string Texture => "Spooky/Content/Tiles/Minibiomes/Ocean/Furniture/LabComputerItem";

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabComputer>());
            Item.width = 16;
			Item.height = 16;
        }
    }

    public class LabComputerItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabComputerSafe>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<LabComputerRustedItem>(), 1)
            .AddIngredient(ItemID.Glass, 5)
            .AddIngredient(ItemID.Wire, 10)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}