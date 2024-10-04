using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Relic
{
	public class EyeWizardRelicItem : ModItem
	{
		public override void SetDefaults() 
        {
			Item.DefaultToPlaceableTile(ModContent.TileType<EyeWizardRelic>());
			Item.master = true;
			Item.width = 16;
			Item.height = 16;
			Item.rare = ItemRarityID.Master;
			Item.value = Item.buyPrice(gold: 1);
		}
	}
}