using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
    public class BigBoneStatue : ModTile
    {
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 5;
			TileObjectData.newTile.Origin = new Point16(2, 4);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(86, 46, 29));
            DustType = DustID.t_Lihzahrd;
            HitSound = SoundID.Tink;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) 
		{
			float divide = 220f;

			r = 255f / divide;
			g = 125f / divide;
			b = 0f / divide;
		}

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) 
		{
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

			if (Main.drawToScreen) 
			{
				zero = Vector2.Zero;
			}

			Tile tile = Main.tile[i, j];
			int width = 16;
			int offsetY = 0;
			int height = 16;
			short frameX = tile.TileFrameX;
			short frameY = tile.TileFrameY;

			TileLoader.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref frameX, ref frameY);

			ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i);

			for (int numFlames = 0; numFlames < 5; numFlames++) 
			{
				float shakeX = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
				float shakeY = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
				spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + shakeX, j * 16 - (int)Main.screenPosition.Y + offsetY + shakeY) + zero, 
				new Rectangle(frameX, frameY, width, height), Color.White * 0.5f, 0f, default, 1f, SpriteEffects.None, 0f);
			}
		}
    }
}