using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Light;
using Microsoft.Xna.Framework;
using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;

using Spooky.Core;
using Spooky.Content.Backgrounds.SpiderCave;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class SpiderCaveBiome : ModBiome
    {
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<SpiderCaveUndergroundBG>();

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpiderCave");
       
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override int BiomeTorchItemType => ItemID.JungleTorch;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<SpookyWaterStyle>();

        public override void Load()
        {
            IL_Main.DrawBlack += ChangeBlackThreshold;
            On_Main.DrawBlack += ForceDrawBlack;
            On_TileLightScanner.GetTileLight += SpiderCaveLighting;
        }

        private static float mult = 0.8f;

        private void SpiderCaveLighting(On_TileLightScanner.orig_GetTileLight orig, TileLightScanner self, int x, int y, out Vector3 outputColor)
        {
            orig(self, x, y, out outputColor);

            Tile tile = Framing.GetTileSafely(x, y);

            if (!WorldGen.InWorld(x, y) || !Main.BackgroundEnabled)
            {
                return;
            }

            if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()))
            {
                bool tileBlock = tile.HasTile && Main.tileBlockLight[tile.TileType] && !(tile.Slope != SlopeType.Solid || tile.IsHalfBlock || !WorldGen.SolidTile(x, y));
                bool wallBlock = Main.wallLight[tile.WallType];
                bool lit = Main.tileLighted[tile.TileType];

                if (!tileBlock && wallBlock && !lit)
                {
                    int yOff = y - (int)(Main.LocalPlayer.position.Y / 16);

                    if (mult > 1)
                    {
                        mult = 1;
                    }

                    float progress = 0.5f + yOff / (int)(Main.LocalPlayer.position.Y / 16) * 0.7f;
                    progress = MathHelper.Max(0.5f, progress);

                    outputColor.X = (0.25f + (yOff > 70 ? ((yOff - 70) * 0.005f) : 0)) * progress * mult;
                    outputColor.Y = (0.25f + (yOff > 70 ? ((yOff - 70) * 0.0005f) : 0)) * progress * mult;
                    outputColor.Z = (0.25f - (yOff > 70 ? ((yOff - 70) * 0.005f) : 0)) * progress * mult;

                    if (yOff > 90 && mult < 1)
                    {
                        outputColor.X += (1 - mult * mult) * progress * 0.6f;
                        outputColor.Y += (1 - mult * mult) * progress * 0.2f;
                    }
                }
            }
        }

        private void ForceDrawBlack(On_Main.orig_DrawBlack orig, Main self, bool force)
        {
            if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()) && Main.BackgroundEnabled)
            {
                orig(self, true);
            }
            else
            {
                orig(self, force);
            }
        }

        private float NewThreshold(float orig)
        {
            if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()) && Main.BackgroundEnabled)
            {
                return 0.1f;
            }
            else
            {
                return orig;
            }
        }

        private void ChangeBlackThreshold(ILContext il)
        {
            if (Main.BackgroundEnabled)
            {
                var c = new ILCursor(il);
                c.TryGotoNext(n => n.MatchLdloc(6), n => n.MatchStloc(13)); //beginning of the loop, local 11 is a looping variable
                c.Index++; //this is kinda goofy since I dont think you could actually ever write c# to compile to the resulting IL from emitting here.
                c.Emit(OpCodes.Ldloc, 3); //pass the original value so we can set that instead if we dont want to change the threshold
                c.EmitDelegate<Func<float, float>>(NewThreshold); //check if were in the biome to set, else set the original value
                c.Emit(OpCodes.Stloc, 3); //num2 in vanilla, controls minimum threshold to turn a tile black
            }
        }

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/SpiderCaveBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            int PlayerY = (int)player.Center.Y / 16;
            int BiomeDepthThreshold = (Main.maxTilesY / 2) - (Main.maxTilesY / 18);

            bool BiomeCondition = ModContent.GetInstance<TileCount>().spiderCaveTiles >= 1500;
            bool UndergroundCondition = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

            return BiomeCondition && UndergroundCondition;
        }
    }
}