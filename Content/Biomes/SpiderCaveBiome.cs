using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Light;
using Microsoft.Xna.Framework;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;

using Spooky.Core;
using Spooky.Content.Backgrounds.SpiderCave;
using Spooky.Content.Tiles.SpiderCave.Furniture;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class SpiderCaveBiome : ModBiome
    {
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<SpiderCaveUndergroundBG>();

		//set the music to be consistent with vanilla's music priorities
		public override int Music
		{
			get
			{
				int music = Main.curMusic;

				//play town music if enough town npcs exist
				if (Main.LocalPlayer.townNPCs > 2f)
				{
					if (Main.dayTime)
					{
						music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyTownDay");
					}
					else
					{
						music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyTownNight");
					}
				}
				//play normal music
				else
				{
					music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpiderCave");
				}

				return music;
			}
		}

		public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<SpookyWaterStyle>();

        public override int BiomeTorchItemType => ModContent.ItemType<SpiderBiomeTorchItem>();

		public override void Load()
        {
	        if (ModLoader.TryGetMod("NotQuiteNitrate", out Mod nqn))
	        {
		        nqn.Call("ModifyDrawBlackThreshold", (Func<float, float>)NewThreshold);
	        }
			
            IL_Main.DrawBlack += ChangeBlackThreshold;
            On_Main.DrawBlack += ForceDrawBlack;
            On_TileLightScanner.GetTileLight += SpiderCaveLighting;
		}

		private static float mult = 0.8f;
		private static float fade = 0f;

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

                    outputColor.X = (0.35f + (yOff > 70 ? ((yOff - 70) * 0.005f) : 0)) * progress * mult;
                    outputColor.Y = (0.35f + (yOff > 70 ? ((yOff - 70) * 0.0005f) : 0)) * progress * mult;
                    outputColor.Z = (0.35f - (yOff > 70 ? ((yOff - 70) * 0.005f) : 0)) * progress * mult;

                    if (yOff > 90 && mult < 1)
                    {
                        outputColor.X += (1 - mult * mult) * progress * 0.6f;
                        outputColor.Y += (1 - mult * mult) * progress * 0.2f;
                    }
                }

				//during the spider war, change every tile that produces light to only produce pure white light
				if (SpiderWarWorld.SpiderWarActive) 
				{
					if (!tileBlock && lit)
					{
						int yOff = y - (int)(Main.LocalPlayer.position.Y / 16);

						if (mult > 1)
						{
							mult = 1;
						}

						float progress = 0.5f + yOff / (int)(Main.LocalPlayer.position.Y / 16) * 0.7f;
						progress = MathHelper.Max(0.5f, progress);

						float WhiteColor = ((Lighting.GetColor(x, y).R + Lighting.GetColor(x, y).G + Lighting.GetColor(x, y).B) / 3);

						fade += 0.01f;
						float timeRatio = Utils.GetLerpValue(0f, 100f, fade);

						outputColor = Color.Lerp(new Color(outputColor.X, outputColor.Y, outputColor.Z),
						new Color(WhiteColor, WhiteColor, WhiteColor),
						Utils.GetLerpValue(0f, 1f, timeRatio)).ToVector3() * progress * mult;
					}
				}
				else
				{
					fade = 0f;
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

            return orig;
        }

        private void ChangeBlackThreshold(ILContext il)
		{
			ILCursor val = new ILCursor(il);
            val.TryGotoNext(
            (Instruction n) => ILPatternMatchingExt.MatchLdloc(n, 8),
            (Instruction n) => ILPatternMatchingExt.MatchStloc(n, 12));
            int index = val.Index;
            val.Index = index + 1;
            val.Emit(OpCodes.Ldloc, 3);
            val.EmitDelegate(NewThreshold);
            val.Emit(OpCodes.Stloc, 3);
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

            bool BiomeCondition = ModContent.GetInstance<TileCount>().spiderCaveTiles >= 1200;
            bool UndergroundCondition = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

            return BiomeCondition && UndergroundCondition;
        }
    }
}