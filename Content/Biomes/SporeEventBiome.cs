using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Achievements;

namespace Spooky.Content.Biomes
{
    public class SporeEventBiome : ModBiome
    {
		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SporeEvent");

		public override SceneEffectPriority Priority => SceneEffectPriority.Event;

		public static Asset<Effect> SporeMist;
		public static Asset<Texture2D> SwirlyNoise, SwirlyNoiseInv, StarNoise;

		public override void Load()
        {
			On_Main.DrawInfernoRings += SporeFogDraw;

			SporeMist = ModContent.Request<Effect>("Spooky/Effects/MoldMist");

			SwirlyNoise = ModContent.Request<Texture2D>("Spooky/ShaderAssets/swirlyNoise");
			SwirlyNoiseInv = ModContent.Request<Texture2D>("Spooky/ShaderAssets/swirlyNoiseInverted");
			StarNoise = ModContent.Request<Texture2D>("Spooky/ShaderAssets/starNoise");
		}

		float FogAlpha = 0f;
		bool InitializedColors = false;

		List<Color> FogColorList = new List<Color> { };
		List<Color> SporeColorList = new List<Color> { };

		private void SporeFogDraw(On_Main.orig_DrawInfernoRings orig, Main self)
		{
			orig(self);

			if ((Flags.SporeEventHappening && Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpiderCaveBiome>())) || 
			ModContent.GetInstance<TileCount>().sporeMonolith >= 4 || Main.LocalPlayer.GetModPlayer<SpookyPlayer>().SporeMonolithEquipped)
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
				Effect sporeEffect = SporeMist.Value;

				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.Default, RasterizerState.CullNone, (Effect)sporeEffect, Main.GameViewMatrix.TransformationMatrix);

				Main.graphics.GraphicsDevice.Textures[1] = SwirlyNoise.Value;
				Main.graphics.GraphicsDevice.Textures[2] = SwirlyNoiseInv.Value;
				Main.graphics.GraphicsDevice.Textures[3] = StarNoise.Value;

				if (!InitializedColors)
				{
					List<Color> ColorList1 = new List<Color>
					{
						new Color(167, 0, 255),
						new Color(229, 93, 11),
						new Color(0, 212, 136), 
						new Color(23, 84, 255),
						new Color(245, 0, 6),
						new Color(122, 147, 132)
					};
					List<Color> ColorList2 = new List<Color>
					{
						new Color(167, 0, 255),
						new Color(229, 93, 11),
						new Color(0, 212, 136),
						new Color(23, 84, 255),
						new Color(245, 0, 6),
						new Color(122, 147, 132)
					};

					for (int i = 0; i < 3; i++)
					{
						int ColorToPutInFoglist = WorldGen.genRand.Next(ColorList1.Count);
						int ColorToPutInSporelist = WorldGen.genRand.Next(ColorList2.Count);

						FogColorList.Add(ColorList1[ColorToPutInFoglist]);
						SporeColorList.Add(ColorList2[ColorToPutInSporelist]);

						ColorList1.RemoveAt(ColorToPutInFoglist);
						ColorList2.RemoveAt(ColorToPutInSporelist);
					}

					InitializedColors = true;
				}

				void DrawFog(Texture2D tex, Vector2 offset, bool usePosOffset = false)
				{
					for (int i = -1; i < 2; i += 2)
					{
						for (int j = -1; j < 2; j += 2)
						{
							Vector2 origPosOffset = usePosOffset ? new Vector2(offset.X, offset.Y) : Vector2.Zero;

							Vector2 origPos = Main.LocalPlayer.Center - origPosOffset - new Vector2(Main.screenWidth * i, Main.screenHeight * j) * 0.5f;

							Vector2 pos = new Vector2(MathF.Floor(origPos.X / (Main.screenWidth)), MathF.Floor(origPos.Y / (Main.screenHeight))) * new Vector2(Main.screenWidth, Main.screenHeight) - Main.screenPosition + offset;

							Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, Main.screenWidth, Main.screenHeight);
							if (!rect.Intersects(new Rectangle(0, 0, Main.screenWidth, Main.screenHeight)))
								continue;

							Main.spriteBatch.Draw(tex, rect, Color.White);
						}
					}
				}

				float fade = Main.GameUpdateCount % 600 / 600f;
				int index = (int)(Main.GameUpdateCount / 600 % 3);

				Color color1 = Color.Lerp(FogColorList[index], FogColorList[(index + 1) % 3], fade);
				Color color2 = Color.Lerp(SporeColorList[index], SporeColorList[(index + 1) % 3], fade);

				float IntensityToUse = (ModContent.GetInstance<TileCount>().sporeMonolith >= 4 || Main.LocalPlayer.GetModPlayer<SpookyPlayer>().SporeMonolithEquipped) ? 1f : Flags.SporeFogIntensity;

				//draw the top layer of fog
				sporeEffect.Parameters["uOpacityTotal"].SetValue(1.5f * (0.8f * IntensityToUse) * FogAlpha);
				sporeEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 60);
				sporeEffect.Parameters["uColor"].SetValue(color1.ToVector4());
				sporeEffect.Parameters["uExponent"].SetValue(3f);
				sporeEffect.Parameters["uScale"].SetValue(1f);
				for (int i = -1; i < 2; i += 2)
				{
					sporeEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * i / 60);
					DrawFog(SwirlyNoise.Value, new Vector2(Main.screenWidth * 3f, Main.screenHeight * 3f) * 0.125f * i, true);
				}

				//draw second layer of slower moving fog
				sporeEffect.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly / 90);
				sporeEffect.Parameters["uColor"].SetValue(color1.ToVector4());
				sporeEffect.Parameters["uScale"].SetValue(2f);
				for (int i = -1; i < 2; i += 2)
				{
					sporeEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * i / 90);
					DrawFog(SwirlyNoiseInv.Value, new Vector2(Main.screenWidth, Main.screenHeight * 1.5f) * 0.125f * i);
				}

				//draw star texture to look like spores are in the air
				sporeEffect.Parameters["uOpacityTotal"].SetValue(2 * (1.5f * IntensityToUse) * FogAlpha);
				sporeEffect.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly / 120);
				sporeEffect.Parameters["uColor"].SetValue(color2.ToVector4());
				sporeEffect.Parameters["uExponent"].SetValue(2f);
				sporeEffect.Parameters["uScale"].SetValue(1f);
				DrawFog(StarNoise.Value, Vector2.Zero);
			}
			else
			{
				InitializedColors = false;

				if (FogColorList.Count > 0)
				{
					FogColorList.Clear();
				}
				if (SporeColorList.Count > 0)
				{
					SporeColorList.Clear();
				}
			}
        }

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/SporeEventBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

		public override void OnInBiome(Player player)
		{
			Lighting.GlobalBrightness = 1f; //same as when you have blindness in vanilla

			ModContent.GetInstance<EventAchievementSpore>().SporeCondition.Complete();
		}

        public override bool IsBiomeActive(Player player)
        {
            return Flags.SporeEventHappening && player.InModBiome(ModContent.GetInstance<SpiderCaveBiome>());
        }
    }
}