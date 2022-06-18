using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class CandleItem : ModItem
	{
		public override void SetStaticDefaults() 
		{
            DisplayName.SetDefault("Small Candle");
			Tooltip.SetDefault("It flickers eerily");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }

		public override void SetDefaults() 
        {
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 14;
			Item.height = 18;
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useStyle = 1;
			Item.maxStack = 99;
			Item.createTile = ModContent.TileType<Candle>();
		}
	}
}