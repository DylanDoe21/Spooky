using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;

namespace Spooky.Content.Backgrounds.Cemetery
{
	public class CemeteryBGAlt : ModSurfaceBackgroundStyle
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

			//width and height (this is not usually a good idea but each of these 
			int Width = 1024;
			int Height = 700;

			//offsets for each individual background layer
			int CloseBGYOffset = 140;
			int MiddleBGYOffset = 85;
			int FarBGYOffset = 85;

			int[] textureSlots = new int[]
			{
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/Cemetery/CemeteryBGAlt3"),
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/Cemetery/CemeteryBGAlt2"),
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/Cemetery/CemeteryBGAlt1"),
			};

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
				//far layer
				var bgScale = 1f;
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
						Main.spriteBatch.Draw(TextureAssets.Background[textureSlots[1]].Value, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + MiddleBGYOffset), new Rectangle(0, 0, Width, Height), BGActualColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
					}
				}

				//front layer
				bgScale = 1.34f;
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
						Main.spriteBatch.Draw(TextureAssets.Background[textureSlots[2]].Value, new Vector2(bgStartX + bgWidthScaled * i, bgTopY + CloseBGYOffset), new Rectangle(0, 0, Width, Height), BGActualColor, 0f, default(Vector2), bgScale, SpriteEffects.None, 0f);
					}
				}
			}

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
