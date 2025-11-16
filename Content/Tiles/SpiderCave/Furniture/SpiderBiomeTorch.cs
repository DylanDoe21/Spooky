using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpiderCave.Furniture
{
	public class SpiderBiomeTorch : ModTile
	{
        private Asset<Texture2D> GlowTexture;

		public sealed override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileNoAttach[Type] = true;
			Main.tileNoFail[Type] = true;
			Main.tileWaterDeath[Type] = true;
			TileID.Sets.FramesOnKillWall[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.DisableSmartInteract[Type] = true;
			TileID.Sets.Torch[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.StyleTorch);

			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);

			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124, 561, 574, 575, 576, 577, 578 };
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);

			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[] { 124, 561, 574, 575, 576, 577, 578 };
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);

			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.addAlternate(0);
			TileObjectData.addTile(Type);
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
			AddMapEntry(new Color(253, 221, 3), Lang.GetItemName(ItemID.Torch));
			AdjTiles = new int[] { TileID.Torches };
		}

		public override bool CanPlace(int i, int j)
		{
			return Main.tile[i, j].LiquidAmount == 0;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;

			int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
			player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = Main.rand.Next(1, 3);
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
		{
			offsetY = 0;
			if (WorldGen.SolidTile(i, j - 1))
			{
				offsetY = 4;
			}
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 66)
            {
                float divide = 300f;

                r = 255f / divide;
                g = 196f / divide;
                b = 0f / divide;
            }
        }

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

			Tile tile = Main.tile[i, j];
			if (!TileDrawing.IsVisible(tile))
			{
				return;
			}

			ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);

			Color color = new Color(55, 55, 55, 0);
			short frameX = Main.tile[i, j].TileFrameX;
			short frameY = Main.tile[i, j].TileFrameY;
			int width = 20;
			int offsetY = 0;
			int height = 20;
			TileLoader.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref frameX, ref frameY);

			Vector2 zero = new Vector2(Main.offScreenRange);

			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}

			for (int k = 0; k < 7; k++)
			{
				float x = (float)Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
				float y = (float)Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
				spriteBatch.Draw(GlowTexture.Value, new Vector2((float)(i * 16 - (int)Main.screenPosition.X) - (width - 16f) / 2f + x, (float)(j * 16 - (int)Main.screenPosition.Y + offsetY) + y) + zero, new Rectangle(frameX, frameY, width, height), color, 0f, default, 1f, SpriteEffects.None, 0);
			}
		}
	}
}