using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Pylon;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Core
{
    public class TileGlobal : GlobalTile
    {
        public override void Load()
		{
            On_Player.CheckForGoodTeleportationSpot += DontTeleport;
        }

        public bool IsProtected(int x, int y)
        {
            if (!Main.gameMenu || Main.dedServ) //shouldnt trigger while generating the world from the menu
            {
                Tile tile = Framing.GetTileSafely(x, y);

                if (tile.WallType == ModContent.WallType<CatacombBrickWall1>())
                    return true;

                if (tile.WallType == ModContent.WallType<CatacombBrickWall2>())
                    return true;
            }

            return false;
        }

        private Vector2 DontTeleport(On_Player.orig_CheckForGoodTeleportationSpot orig, Player self, ref bool canSpawn, int teleportStartX, int teleportRangeX, int teleportStartY, int teleportRangeY, Player.RandomTeleportationAttemptSettings settings)
        {
            Vector2 result = orig(self, ref canSpawn, teleportStartX, teleportRangeX, teleportStartY, teleportRangeY, settings);

            //If invalid spot, recurse untill a valid one is found
            if (IsProtected((int)result.X, (int)result.Y))
            {
                settings.attemptsBeforeGivingUp--;
                result = self.CheckForGoodTeleportationSpot(ref canSpawn, teleportStartX, teleportRangeX, teleportStartY, teleportRangeY, settings);
            }
            return result;
        }

        public override bool Slope(int i, int j, int type)
        {
            Tile tileAbove = Main.tile[i, j - 1];

            if (tileAbove.TileType == ModContent.TileType<Cauldron>() || 
            tileAbove.TileType == ModContent.TileType<NoseShrine>() ||
            tileAbove.TileType == ModContent.TileType<OrroboroEgg>() ||
            tileAbove.TileType == ModContent.TileType<CemeteryPylon>() ||
            tileAbove.TileType == ModContent.TileType<SpookyBiomePylon>() ||
            tileAbove.TileType == ModContent.TileType<SpookyHellPylon>())
            {
                return false;
            }

            return true;
        }
    }
}