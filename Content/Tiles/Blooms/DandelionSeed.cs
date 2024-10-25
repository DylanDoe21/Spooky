using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Blooms
{
    public class DandelionSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
			Item.DefaultToPlaceableTile(ModContent.TileType<DandelionBloomPlant>());
			Item.width = 44;
			Item.height = 56;
			Item.noUseGraphic = true;
			Item.rare = ItemRarityID.Blue;
			Item.placeStyle = Main.rand.Next(0, 3);
        }

        public override bool? UseItem(Player player)
		{
			Item.placeStyle = Main.rand.Next(0, 3);
			return null;
		}
    }
}