using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;

using Spooky.Content.Tiles.Cemetery;

namespace Spooky.Content.Tiles.Catacomb
{
    public class CatacombGrassSeeds : ModItem
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
			Item.useTime = 10;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
		}

        public override bool? UseItem(Player player)
		{
			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
			
			if (tile.HasTile && tile.TileType == ModContent.TileType<CemeteryDirt>() && 
			player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.Simple))
			{
				SoundEngine.PlaySound(SoundID.Dig, player.Center);

				Main.tile[Player.tileTargetX, Player.tileTargetY].TileType = (ushort)ModContent.TileType<CatacombGrass>();

				return true;
			}

			return false;
		}
    }
}