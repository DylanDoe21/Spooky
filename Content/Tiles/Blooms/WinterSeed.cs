using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.Blooms
{
    public class WinterSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 48;
			Item.height = 40;
			Item.useTime = 15;
			Item.useAnimation = 15;
            Item.rare = ItemRarityID.Blue;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
			//Item.createTile = ModContent.TileType<WinterSeedPlant>();
        }
    }
}