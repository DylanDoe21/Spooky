using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Tiles.Minibiomes.Jungle
{
    public class JungleSoilGrassSeeds : ModItem
	{
        public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 25;
		}

		public override void SetDefaults()
		{
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
		}

        public override bool? UseItem(Player player)
		{
			if (Main.myPlayer != player.whoAmI)
			{
				return false;
			}

			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);

			if (tile.HasTile && tile.TileType == ModContent.TileType<JungleSoil>() && ItemGlobal.WithinPlacementRange(player, Player.tileTargetX, Player.tileTargetY))
			{
				WorldGen.PlaceTile(Player.tileTargetX, Player.tileTargetY, ModContent.TileType<JungleSoilGrass>(), forced: true);
				player.inventory[player.selectedItem].stack--;

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendTileSquare(player.whoAmI, Player.tileTargetX, Player.tileTargetY);
				}
			}

			return null;
		}
    }
}