using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.Blooms
{
    public class SpringSeed : ModItem
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
            Item.noUseGraphic = true;
            Item.width = 52;
			Item.height = 52;
			Item.useTime = 15;
			Item.useAnimation = 15;
            Item.rare = ItemRarityID.Blue;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.placeStyle = Main.rand.Next(0, 4);
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<SpringBloomPlant>();
        }

        public override bool? UseItem(Player player)
		{
			Item.placeStyle = Main.rand.Next(0, 4);
			return null;
		}
    }
}