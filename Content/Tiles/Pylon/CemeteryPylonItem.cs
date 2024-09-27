using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Pylon
{
	public class CemeteryPylonItem : ModItem
    {
		public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<CemeteryPylon>());
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 10;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 10);
		}
	}
}