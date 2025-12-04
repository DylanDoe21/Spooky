using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Items.Pets;

namespace Spooky.Content.Tiles.SpiderCave.Ambient
{
	public class MushroomFriendTile : ModTile
	{
		private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			RegisterItemDrop(ModContent.ItemType<FriendlyShroom>());
			AddMapEntry(new Color(198, 174, 180));
			DustType = 288;
            HitSound = SoundID.Dig;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Cap");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;

			Color color = Main.LocalPlayer.shirtColor.MultiplyRGBA(Lighting.GetColor(new Point(i, j)));

            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), color);
        }
	}
}