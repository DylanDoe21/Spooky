using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Liquid;
using Microsoft.Xna.Framework;
using MonoMod.Cil;

using Spooky.Core;
using Spooky.Content.Backgrounds.TarPits;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class TarPitsBiome : ModBiome
    {
		public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<TarPitsUndergroundBG>();

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
					music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/TarPits");
				}

				return music;
			}
		}

		public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

		public override ModWaterStyle WaterStyle => ModContent.GetInstance<TarWaterStyle>();

		public override int BiomeTorchItemType => ItemID.DesertTorch;

		public override void Load()
		{
			IL_LiquidRenderer.DrawNormalLiquids += WaterOpacityChanger;
			IL_Main.oldDrawWater += OldWaterOpacityChanger;
			IL_TileDrawing.DrawTile_LiquidBehindTile += WaterSlopeOpacityChanger;
			On_WaterfallManager.GetAlpha += WaterfallOpacityChanger;
		}

		public override void Unload()
		{
			IL_LiquidRenderer.DrawNormalLiquids -= WaterOpacityChanger;
			IL_Main.oldDrawWater -= OldWaterOpacityChanger;
			IL_TileDrawing.DrawTile_LiquidBehindTile -= WaterSlopeOpacityChanger;
			On_WaterfallManager.GetAlpha -= WaterfallOpacityChanger;
		}

		//water opacity amount used for all il edits below
		float WaterOpacity = 0.88f;

		//Huge thanks to lion8cake for making all of these il edits
		private void WaterOpacityChanger(ILContext il)
		{
			ILCursor c = new(il);

			int pointer_varNum = -1; //To save the local variable index of the pointer used for drawing liquids
			int opacityNum_varNum = -1; //To save the local variable index of the alpha for the liquid

			c.GotoNext(MoveType.After, i => i.MatchLdloc(out pointer_varNum), i => i.MatchLdfld<LiquidRenderer.LiquidDrawCache>("Type"), i => i.MatchLdelemR4(), i => i.MatchBr(out _), i => i.MatchLdcR4(1), i => i.MatchMul(), i => i.MatchStloc(out opacityNum_varNum)); //match to saving of num at the line float num = ptr2->Opacity * (isBackgroundDraw ? 1f : DEFAULT_OPACITY[ptr2->Type]);
			c.EmitLdloca(opacityNum_varNum); //parse through num with a reference through the delegate
			//Ldloc2 or ptr2 is a pointer, (pointers are just accesses to fields through memory) which means that we can't parse them through a delegate by themselves
			//Here we parse through the pointer (ptr2) value for Type and Opacity since thats the only LiquidDrawCache values we use
			c.EmitLdloc(pointer_varNum);
			c.EmitLdfld(typeof(LiquidRenderer.LiquidDrawCache).GetField("Opacity")); //we get ptr2.Opacity by parsing throgh both ptr2 and the Opacity field
			c.EmitLdloc(pointer_varNum);
			c.EmitLdfld(typeof(LiquidRenderer.LiquidDrawCache).GetField("Type")); //we get ptr2.Opacity by parsing throgh both ptr2 and the Type field
			c.EmitLdarg(5);
			c.EmitDelegate((ref float num, float ptr2Opacity, byte ptr2Type, bool isBackgroundDraw) =>
			{
				//Anything placed in this delegate is like calling a new method 
				float LiquidOpacity = WaterOpacity; //ranges from 1f to 0f, here we set the color to 50% opacity
				bool opacityCondition = ptr2Type == LiquidID.Water && Main.waterStyle == ModContent.GetInstance<TarWaterStyle>().Slot; //the condition for when our opacity should be applied
				//This gets the liquid type water and gets the water style for our liquid, this can be changed to anything boolean related
				//We set num (or the opacity of the draw liquid) to either the original value or our value depending on the condition above
				num = opacityCondition ? ptr2Opacity * (isBackgroundDraw ? 1f : LiquidOpacity) : num;
			});
		}

		private void OldWaterOpacityChanger(ILContext il)
		{
			ILCursor c = new(il);
			int alpha_varNum = -1; //The local variable index for the alpha variable
			int x_varNum = -1; //The local variable index for the x tile position variable
			int y_varNum = -1; //The local variable index for the y tile position variable

			c.GotoNext(MoveType.After, i => i.MatchLdsflda<Main>("tile"), i => i.MatchLdloc(out x_varNum), i => i.MatchLdloc(out y_varNum)); //Here, we dynamically get the X and Y variable number, we do this so that when tmodloader updates adding more local variables to the method, this edit doesn't break
			c.GotoNext(MoveType.After, i => i.MatchLdsfld<Main>("drewLava"), i => i.MatchBrtrue(out _), i => i.MatchLdcR4(0.5f), i => i.MatchStloc(out alpha_varNum)); //After num9 is initiated with 0.5f, due to this that means this only works with Water
			c.EmitLdloca(alpha_varNum); //parse through num9 with a reference through the delegate
			c.EmitLdloc(y_varNum); //parse through y/j 
			c.EmitLdloc(x_varNum); //parse through x/i
			c.EmitDelegate((ref float num9, int j, int i) =>
			{
				Tile tile = Main.tile[i, j]; //use x and y to get the tile position
				float LiquidOpacity = WaterOpacity; //ranges from 1f to 0f, here we set the color to 50% opacity
				bool opacityCondition = tile.LiquidType == LiquidID.Water && Main.waterStyle == ModContent.GetInstance<TarWaterStyle>().Slot; //the condition for when our opacity should be applied
																																				 //This gets the liquid type water and gets the water style for our liquid, this can be changed to anything boolean related
				num9 = opacityCondition ? LiquidOpacity : num9; //we get the opacity and using the condition deciide whether to use the normal opacity or use our opacity
			});
		}

		private void WaterSlopeOpacityChanger(ILContext il)
		{
			ILCursor c = new(il);
			int alpha_varNum = -1; //The local variable index for the alpha variable

			c.GotoNext(MoveType.After, i => i.MatchLdcR4(0.5f), i => i.MatchStloc(out alpha_varNum)); //After num6 is initiated with 0.5f, due to this that means this only works with Water
			c.EmitLdloca(alpha_varNum); //parse through num6 with a reference through the delegate
			c.EmitLdarg(6); //parse through the method's tileX argument
			c.EmitLdarg(7); //parse through the method's tileY argument
			c.EmitDelegate((ref float num6, int tileX, int tileY) =>
			{
				Tile tile = Main.tile[tileX, tileY]; //use x and y to get the tile position
				float LiquidOpacity = WaterOpacity;
				bool opacityCondition = tile.LiquidType == LiquidID.Water && Main.waterStyle == ModContent.GetInstance<TarWaterStyle>().Slot; //the condition for when our opacity should be applied
																																				 //This gets the liquid type water and gets the water style for our liquid, this can be changed to anything boolean related
				num6 = opacityCondition ? LiquidOpacity : num6; //we get the opacity and using the condition deciide whether to use the normal opacity or use our opacity
			});
		}

		private float WaterfallOpacityChanger(On_WaterfallManager.orig_GetAlpha orig, float Alpha, int maxSteps, int waterfallType, int y, int s, Tile tileCache)
		{
			float LiquidOpacity = WaterOpacity;
			bool opacityCondition = waterfallType == ModContent.GetInstance<TarWaterfallStyle>().Slot; //the condition for when our opacity should be applied
																									   //This gets just our waterfall type
			if (opacityCondition)
			{
				float num = LiquidOpacity * Alpha; //multiply our opacity with alpha so liquid transitions still work
				if (s > maxSteps - 10) //mimics the fading at the end of the waterfalls
				{
					num *= (float)(maxSteps - s) / 10f;
				}
				return num;
			}
			return orig.Invoke(Alpha, maxSteps, waterfallType, y, s, tileCache);
		}

		//bestiary stuff
		public override string BestiaryIcon => "Spooky/Content/Biomes/TarPitsBiomeIcon";
		public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void OnInBiome(Player player)
        {
            player.ZoneDesert = false;
			player.ZoneCrimson = false;
			player.ZoneCorrupt = false;
			player.ZoneHallow = false;
			player.ZoneJungle = false;

			if (player.wet)
			{
				player.velocity *= 0.85f;
			}
		}

		public override bool IsBiomeActive(Player player)
        {
			//part of the biome condition makes it so that there must be more tar pits tiles than sand tiles so the biome zone doesnt overreach in game
			bool BiomeCondition = ModContent.GetInstance<TileCount>().tarPitsTiles >= 500 && (ModContent.GetInstance<TileCount>().tarPitsTiles > Main.SceneMetrics.SandTileCount / 5);
			bool UndergroundCondition = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

            return BiomeCondition && UndergroundCondition;
        }
    }
}