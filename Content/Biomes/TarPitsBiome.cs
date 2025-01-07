using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Liquid;
using Microsoft.Xna.Framework;
using System.Reflection;
using MonoMod.Cil;

using Spooky.Core;
using Spooky.Content.Backgrounds.TarPits;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class TarPitsBiome : ModBiome
    {
		public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<TarPitsUndergroundBG>();

		public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/TarPits");

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
			c.GotoNext(MoveType.After, i => i.MatchMul(), i => i.MatchStloc(7)); //match to saving of num at the line float num = ptr2->Opacity * (isBackgroundDraw ? 1f : DEFAULT_OPACITY[ptr2->Type]);
			c.EmitLdloca(7); 
			//parse through num with a reference through the delegate
			//Ldloc2 or ptr2 is a pointer, (pointers are just accesses to fields through memory) which means that we can't parse them through a delegate by themselves
			//Here we parse through the pointer (ptr2) value for Type and Opacity since thats the only LiquidDrawCache values we use

			c.EmitLdloc2();
			c.EmitLdfld(typeof(LiquidRenderer).GetNestedType("LiquidDrawCache", BindingFlags.NonPublic).GetField("Opacity")); //we get ptr2.Opacity by parsing throgh both ptr2 and the Opacity field
			c.EmitLdloc2();
			c.EmitLdfld(typeof(LiquidRenderer).GetNestedType("LiquidDrawCache", BindingFlags.NonPublic).GetField("Type")); //we get ptr2.Opacity by parsing throgh both ptr2 and the Type field
			c.EmitLdarg(5);
			c.EmitDelegate((ref float num, float ptr2Opacity, byte ptr2Type, bool isBackgroundDraw) =>
			{
				//Anything placed in this delegate is like calling a new method 
				float LiquidOpacity = WaterOpacity; //ranges from 1f to 0f
				bool opacityCondition = ptr2Type == LiquidID.Water && Main.waterStyle == ModContent.GetInstance<TarWaterStyle>().Slot;
				//the condition for when our opacity should be applied
				//This gets the liquid type water and gets the water style for our liquid, this can be changed to anything boolean related
				//We set num (or the opacity of the draw liquid) to either the original value or our value depending on the condition above

				num = opacityCondition ? ptr2Opacity * (isBackgroundDraw ? 1f : LiquidOpacity) : num;
			});
		}

		private void OldWaterOpacityChanger(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchLdcR4(out _), i => i.MatchStloc(17)); //After num9 is initiated with 0.5f, due to this that means this only works with Water
			c.EmitLdloca(17); //parse through num9 with a reference through the delegate
			c.EmitLdloc(11); //parse through y/j 
			c.EmitLdloc(12); //parse through x/i
			c.EmitDelegate((ref float num9, int j, int i) =>
			{
				Tile tile = Main.tile[i, j]; //use x and y to get the tile position
				float LiquidOpacity = WaterOpacity; //ranges from 1f to 0f
				bool opacityCondition = tile.LiquidType == LiquidID.Water && Main.waterStyle == ModContent.GetInstance<TarWaterStyle>().Slot; //the condition for when our opacity should be applied
																																				//This gets the liquid type water and gets the water style for our liquid, this can be changed to anything boolean related
				num9 = opacityCondition ? LiquidOpacity : num9; //we get the opacity and using the condition deciide whether to use the normal opacity or use our opacity
			});
		}

		private void WaterSlopeOpacityChanger(ILContext il)
		{
			ILCursor c = new(il);
			c.GotoNext(MoveType.After, i => i.MatchLdcR4(out _), i => i.MatchStloc(18)); //After num6 is initiated with 0.5f, due to this that means this only works with Water
			c.EmitLdloca(18); //parse through num6 with a reference through the delegate
			c.EmitLdarg(6); //parse through the method's tileX argument
			c.EmitLdarg(7); //parse through the method's tileY argument
			c.EmitDelegate((ref float num6, int tileX, int tileY) =>
			{
				Tile tile = Main.tile[tileX, tileY]; //use x and y to get the tile position
				float LiquidOpacity = WaterOpacity; //ranges from 1f to 0f
				bool opacityCondition = tile.LiquidType == LiquidID.Water && Main.waterStyle == ModContent.GetInstance<TarWaterStyle>().Slot; //the condition for when our opacity should be applied
																																				//This gets the liquid type water and gets the water style for our liquid, this can be changed to anything boolean related
				num6 = opacityCondition ? LiquidOpacity : num6; //we get the opacity and using the condition deciide whether to use the normal opacity or use our opacity
			});
		}

		private float WaterfallOpacityChanger(On_WaterfallManager.orig_GetAlpha orig, float Alpha, int maxSteps, int waterfallType, int y, int s, Tile tileCache)
		{
			float LiquidOpacity = WaterOpacity; //ranges from 1f to 0f
			bool opacityCondition = waterfallType == ModContent.GetInstance<TarWaterfallStyle>().Slot; 
			//the condition for when our opacity should be applied
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

			if (player.wet)
			{
				player.velocity *= 0.85f;
			}
		}

		public override bool IsBiomeActive(Player player)
        {
			bool BiomeCondition = ModContent.GetInstance<TileCount>().tarPitsTiles >= 500 && Main.SceneMetrics.SandTileCount < 9000;
			bool UndergroundCondition = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

            return BiomeCondition && UndergroundCondition;
        }
    }
}