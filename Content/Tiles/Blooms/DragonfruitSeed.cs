using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Blooms
{
    public class DragonfruitSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<DragonfruitBloomPlant>());
			Item.width = 34;
			Item.height = 40;
			Item.noUseGraphic = true;
			Item.rare = ItemRarityID.Blue;
		}
	}
}