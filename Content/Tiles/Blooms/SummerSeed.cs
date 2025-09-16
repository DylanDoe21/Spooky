using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items;

namespace Spooky.Content.Tiles.Blooms
{
    public class SummerSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SummerBloomPlant>());
			Item.width = 34;
			Item.height = 42;
			Item.noUseGraphic = true;
			Item.rare = ModContent.RarityType<BloomHMRarity>();
			Item.placeStyle = Main.rand.Next(0, 4);
		}

		public override bool? UseItem(Player player)
		{
			Item.placeStyle = Main.rand.Next(0, 4);
			return null;
		}
    }
}