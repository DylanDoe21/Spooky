using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.Relic
{
	public class DaffodilRelicItem : ModItem
	{
		public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

		public override void SetDefaults() 
        {
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.master = true;
			Item.width = 30;
			Item.height = 44;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Master;
			Item.value = Item.buyPrice(gold: 1);
			Item.createTile = ModContent.TileType<DaffodilRelic>();
		}
	}
}