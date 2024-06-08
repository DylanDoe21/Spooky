using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Relic
{
	public class OrroboroRelicItem : ModItem
	{
		public override void SetDefaults() 
        {
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.master = true;
			Item.width = 30;
			Item.height = 44;
			Item.useTime = 10;
			Item.useAnimation = 14;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.Master;
			Item.value = Item.buyPrice(gold: 1);
			Item.createTile = ModContent.TileType<OrroboroRelic>();
		}
	}
}