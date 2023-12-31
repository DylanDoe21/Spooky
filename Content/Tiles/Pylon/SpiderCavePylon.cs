using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Map;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.ModLoader.Default;
using Terraria.ObjectData;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Biomes;

namespace Spooky.Content.Tiles.Pylon
{
    public static class SpiderCaveCondition
    {
        public static Condition InSpiderCaveBiome = new Condition("Mods.Spooky.Conditions.InSpiderCaveBiome", () => Main.LocalPlayer.InModBiome<Biomes.SpiderCaveBiome>());
    }

    public class SpiderCavePylon : ModPylon
	{
		public const int CrystalVerticalFrameCount = 8;

		public Asset<Texture2D> crystalTexture;
		public Asset<Texture2D> crystalHighlightTexture;
		public Asset<Texture2D> mapIcon;

		public override void Load() 
        {
			crystalTexture = ModContent.Request<Texture2D>(Texture + "Crystal");
			crystalHighlightTexture = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Pylon/PylonHighlight");
			mapIcon = ModContent.Request<Texture2D>(Texture + "Icon");
		}

		public override void SetStaticDefaults() 
        {
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.InteractibleByNPCs[Type] = true;
			TileID.Sets.PreventsSandfall[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TEModdedPylon moddedPylon = ModContent.GetInstance<PylonTileEntity>();
			TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(moddedPylon.PlacementPreviewHook_CheckIfCanPlace, 1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(moddedPylon.Hook_AfterPlacement, -1, 0, false);
			TileObjectData.addTile(Type);
			AddToArray(ref TileID.Sets.CountsAsPylon);
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(Color.OrangeRed, name);
            DustType = -1;
		}

        public override NPCShop.Entry GetNPCShopEntry()
        {
            return new NPCShop.Entry(ModContent.ItemType<SpiderCavePylonItem>(), Condition.HappyEnoughToSellPylons, SpiderCaveCondition.InSpiderCaveBiome);
        }

        public override void MouseOver(int i, int j) 
        {
			Main.LocalPlayer.cursorItemIconEnabled = true;
			Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<SpiderCavePylonItem>();
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
			ModContent.GetInstance<PylonTileEntity>().Kill(i, j);
		}

		public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) 
        {
			return ModContent.GetInstance<TileCount>().spiderCaveTiles >= 1800;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) 
        {
			r = 0.5f;
			g = 0.5f;
			b = 0.5f;
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) 
        {
			DefaultDrawPylonCrystal(spriteBatch, i, j, crystalTexture, crystalHighlightTexture, new Vector2(0f, -12f), Color.White * 0.1f, Color.White, 4, CrystalVerticalFrameCount);
		}

		public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale) 
        {
			bool mouseOver = DefaultDrawMapIcon(ref context, mapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1.5f, 2f), drawColor, deselectedScale, selectedScale);
			DefaultMapClickHandle(mouseOver, pylonInfo, ModContent.GetInstance<SpiderCavePylonItem>().DisplayName.Key, ref mouseOverText);
		}
	}
}