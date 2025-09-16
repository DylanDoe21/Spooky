using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items;

namespace Spooky.Content.Tiles.Blooms
{
    public class FossilSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<FossilBloomPlant>());
			Item.width = 36;
			Item.height = 40;
			Item.noUseGraphic = true;
			Item.rare = ModContent.RarityType<BloomPreHMRarity>();
			Item.placeStyle = Main.rand.Next(0, 4);
		}

		public override bool? UseItem(Player player)
		{
			Item.placeStyle = Main.rand.Next(0, 4);
			return null;
		}
    }
}