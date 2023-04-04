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

namespace Spooky.Content.Tiles.Pylon
{
	public class SpookyHellPylon : ModPylon
	{
		public const int CrystalVerticalFrameCount = 8;

		public Asset<Texture2D> crystalTexture;
		public Asset<Texture2D> crystalHighlightTexture;
		public Asset<Texture2D> mapIcon;

		public override void Load() 
        {
			// We'll need these textures for later, it's best practice to cache them on load instead of continually requesting every draw call.
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
            AddMapEntry(Color.Red, name);
			DustType = -1;
		}

		public override bool ValidTeleportCheck_NPCCount(TeleportPylonInfo pylonInfo, int defaultNecessaryNPCCount) 
		{
			//allows this pylon to be used, regardless of nearby npcs
			return true;
		}

		public override NPCShop.Entry GetNPCShopEntry()
        {
			//this pylon is never sold
			return null;
		}

		public override void MouseOver(int i, int j) 
        {
			Main.LocalPlayer.cursorItemIconEnabled = true;
			Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<SpookyHellPylonItem>();
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
			ModContent.GetInstance<PylonTileEntity>().Kill(i, j);
		}
		
		public override bool ValidTeleportCheck_BiomeRequirements(TeleportPylonInfo pylonInfo, SceneMetrics sceneData) 
        {
			return ModContent.GetInstance<TileCount>().spookyHellTiles >= 500;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) 
        {
			// Pylons in vanilla light up, which is just a simple functionality we add using ModTile's ModifyLight.
			// Let's just add a simple white light for our pylon:
			r = g = b = 0.75f;
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) 
        {
			// We want to draw the pylon crystal the exact same way vanilla does, so we can use this built in method in ModPylon for default crystal drawing:
			// For the sake of example, lets make our pylon create a bit more dust by decreasing the dustConsequent value down to 1. If you want your dust spawning to be identical to vanilla, set dustConsequent to 4.
			// We also multiply the pylonShadowColor in order to decrease its opacity, so it actually looks like a "shadow"
			DefaultDrawPylonCrystal(spriteBatch, i, j, crystalTexture, crystalHighlightTexture, new Vector2(0f, -12f), Color.White * 0.1f, Color.White, 4, CrystalVerticalFrameCount);
		}

		public override void DrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, TeleportPylonInfo pylonInfo, bool isNearPylon, Color drawColor, float deselectedScale, float selectedScale) 
        {
			// Just like in SpecialDraw, we want things to be handled the EXACT same way vanilla would handle it, which ModPylon also has built in methods for:
			bool mouseOver = DefaultDrawMapIcon(ref context, mapIcon, pylonInfo.PositionInTiles.ToVector2() + new Vector2(1.5f, 2f), drawColor, deselectedScale, selectedScale);
			DefaultMapClickHandle(mouseOver, pylonInfo, "Mods.Spooky.ItemName.SpookyHellPylonItem", ref mouseOverText);
		}
	}
}