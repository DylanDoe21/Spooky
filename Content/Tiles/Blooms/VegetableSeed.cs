using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Blooms
{
    public class VegetableSeed : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<VegetableBloomPlant>());
			Item.width = 46;
			Item.height = 44;
			Item.noUseGraphic = true;
			Item.rare = ItemRarityID.Blue;
			Item.placeStyle = Main.rand.Next(0, 4);
		}

		public override bool? UseItem(Player player)
		{
			Item.placeStyle = Main.rand.Next(0, 4);
			return null;
		}
    }
}