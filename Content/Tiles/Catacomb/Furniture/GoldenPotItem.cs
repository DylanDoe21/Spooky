using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
    public class GoldenPotItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 30;
			Item.height = 34;
			Item.useTime = 10;
			Item.useAnimation = 14;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<GoldenPot>();
        }
    }
}