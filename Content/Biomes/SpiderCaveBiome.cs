using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Light;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

		public static Effect SporeMist;

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
			On_Main.DrawInfernoRings += SporeFogDraw;

			SporeMist = ModContent.Request<Effect>("Spooky/Effects/MoldMist", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		}

		float FogAlpha = 0f;

		private void SporeFogDraw(On_Main.orig_DrawInfernoRings orig, Main self)
		{
			orig(self);

			if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()) && Flags.SporeEventHappening)
			{
				if (FogAlpha < 1f)
				{
					FogAlpha += 0.01f;
				}
			}
			else
			{
				if (FogAlpha > 0f)
				{
					FogAlpha -= 0.01f;
				}
			}

			if (FogAlpha > 0f)
			{
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, SporeMist, Main.GameViewMatrix.TransformationMatrix);

				Main.graphics.GraphicsDevice.Textures[1] = ModContent.Request<Texture2D>("Spooky/ShaderAssets/swirlyNoise").Value;
				Main.graphics.GraphicsDevice.Textures[2] = ModContent.Request<Texture2D>("Spooky/ShaderAssets/swirlyNoiseInverted").Value;
				Main.graphics.GraphicsDevice.Textures[3] = ModContent.Request<Texture2D>("Spooky/ShaderAssets/starNoise").Value;

				Color color1 = new Color(0, 0, 0);
				Color color2 = new Color(0, 0, 0);

				//regular fog color
				switch (Flags.SporeFogColor)
				{
					case 0:
					{
						//blue fog
						color1 = new Color(47, 65, 233);
						break;
					}
					case 1:
					{
						//red fog
						color1 = new Color(255, 19, 0);
						break;
					}
					case 2:
					{
						//white fog
						color1 = new Color(219, 255, 245);
						break;
					}
					case 3:
					{
						//yellow fog
						color1 = new Color(255, 163, 0);
						break;
					}
					case 4:
					{
						//green fog
						color1 = new Color(94, 215, 35);
						break;
					}
					case 5:
					{
						//teal fog
						color1 = new Color(52, 239, 151);
						break;
					}
					case 6:
					{
						//purple fog
						color1 = new Color(162, 90, 250);
						break;
					}
				}
				//special color for the dots that draw behind the fog
				switch (Flags.SporeFogDotColor)
				{
					case 0:
					{
						//blue fog
						color2 = new Color(47, 65, 233);
						break;
					}
					case 1:
					{
						//red fog
						color2 = new Color(255, 19, 0);
						break;
					}
					case 2:
					{
						//white fog
						color2 = new Color(219, 255, 245);
						break;
					}
					case 3:
					{
						//yellow fog
						color2 = new Color(255, 163, 0);
						break;
					}
					case 4:
					{
						//green fog
						color2 = new Color(94, 215, 35);
						break;
					}
					case 5:
					{
						//teal fog
						color2 = new Color(52, 239, 151);
						break;
					}
					case 6:
					{
						//purple fog
						color2 = new Color(162, 90, 250);
						break;
					}
				}

				//first, draw the top layer of mostly visible fog
				SporeMist.Parameters["uOpacityTotal"].SetValue((0.6f * Flags.SporeFogIntensity) * FogAlpha);
				SporeMist.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 40);
				SporeMist.Parameters["uColor"].SetValue(color1.ToVector4());
				Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/ShaderAssets/swirlyNoise").Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

				//draw second layer of slightly more transparent fog
				SporeMist.Parameters["uOpacityTotal"].SetValue((0.45f * Flags.SporeFogIntensity) * FogAlpha);
				SporeMist.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly / 50);
				SporeMist.Parameters["uColor"].SetValue(color1.ToVector4());
				Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/ShaderAssets/swirlyNoiseInverted").Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);

				//draw star texture to look like spores are in the air
				SporeMist.Parameters["uOpacityTotal"].SetValue((1f * Flags.SporeFogIntensity) * FogAlpha);
				SporeMist.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 120);
				SporeMist.Parameters["uColor"].SetValue(color2.ToVector4());
				Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Spooky/ShaderAssets/starNoise").Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
			}
        }

		private static float mult = 0.5f;

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
            var c = new ILCursor(il);
            c.TryGotoNext(n => n.MatchLdloc(6), n => n.MatchStloc(13)); //beginning of the loop, local 11 is a looping variable
            c.Index++; //this is kinda goofy since I dont think you could actually ever write c# to compile to the resulting IL from emitting here.
            c.Emit(OpCodes.Ldloc, 3); //pass the original value so we can set that instead if we dont want to change the threshold
            c.EmitDelegate<Func<float, float>>(NewThreshold); //check if were in the biome to set, else set the original value
            c.Emit(OpCodes.Stloc, 3); //num2 in vanilla, controls minimum threshold to turn a tile black
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