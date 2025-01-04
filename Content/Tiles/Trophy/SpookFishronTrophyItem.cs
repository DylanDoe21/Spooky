using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Trophy
{
    public class SpookFishronTrophyItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SpookFishronTrophy>());
            Item.width = 16;
			Item.height = 16;
            Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
        }
    }
}