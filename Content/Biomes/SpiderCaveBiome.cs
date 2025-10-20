using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Light;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using ReLogic.Content;
using Spooky.Core;
using Spooky.Content.Backgrounds.SpiderCave;
using Spooky.Content.Tiles.SpiderCave.Furniture;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class SpiderCaveBiome : ModBiome
    {
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<SpiderCaveUndergroundBG>();

		public static Asset<Effect> SporeMist;
		public static Asset<Texture2D> SwirlyNoise, SwirlyNoiseInv, StarNoise;

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

			SporeMist = ModContent.Request<Effect>("Spooky/Effects/MoldMist");
			
			// Ebonfly: pls load textures on mod load instead of every time they're used :pray:
			SwirlyNoise = ModContent.Request<Texture2D>("Spooky/ShaderAssets/swirlyNoise");
			SwirlyNoiseInv = ModContent.Request<Texture2D>("Spooky/ShaderAssets/swirlyNoiseInverted");
			StarNoise = ModContent.Request<Texture2D>("Spooky/ShaderAssets/starNoise");
		}

		float FogAlpha = 0f;

		private void SporeFogDraw(On_Main.orig_DrawInfernoRings orig, Main self)
		{
			orig(self);
			
			Flags.SporeEventHappening = true;
			Flags.SporeEventTimeLeft = 666;
			Flags.SporeFogColor = 0;
			Flags.SporeFogIntensity = 1f;

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

			if (FogAlpha > 0f && Flags.SporeFogIntensity > 0f)
			{
				Effect sporeEffect = SporeMist.Value;
				
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, sporeEffect, Main.GameViewMatrix.TransformationMatrix);

				Main.graphics.GraphicsDevice.Textures[1] = SwirlyNoise.Value;
				Main.graphics.GraphicsDevice.Textures[2] = SwirlyNoiseInv.Value;
				Main.graphics.GraphicsDevice.Textures[3] = StarNoise.Value;

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
				
				// Ebonfly: the math here kinda reeks so don't make the offset too drastic 
                void DrawFog(Texture2D tex, Vector2 offset)
                {
                    for (int i = -1; i < 2; i += 2)
                    for (int j = -1; j < 2; j += 2)
                    {
                        Vector2 origPos = Main.LocalPlayer.Center - new Vector2(Main.screenWidth * i,  Main.screenHeight * j) * 0.5f;

                        Vector2 pos = new Vector2(MathF.Floor(origPos.X / (Main.screenWidth)), MathF.Floor(origPos.Y / (Main.screenHeight))) 
                            * new Vector2(Main.screenWidth, Main.screenHeight) - Main.screenPosition + offset;

                        Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, Main.screenWidth, Main.screenHeight);
                        if (!rect.Intersects(new Rectangle(0,0, Main.screenWidth, Main.screenHeight)))
                            continue;
                        
                        Main.spriteBatch.Draw(tex, rect, Color.White);
                    }
                }

                //first, draw the top layer of mostly visible fog
                sporeEffect.Parameters["uOpacityTotal"].SetValue(1.5f*(0.6f * Flags.SporeFogIntensity) * FogAlpha);
                sporeEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 40);
                sporeEffect.Parameters["uColor"].SetValue(color1.ToVector4());
                sporeEffect.Parameters["uExponent"].SetValue(3f);
                for (int i = -1; i < 2; i+=2)
                    DrawFog(SwirlyNoise.Value, new Vector2(Main.screenWidth , Main.screenHeight*1.5f)* 0.125f*i);

                //draw second layer of slightly more transparent fog
                sporeEffect.Parameters["uOpacityTotal"].SetValue(1.5f*(0.45f * Flags.SporeFogIntensity) * FogAlpha);
                sporeEffect.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly / 50);
                sporeEffect.Parameters["uColor"].SetValue(color1.ToVector4());
                DrawFog(SwirlyNoiseInv.Value, Vector2.Zero);

                //draw star texture to look like spores are in the air
                sporeEffect.Parameters["uOpacityTotal"].SetValue(2*(1f * Flags.SporeFogIntensity) * FogAlpha);
                sporeEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 120);
                sporeEffect.Parameters["uColor"].SetValue(color2.ToVector4());
                sporeEffect.Parameters["uExponent"].SetValue(2f);
                DrawFog(StarNoise.Value, Vector2.Zero);
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