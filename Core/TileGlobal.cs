using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Pylon;
using Spooky.Content.Tiles.Catacomb.Ambient;
using Spooky.Content.Tiles.Cemetery.Ambient;
using Spooky.Content.Tiles.SpiderCave.Ambient;
using Spooky.Content.Tiles.SpookyBiome.Ambient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Core
{
    public class TileGlobal : GlobalTile
    {
        public static bool LightingEssentialsActive() => ModLoader.TryGetMod("LightingEssentials", out _);

        public override void Load()
        {
            On_Player.CheckForGoodTeleportationSpot += DontAllowTeleportation;
        }

        public override void SetStaticDefaults()
        {
            //set lighting to true for all the ambient grasses if lighting essentials is enabled
            if (LightingEssentialsActive())
            {
                Main.tileLighted[ModContent.TileType<SpookyWeedsOrange>()] = true;
                Main.tileLighted[ModContent.TileType<SpookyWeedsGreen>()] = true;
                Main.tileLighted[ModContent.TileType<CemeteryWeeds>()] = true;
                Main.tileLighted[ModContent.TileType<CatacombWeeds>()] = true;
                Main.tileLighted[ModContent.TileType<SpiderCaveWeeds>()] = true;
            }
        }

        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            //add lighting to ambient grasses if lighting essentials is enabled
            if (LightingEssentialsActive())
            {
                if (Main.tile[i, j].TileType == ModContent.TileType<SpookyWeedsOrange>())
                {
                    r = 175f / 450f;
                    g = 102f / 450f;
                    b = 36f / 450f;
                }

                if (Main.tile[i, j].TileType == ModContent.TileType<SpookyWeedsGreen>())
                {
                    r = 78f / 450f;
                    g = 120f / 450f;
                    b = 48f / 450f;
                }

                if (Main.tile[i, j].TileType == ModContent.TileType<CemeteryWeeds>())
                {
                    r = 38f / 420f;
                    g = 77f / 420f;
                    b = 53f / 420f;
                }

                if (Main.tile[i, j].TileType == ModContent.TileType<CatacombWeeds>())
                {
                    r = 56f / 450f;
                    g = 109f / 450f;
                    b = 62f / 450f;
                }

                if (Main.tile[i, j].TileType == ModContent.TileType<SpiderCaveWeeds>())
                {
                    r = 120f / 450f;
                    g = 100f / 450f;
                    b = 24f / 450f;
                }
            }
        }

        public override bool Slope(int i, int j, int type)
        {
            Tile tileAbove = Main.tile[i, j - 1];

            //dont allow sloping under specific spooky mod tiles
            if (tileAbove.TileType == ModContent.TileType<Cauldron>() ||
            tileAbove.TileType == ModContent.TileType<NoseShrine>() ||
            tileAbove.TileType == ModContent.TileType<OrroboroEgg>() ||
            tileAbove.TileType == ModContent.TileType<CemeteryPylon>() ||
            tileAbove.TileType == ModContent.TileType<SpookyBiomePylon>() ||
            tileAbove.TileType == ModContent.TileType<SpookyHellPylon>())
            {
                return false;
            }

            return base.Slope(i, j, type);
        }

        public bool IsProtected(int x, int y)
        {
            if (!Main.gameMenu || Main.dedServ)
            {
                Tile tile = Framing.GetTileSafely(x, y);

                if (tile.WallType == ModContent.WallType<CatacombBrickWall1>() || tile.WallType == ModContent.WallType<CatacombBrickWall2>() ||
                tile.WallType == ModContent.WallType<CatacombGrassWall1>() || tile.WallType == ModContent.WallType<CatacombGrassWall2>())
                {
                    return true;
                }
            }

            return false;
        }

        private Vector2 DontAllowTeleportation(On_Player.orig_CheckForGoodTeleportationSpot orig, Player self, ref bool canSpawn, int teleportStartX, int teleportRangeX, int teleportStartY, int teleportRangeY, Player.RandomTeleportationAttemptSettings settings)
        {
            Vector2 result = orig(self, ref canSpawn, teleportStartX, teleportRangeX, teleportStartY, teleportRangeY, settings);

            if (IsProtected((int)result.X, (int)result.Y))
            {
                settings.attemptsBeforeGivingUp--;
                result = self.CheckForGoodTeleportationSpot(ref canSpawn, teleportStartX, teleportRangeX, teleportStartY, teleportRangeY, settings);
            }
            
            return result;
        }
    }
}