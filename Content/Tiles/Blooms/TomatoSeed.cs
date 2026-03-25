using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items;

namespace Spooky.Content.Tiles.Blooms
{
    public class TomatoSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<TomatoBloomPlant>());
			Item.width = 34;
			Item.height = 40;
			Item.noUseGraphic = true;
			Item.rare = ModContent.RarityType<BloomPreHMRarity>();
		}
	}
}