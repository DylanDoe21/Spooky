using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria;
using Terraria.ModLoader;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Backgrounds.Cemetery
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

	public class CemeteryBG : ModSurfaceBackgroundStyle
	{
		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
		{
			return -1; //BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/Cemetery/CemeteryBG1");
		}

		public override int ChooseMiddleTexture()
		{
			return -1; //BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/Cemetery/CemeteryBG2");
		}

		public override int ChooseFarTexture()
		{
			return -1; //BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/Cemetery/CemeteryBG3");
		}

		public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
		{
			float a = 1200f;
			float b = 1680f;
			int[] textureSlots = new int[] 
			{
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/Cemetery/CemeteryBG3"),
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/Cemetery/CemeteryBG2"),
				BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/Cemetery/CemeteryBG1"),
			};

			int length = textureSlots.Length;
			for (int i = 0; i < textureSlots.Length; i++)
			{
				//Custom: bgScale,textureslot,patallaz,these 2 numbers....,Top and Start?
				float bgParallax = 0.37f + 0.2f - (0.1f * (length - i));
				int textureSlot = textureSlots[i];
				Main.instance.LoadBackground(textureSlot);
				float bgScale = 2.5f;
				int bgW = (int)(Main.backgroundWidth[textureSlot] * bgScale);
				SkyManager.Instance.DrawToDepth(Main.spriteBatch, 1f / bgParallax);
				float screenOff = typeof(Main).GetFieldValue<float>("screenOff", Main.instance);
				float scAdj = typeof(Main).GetFieldValue<float>("scAdj", Main.instance);
				int bgStart = (int)(-Math.IEEERemainder(Main.screenPosition.X * bgParallax, bgW) - (bgW / 2));

				//offset values so each background layer isnt drawn way too high above one another to give a more flat terrain feel
				int TopHeightOffset = i == 0 ? 320 : (i == 1 ? 400 : 500);
				int bgTop = (int)((-Main.screenPosition.Y + screenOff / 2f) / (Main.worldSurface * 16.0) * a + b) + (int)scAdj - ((length - i) * TopHeightOffset);
				int bgLoops = Main.screenWidth / bgW + 2;

				if (Main.gameMenu)
				{
					int TopHeightOffsetForMenu = i == 0 ? 320 : (i == 1 ? 420 : 520);
					bgTop = TopHeightOffsetForMenu;
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
