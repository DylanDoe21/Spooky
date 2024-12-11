using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Spooky.Core;

namespace Spooky.Content.Tiles.Catacomb
{
    public class CatacombGrassWall1 : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombGrassWall";

        private static Asset<Texture2D> MergeTexture;
        private static Asset<Texture2D> LeafTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(2, 42, 0));
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
        }

        public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }

		public static void DrawTreePiece(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null)
		{
			Tile tile = Main.tile[i, j];
			Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
			Color color = Lighting.GetColor(i, j);

			Main.spriteBatch.Draw(tex, drawPos, source, color, 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Catacomb/CatacombGrassWall1Merge");
            LeafTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/CemeteryGrassWallLeaf");

			Vector2 pos = TileGlobal.TileCustomPosition(i, j);

			//do not draw any fancy grass on the wall if its merged with a surrounding wall
			if (Main.tile[i, j + 1].WallType != ModContent.WallType<CatacombBrickWall1>() && Main.tile[i - 1, j].WallType != ModContent.WallType<CatacombBrickWall1>() &&
			Main.tile[i, j - 1].WallType != ModContent.WallType<CatacombBrickWall1>() && Main.tile[i + 1, j].WallType != ModContent.WallType<CatacombBrickWall1>())
			{
				if (i > Main.screenPosition.X / 16 && i < Main.screenPosition.X / 16 + Main.screenWidth / 16 && j > Main.screenPosition.Y / 16 && j < Main.screenPosition.Y / 16 + Main.screenHeight / 16)
				{
					var rand = new Random(i + (j * 100000));

					float offset = i * j % 6.28f + (float)rand.NextDouble() / 8f;
					float sin = (float)Math.Sin(Main.GameUpdateCount / 45f + offset);

					spriteBatch.Draw(LeafTexture.Value, pos + new Vector2(6, 6) + new Vector2(1, 0.5f) * sin * 2.2f,
					new Rectangle(rand.Next(6) * 18, 0, 16, 16), Lighting.GetColor(i, j), sin * 0.09f, new Vector2(3, 6), 1 + sin / 14f, 0, 0);
				}
			}
			else
			{
				//down wall merge
				if (Main.tile[i, j + 1].WallType == ModContent.WallType<CatacombBrickWall1>())
				{
					spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 0, 0, 16, 16), Lighting.GetColor(i, j));
				}
				//left wall merge
				if (Main.tile[i - 1, j].WallType == ModContent.WallType<CatacombBrickWall1>())
				{
					spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 1, 0, 16, 16), Lighting.GetColor(i, j));
				}
				//up wall merge
				if (Main.tile[i, j - 1].WallType == ModContent.WallType<CatacombBrickWall1>())
				{
					spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 2, 0, 16, 16), Lighting.GetColor(i, j));
				}
				//right wall merge
				if (Main.tile[i + 1, j].WallType == ModContent.WallType<CatacombBrickWall1>())
				{
					spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 3, 0, 16, 16), Lighting.GetColor(i, j));
				}
			}
		}
    }

    public class CatacombGrassWall2 : CatacombGrassWall1
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombGrassWall";

        private static Asset<Texture2D> MergeTexture;
        private static Asset<Texture2D> LeafTexture;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Catacomb/CatacombGrassWall2Merge");
            LeafTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/CemeteryGrassWallLeaf");

			Vector2 pos = TileGlobal.TileCustomPosition(i, j);

			//do not draw any fancy grass on the wall if its merged with a surrounding wall
			if (Main.tile[i, j + 1].WallType != ModContent.WallType<CatacombBrickWall2>() && Main.tile[i - 1, j].WallType != ModContent.WallType<CatacombBrickWall2>() &&
			Main.tile[i, j - 1].WallType != ModContent.WallType<CatacombBrickWall2>() && Main.tile[i + 1, j].WallType != ModContent.WallType<CatacombBrickWall2>())
			{
				if (i > Main.screenPosition.X / 16 && i < Main.screenPosition.X / 16 + Main.screenWidth / 16 && j > Main.screenPosition.Y / 16 && j < Main.screenPosition.Y / 16 + Main.screenHeight / 16)
				{
					var rand = new Random(i + (j * 100000));

					float offset = i * j % 6.28f + (float)rand.NextDouble() / 8f;
					float sin = (float)Math.Sin(Main.GameUpdateCount / 45f + offset);

					spriteBatch.Draw(LeafTexture.Value, pos + new Vector2(6, 6) + new Vector2(1, 0.5f) * sin * 2.2f,
					new Rectangle(rand.Next(6) * 18, 0, 16, 16), Lighting.GetColor(i, j), sin * 0.09f, new Vector2(3, 6), 1 + sin / 14f, 0, 0);
				}
            }
			else
			{
				//down wall merge
				if (Main.tile[i, j + 1].WallType == ModContent.WallType<CatacombBrickWall2>())
				{
					spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 0, 0, 16, 16), Lighting.GetColor(i, j));
				}
				//left wall merge
				if (Main.tile[i - 1, j].WallType == ModContent.WallType<CatacombBrickWall2>())
				{
					spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 1, 0, 16, 16), Lighting.GetColor(i, j));
				}
				//up wall merge
				if (Main.tile[i, j - 1].WallType == ModContent.WallType<CatacombBrickWall2>())
				{
					spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 2, 0, 16, 16), Lighting.GetColor(i, j));
				}
				//right wall merge
				if (Main.tile[i + 1, j].WallType == ModContent.WallType<CatacombBrickWall2>())
				{
					spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 3, 0, 16, 16), Lighting.GetColor(i, j));
				}
			}
        }
    }
}