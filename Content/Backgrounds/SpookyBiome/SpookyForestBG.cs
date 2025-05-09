using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;

using Spooky.Core;

namespace Spooky.Content.Backgrounds.SpookyBiome
{
	public class SpookyForestBG : ModSurfaceBackgroundStyle
	{
		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) => -1;

		public override int ChooseMiddleTexture() => -1;

		public override int ChooseFarTexture() => -1;

		public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
		{
			FieldInfo screenOffField = typeof(Main).GetField("screenOff", BindingFlags.Instance | BindingFlags.NonPublic);
			FieldInfo scAdjField = typeof(Main).GetField("scAdj", BindingFlags.Instance | BindingFlags.NonPublic);
			FieldInfo BGColorModifiedField = typeof(Main).GetField("ColorOfSurfaceBackgroundsModified", BindingFlags.Static | BindingFlags.NonPublic);

			float screenOff = (float)screenOffField.GetValue(Main.instance);
			float scAdj = (float)scAdjField.GetValue(Main.instance);
			Color BGColorModified = (Color)BGColorModifiedField.GetValue(null);

			int[] textureSlots = new int[]
			{
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBG4"),
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBG3"),
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBG2"),
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBG1"),
			};

			if (Flags.SpookyBackgroundAlt)
			{
				textureSlots = new int[] 
				{
					BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBGAlt3"),
					BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBGAlt2"),
					BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBGAlt1"),
					BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBGAlt1"),
				};
			}

			//actual background color using vanillas own background color with modifications
			Color BGActualColor = new Color(BGColorModified.R, BGColorModified.G, BGColorModified.B, BGColorModified.A);
			
			bool canBGDraw = false;

			//only draw the background if you arent on the menu or a secret worldseed where surface backgrounds arent supposed to be drawn
			if ((!Main.remixWorld || (Main.gameMenu && !WorldGen.remixWorldGen)) && (!WorldGen.remixWorldGen || !WorldGen.drunkWorldGen))
			{
				canBGDraw = true;
			}
			//dont draw while on the map
			if (Main.mapFullscreen)
			{
				canBGDraw = false;
			}

			int offset = 30;
			if (Main.gameMenu)
			{
				offset = 0;
			}
			if (WorldGen.drunkWorldGen)
			{
				offset = -180;
			}

			float surfacePosition = (float)Main.worldSurface;
			if (surfacePosition == 0f)
			{
				surfacePosition = 1f;
			}

			float screenPosition = Main.screenPosition.Y + (float)(Main.screenHeight / 2) - 600f;
			double backgroundTopMagicNumber = (0f - screenPosition + screenOff / 2f) / (surfacePosition * 16f);
			float bgGlobalScaleMultiplier = 2f;
			int pushBGTopHack;
			int offset2 = -180;

			int menuOffset = 0;
			if (Main.gameMenu)
			{
				menuOffset -= offset2;
			}

			pushBGTopHack = menuOffset;
			pushBGTopHack += offset;
			pushBGTopHack += offset2;

			if (canBGDraw)
			{
				//default background
				if (!Flags.SpookyBackgroundAlt)
				{
					//width and height (this is not usually a good idea but each of these 
					int Width = 1024;
					int Height = 700;

					//offsets for each individual background layer
					int CloseBGYOffset = -150;
					int MiddleCloseBGYOffset = -200;
					int MiddleFarBGYOffset = -150;
					int FarBGYOffset = -60;

					//back layer mountain
					var bgScale = 0.9f;
					var bgParallax = 0.35;
					var bgTopY = (int)(backgroundTopMagicNumber * 1800.0 + 1500.0) + (int)scAdj + pushBGTopHack;
					bgScale *= bgGlobalScaleMultiplier;
					var bgWidthScaled = (int)((float)Width * bgScale);
					SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1.2f / (float)bgParallax);
					var bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));
					
					if (Main.gameMenu)
					{
						bgTopY = 320 + pushBGTopHack;
					}

					var bgLoops = Main.screenWidth / bgWidthScaled + 2;
					if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
					{
						for (int i = -bgLoops; i < bgLoops; i++)
						{
							Main.spriteBatch.Draw(TextureAssets.Background[textureSlots[0]].Value, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + FarBGYOffset), new Rectangle(0, 0, Width, Height), BGActualColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
						}
					}

					//far layer infront of mountain
					bgScale = 1.1f;
					bgParallax = 0.43;
					bgTopY = (int)(backgroundTopMagicNumber * 1950.0 + 1750.0) + (int)scAdj + pushBGTopHack;
					bgScale *= bgGlobalScaleMultiplier;
					bgWidthScaled = (int)((float)Width * bgScale);
					SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / (float)bgParallax);
					bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));

					if (Main.gameMenu)
					{
						bgTopY = 400 + pushBGTopHack;
						bgStartX -= 80;
					}

					bgLoops = Main.screenWidth / bgWidthScaled + 2;
					if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
					{
						for (int i = -bgLoops; i < bgLoops; i++)
						{
							Main.spriteBatch.Draw(TextureAssets.Background[textureSlots[1]].Value, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + MiddleFarBGYOffset), new Rectangle(0, 0, Width, Height), BGActualColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
						}
					}

					//closer layer behind the front
					bgScale = 1.22f;
					bgParallax = 0.49;
					bgTopY = (int)(backgroundTopMagicNumber * 2100.0 + 2000.0) + (int)scAdj + pushBGTopHack;
					bgScale *= bgGlobalScaleMultiplier;
					bgWidthScaled = (int)((float)Width * bgScale);
					SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / (float)bgParallax);
					bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));

					if (Main.gameMenu)
					{
						bgTopY = 480 + pushBGTopHack;
						bgStartX -= 100;
					}

					bgLoops = Main.screenWidth / bgWidthScaled + 2;
					if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
					{
						for (int i = -bgLoops; i < bgLoops; i++)
						{
							Main.spriteBatch.Draw(TextureAssets.Background[textureSlots[2]].Value, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + MiddleCloseBGYOffset), new Rectangle(0, 0, Width, Height), BGActualColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
						}
					}

					//front layer
					bgScale = 1.34f;
					bgParallax = 0.56;
					bgTopY = (int)(backgroundTopMagicNumber * 2250.0 + 2250.0) + (int)scAdj + pushBGTopHack;
					bgScale *= bgGlobalScaleMultiplier;
					bgWidthScaled = (int)((float)Width * bgScale);
					SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / (float)bgParallax);
					bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));

					if (Main.gameMenu)
					{
						bgTopY = 560 + pushBGTopHack;
						bgStartX -= 120;
					}

					bgLoops = Main.screenWidth / bgWidthScaled + 2;
					if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
					{
						for (int i = -bgLoops; i < bgLoops; i++)
						{
							Main.spriteBatch.Draw(TextureAssets.Background[textureSlots[3]].Value, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + CloseBGYOffset), new Rectangle(0, 0, Width, Height), BGActualColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
						}
					}

					/*
					int CloseBGYOffset = -150;
					int MiddleBGYOffset = -200;
					int FarBGYOffset = -150;

					//back layer
					var bgScale = 1.05f;
					var bgParallax = 0.35;
					var bgTopY = (int)(backgroundTopMagicNumber * 1800.0 + 1500.0) + (int)scAdj + pushBGTopHack;
					bgScale *= bgGlobalScaleMultiplier;
					var bgWidthScaled = (int)((float)Width * bgScale);
					SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1.2f / (float)bgParallax);
					var bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));

					if (Main.gameMenu)
					{
						bgTopY = 320 + pushBGTopHack;
					}

					var bgLoops = Main.screenWidth / bgWidthScaled + 2;
					if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
					{
						for (int i = -bgLoops; i < bgLoops; i++)
						{
							Main.spriteBatch.Draw(TextureAssets.Background[textureSlots[1]].Value, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + FarBGYOffset), new Rectangle(0, 0, Width, Height), BGActualColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
						}
					}

					//middle layer
					bgScale = 1.2f;
					bgParallax = 0.45;
					bgTopY = (int)(backgroundTopMagicNumber * 2100.0 + 2000.0) + (int)scAdj + pushBGTopHack;
					bgScale *= bgGlobalScaleMultiplier;
					bgWidthScaled = (int)((float)Width * bgScale);
					SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / (float)bgParallax);
					bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));

					if (Main.gameMenu)
					{
						bgTopY = 480 + pushBGTopHack;
						bgStartX -= 100;
					}

					bgLoops = Main.screenWidth / bgWidthScaled + 2;
					if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
					{
						for (int i = -bgLoops; i < bgLoops; i++)
						{
							Main.spriteBatch.Draw(TextureAssets.Background[textureSlots[2]].Value, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + MiddleBGYOffset), new Rectangle(0, 0, Width, Height), BGActualColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
						}
					}

					//front layer
					bgScale = 1.35f;
					bgParallax = 0.55;
					bgTopY = (int)(backgroundTopMagicNumber * 2250.0 + 2250.0) + (int)scAdj + pushBGTopHack;
					bgScale *= bgGlobalScaleMultiplier;
					bgWidthScaled = (int)((float)Width * bgScale);
					SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / (float)bgParallax);
					bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));

					if (Main.gameMenu)
					{
						bgTopY = 560 + pushBGTopHack;
						bgStartX -= 120;
					}

					bgLoops = Main.screenWidth / bgWidthScaled + 2;
					if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
					{
						for (int i = -bgLoops; i < bgLoops; i++)
						{
							Main.spriteBatch.Draw(TextureAssets.Background[textureSlots[3]].Value, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + CloseBGYOffset), new Rectangle(0, 0, Width, Height), BGActualColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
						}
					}
					*/
				}
				//alternate background
				else
				{
					//width and height (this is not usually a good idea but each of these bgs are the same dimensions
					int Width = 1024;
					int Height = 700;

					//offsets for each individual background layer
					int CloseBGYOffset = -325;
					int MiddleBGYOffset = -325;
					int FarBGYOffset = -100;

					//back layer
					var bgScale = 1.05f;
					var bgParallax = 0.35;
					var bgTopY = (int)(backgroundTopMagicNumber * 1800.0 + 1500.0) + (int)scAdj + pushBGTopHack;
					bgScale *= bgGlobalScaleMultiplier;
					var bgWidthScaled = (int)((float)Width * bgScale);
					SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1.2f / (float)bgParallax);
					var bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));

					if (Main.gameMenu)
					{
						bgTopY = 320 + pushBGTopHack;
					}

					var bgLoops = Main.screenWidth / bgWidthScaled + 2;
					if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
					{
						for (int i = -bgLoops; i < bgLoops; i++)
						{
							Main.spriteBatch.Draw(TextureAssets.Background[textureSlots[0]].Value, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + FarBGYOffset), new Rectangle(0, 0, Width, Height), BGActualColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
						}
					}

					//middle layer
					bgScale = 1.2f;
					bgParallax = 0.45;
					bgTopY = (int)(backgroundTopMagicNumber * 2100.0 + 2000.0) + (int)scAdj + pushBGTopHack;
					bgScale *= bgGlobalScaleMultiplier;
					bgWidthScaled = (int)((float)Width * bgScale);
					SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / (float)bgParallax);
					bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));

					if (Main.gameMenu)
					{
						bgTopY = 480 + pushBGTopHack;
						bgStartX -= 100;
					}

					bgLoops = Main.screenWidth / bgWidthScaled + 2;
					if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
					{
						for (int i = -bgLoops; i < bgLoops; i++)
						{
							Main.spriteBatch.Draw(TextureAssets.Background[textureSlots[1]].Value, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + MiddleBGYOffset), new Rectangle(0, 0, Width, Height), BGActualColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
						}
					}

					//front layer
					bgScale = 1.35f;
					bgParallax = 0.55;
					bgTopY = (int)(backgroundTopMagicNumber * 2250.0 + 2250.0) + (int)scAdj + pushBGTopHack;
					bgScale *= bgGlobalScaleMultiplier;
					bgWidthScaled = (int)((float)Width * bgScale);
					SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / (float)bgParallax);
					bgStartX = (int)(0.0 - Math.IEEERemainder((double)Main.screenPosition.X * bgParallax, bgWidthScaled) - (double)(bgWidthScaled / 2));

					if (Main.gameMenu)
					{
						bgTopY = 560 + pushBGTopHack;
						bgStartX -= 120;
					}

					bgLoops = Main.screenWidth / bgWidthScaled + 2;
					if ((double)Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
					{
						for (int i = -bgLoops; i < bgLoops; i++)
						{
							Main.spriteBatch.Draw(TextureAssets.Background[textureSlots[2]].Value, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + CloseBGYOffset), new Rectangle(0, 0, Width, Height), BGActualColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
						}
					}
				}
			}

			Texture2D value = TextureAssets.MagicPixel.Value;
			float flashPower = SpookyWorld.BGTransitionFlash;
			Color color = Color.Black * flashPower;
			Main.spriteBatch.Draw(value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), color);

			return false;
		}

		public override void ModifyFarFades(float[] fades, float transitionSpeed)
		{
			for (int i = 0; i < fades.Length; i++)
			{
				if (i == Slot)
				{
					fades[i] += transitionSpeed;
					if (fades[i] > 1f)
					{
						fades[i] = 1f;
					}
				}
				else
				{
					fades[i] -= transitionSpeed;
					if (fades[i] < 0f)
					{
						fades[i] = 0f;
					}
				}
			}
		}
    }
}