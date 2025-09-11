using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Map;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.ModLoader.Default;
using Terraria.ObjectData;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

using Spooky.Content.Biomes;
using Spooky.Content.Tiles.Minibiomes.Christmas;

namespace Spooky.Content.Tiles.Pylon
{
    public static class KrampusCondition
    {
        public static Condition InKrampusWorkshop = new Condition("Mods.Spooky.Conditions.InKrampusWorkshop", () => Main.LocalPlayer.InModBiome<ChristmasDungeonBiome>());
    }

    public class KrampusPylon : ModPylon
	{
		public const int CrystalVerticalFrameCount = 8;

		public Asset<Texture2D> crystalTexture;
		public Asset<Texture2D> crystalHighlightTexture;
		public Asset<Texture2D> mapIcon;

		public static readonly SoundStyle MachineSound = new("Spooky/Content/Sounds/KrampusPylonNoise", SoundType.Sound) { Volume = 0.1f };

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
            AddMapEntry(Color.Teal, name);
            DustType = -1;
		}

		public override NPCShop.Entry GetNPCShopEntry()
        {
			//this pylon should never be sold
            return null;
        }

		public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount) 
		{
			//allows this pylon to be used, regardless of nearby npcs
			return true;
		}

		public override void MouseOver(int i, int j) 
        {
			Main.LocalPlayer.cursorItemIconEnabled = true;
			Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<KrampusPylonItem>();
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
			ModContent.GetInstance<PylonTileEntity>().Kill(i, j);
		}

		public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) 
        {
			int[] DungeonWalls = new int[] { ModContent.WallType<ChristmasBrickRedWall>(), ModContent.WallType<ChristmasBrickBlueWall>(),
			ModContent.WallType<ChristmasBrickGreenWall>(), ModContent.WallType<ChristmasWoodWall>(), ModContent.WallType<ChristmasWindow>() };

			bool BiomeCondition = DungeonWalls.Contains(Main.tile[pylonInfo.PositionInTiles.X, pylonInfo.PositionInTiles.Y].WallType);

			return BiomeCondition;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) 
        {
			r = 0.5f;
			g = 0.45f;
			b = 0.5f;
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) 
        {
			DefaultDrawPylonCrystal(spriteBatch, i, j, crystalTexture, crystalHighlightTexture, new Vector2(0f, -12f), Color.Gray * 0.1f, Color.Gray, 20, CrystalVerticalFrameCount);
		}

		public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale) 
        {
			bool mouseOver = DefaultDrawMapIcon(ref context, mapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1.5f, 2f), drawColor, deselectedScale, selectedScale);
			DefaultMapClickHandle(mouseOver, pylonInfo, ModContent.GetInstance<KrampusPylonItem>().DisplayName.Key, ref mouseOverText);
		}
	}
}