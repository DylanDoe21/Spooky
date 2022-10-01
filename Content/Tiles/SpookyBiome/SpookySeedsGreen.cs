using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyBiome
{
    public class SpookySeedsGreen : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Green Spooky Seeds");
			Tooltip.SetDefault("Places grass on spooky dirt");
		}

		public override void SetDefaults()
		{
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 7;
			Item.useAnimation = 15;
			Item.useStyle = 1;
			Item.maxStack = 999;
		}

        public override bool? UseItem(Player player)
		{
			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
			if (tile.HasTile && tile.TileType == ModContent.TileType<SpookyDirt>())
			{
				Main.tile[Player.tileTargetX, Player.tileTargetY].TileType = (ushort)ModContent.TileType<SpookyGrassGreen>();
				player.inventory[player.selectedItem].stack--;
			}

			return true;
		}
    }
}