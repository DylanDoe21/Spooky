using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;

namespace Spooky.Content.Backgrounds.SpookyBiome
{
	public static class BGReflectionUtil
	{
		public static object GetFieldValue(this Type type, string fieldName, object obj = null, BindingFlags? flags = null)
		{
			flags ??= BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
			FieldInfo field = type.GetField(fieldName, flags.Value);
			return field.GetValue(obj);
		}

		public static T GetFieldValue<T>(this Type type, string fieldName, object obj = null, BindingFlags? flags = null)
		{
			flags ??= BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
			FieldInfo field = type.GetField(fieldName, flags.Value);
			return (T)field.GetValue(obj);
		}
	}

	public class SpookyForestBG : ModSurfaceBackgroundStyle
	{
		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
		{
			return -1; //BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyForestBG1");
		}

		public override int ChooseMiddleTexture()
		{
			return -1; //BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyForestBG2");
		}

		public override int ChooseFarTexture()
		{
			return -1; //BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyForestBG3");
		}

		public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
		{
			float a = 1024f;
			float b = 700f;
			int[] textureSlots = new int[] 
			{	
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBG4"),
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBG3"),
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBG2"),
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBG1"),
			};

			int length = textureSlots.Length;
			for (int i = 0; i < textureSlots.Length; i++)
			{
				float bgParallax = 0.37f + 0.2f - (0.1f * (length - i));
				int textureSlot = textureSlots[i];
				Main.instance.LoadBackground(textureSlot);
				float bgScale = 2.5f;
				SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / bgParallax);
				float screenOff = typeof(Main).GetFieldValue<float>("screenOff", Main.instance);
				float scAdj = typeof(Main).GetFieldValue<float>("scAdj", Main.instance);

				//horizontal parallax
				int bgW = (int)(Main.backgroundWidth[textureSlot] * bgScale);
				int bgStart = (int)(-Math.IEEERemainder(Main.screenPosition.X * bgParallax, bgW) - (bgW / 2));

				//offset values so each background layer isnt drawn way too high above one another to give a more flat terrain feel
				int TopHeightOffset = 0;

				switch (i)
				{
					case 0:
					{
						TopHeightOffset = 10;
						break;
					}
					case 1:
					{
						TopHeightOffset = -25;
						break;
					}
					case 2:
					{
						TopHeightOffset = -100;
						break;
					}
					case 3:
					{
						TopHeightOffset = -350;
						break;
					}
				}

				int bgTop = (int)((-Main.screenPosition.Y + screenOff / 2f) / (Main.worldSurface * 16.0) * a + b) + (int)scAdj - ((length - i) * TopHeightOffset);
				int bgLoops = Main.screenWidth / bgW + 2;

				if (Main.gameMenu)
				{
					bgTop = 320;
				}

				Color backColor = typeof(Main).GetFieldValue<Color>("ColorOfSurfaceBackgroundsModified", Main.instance);

				if (Main.screenPosition.Y < Main.worldSurface * 16.0 + 16.0)
				{
					for (int k = 0; k < bgLoops; k++)
					{
						Main.spriteBatch.Draw(TextureAssets.Background[textureSlot].Value,
						new Vector2(bgStart + bgW * k, bgTop),
						new Rectangle(0, 0, Main.backgroundWidth[textureSlot], Main.backgroundHeight[textureSlot]),
						backColor, 0f, default, bgScale, SpriteEffects.None, 0f);
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