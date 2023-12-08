using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Microsoft.Xna.Framework;
using System.Reflection;

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

        public override void SetStaticDefaults()
        {
            //do not run any lighting essentials checking if the mod is not enabled
            if (LightingEssentialsActive())
            {
                //set tileLighted to true for all ambient grasses in spooky mod
                //using reflection, get the lighting essentials config and make sure the "Light Environment" option is turned on
                var LightingEssentialsConfig = ModContent.Find<ModConfig>("LightingEssentials/Config");
                bool LightEnvironmentOn = (bool)LightingEssentialsConfig.GetType().GetField("LightEnvironment", BindingFlags.Public | BindingFlags.Instance).GetValue(LightingEssentialsConfig);

                if (LightEnvironmentOn)
                {
                    Main.tileLighted[ModContent.TileType<SpookyWeedsOrange>()] = true;
                    Main.tileLighted[ModContent.TileType<SpookyWeedsGreen>()] = true;
                    Main.tileLighted[ModContent.TileType<CemeteryWeeds>()] = true;
                    Main.tileLighted[ModContent.TileType<CatacombWeeds>()] = true;
                    Main.tileLighted[ModContent.TileType<SpiderCaveWeeds>()] = true;
                }
            }
        }

        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            //do not run any lighting essentials checking if the mod is not enabled
            if (LightingEssentialsActive())
            {
                //add the actual lighting for all the ambient grasses in spooky mod
                //like above, use reflection get the lighting essentials config and make sure the "Light Environment" option is turned on
                var LightingEssentialsConfig = ModContent.Find<ModConfig>("LightingEssentials/Config");
                bool LightEnvironmentOn = (bool)LightingEssentialsConfig.GetType().GetField("LightEnvironment", BindingFlags.Public | BindingFlags.Instance).GetValue(LightingEssentialsConfig);

                if (LightEnvironmentOn)
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