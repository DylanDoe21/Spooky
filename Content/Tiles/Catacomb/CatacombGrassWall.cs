using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.Catacomb
{
    public class CatacombGrassWall1 : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombGrassWall";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(2, 42, 0));
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
        }

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //wall merges
            Texture2D mergeTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Catacomb/CatacombGrassWall1Merge").Value;

            //down wall merge
            if (Main.tile[i, j + 1].WallType == ModContent.WallType<CatacombBrickWall1>() || Main.tile[i - 1, j].WallType == ModContent.WallType<CatacombBrickWall1>() ||
            Main.tile[i, j - 1].WallType == ModContent.WallType<CatacombBrickWall1>() || Main.tile[i + 1, j].WallType == ModContent.WallType<CatacombBrickWall1>())
            { 
                if (Main.tile[i, j + 1].WallType == ModContent.WallType<CatacombBrickWall1>())
                {
                    spriteBatch.Draw(mergeTex, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 0, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //left wall merge
                if (Main.tile[i - 1, j].WallType == ModContent.WallType<CatacombBrickWall1>())
                {
                    spriteBatch.Draw(mergeTex, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 1, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //up wall merge
                if (Main.tile[i, j - 1].WallType == ModContent.WallType<CatacombBrickWall1>())
                {
                    spriteBatch.Draw(mergeTex, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 2, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //right wall merge
                if (Main.tile[i + 1, j].WallType == ModContent.WallType<CatacombBrickWall1>())
                {
                    spriteBatch.Draw(mergeTex, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 3, 0, 16, 16), Lighting.GetColor(i, j));
                }
            }
            //do not draw any fancy grass on the wall if its merged with a surrounding wall
            else
            {
                if (i > Main.screenPosition.X / 16 && i < Main.screenPosition.X / 16 + Main.screenWidth / 16 && j > Main.screenPosition.Y / 16 && j < Main.screenPosition.Y / 16 + Main.screenHeight / 16)
                {
                    Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/CemeteryGrassWallLeaf").Value;
                    var rand = new Random(i + (j * 100000));

                    float offset = i * j % 6.28f + (float)rand.NextDouble() / 8f;
                    float sin = (float)Math.Sin(Main.GameUpdateCount / 45f + offset);

                    spriteBatch.Draw(tex, (new Vector2(i + 0.5f, j + 0.5f) + TileOffset) * 16 + new Vector2(1, 0.5f) * sin * 2.2f - Main.screenPosition,
                    new Rectangle(rand.Next(6) * 18, 0, 16, 16), Lighting.GetColor(i, j), offset + sin * 0.09f, new Vector2(12, 12), 1 + sin / 14f, 0, 0);
                }
            }
        }
    }

    public class CatacombGrassWall2 : CatacombGrassWall1
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombGrassWall";

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //wall merges
            Texture2D mergeTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Catacomb/CatacombGrassWall2Merge").Value;

            //down wall merge
            if (Main.tile[i, j + 1].WallType == ModContent.WallType<CatacombBrickWall2>() || Main.tile[i - 1, j].WallType == ModContent.WallType<CatacombBrickWall2>() ||
            Main.tile[i, j - 1].WallType == ModContent.WallType<CatacombBrickWall2>() || Main.tile[i + 1, j].WallType == ModContent.WallType<CatacombBrickWall2>())
            { 
                if (Main.tile[i, j + 1].WallType == ModContent.WallType<CatacombBrickWall2>())
                {
                    spriteBatch.Draw(mergeTex, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 0, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //left wall merge
                if (Main.tile[i - 1, j].WallType == ModContent.WallType<CatacombBrickWall2>())
                {
                    spriteBatch.Draw(mergeTex, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 1, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //up wall merge
                if (Main.tile[i, j - 1].WallType == ModContent.WallType<CatacombBrickWall2>())
                {
                    spriteBatch.Draw(mergeTex, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 2, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //right wall merge
                if (Main.tile[i + 1, j].WallType == ModContent.WallType<CatacombBrickWall2>())
                {
                    spriteBatch.Draw(mergeTex, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 3, 0, 16, 16), Lighting.GetColor(i, j));
                }
            }
            //do not draw any fancy grass on the wall if its merged with a surrounding wall
            else
            {
                if (i > Main.screenPosition.X / 16 && i < Main.screenPosition.X / 16 + Main.screenWidth / 16 && j > Main.screenPosition.Y / 16 && j < Main.screenPosition.Y / 16 + Main.screenHeight / 16)
                {
                    Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/CemeteryGrassWallLeaf").Value;
                    var rand = new Random(i + (j * 100000));

                    float offset = i * j % 6.28f + (float)rand.NextDouble() / 8f;
                    float sin = (float)Math.Sin(Main.GameUpdateCount / 45f + offset);

                    spriteBatch.Draw(tex, (new Vector2(i + 0.5f, j + 0.5f) + TileOffset) * 16 + new Vector2(1, 0.5f) * sin * 2.2f - Main.screenPosition,
                    new Rectangle(rand.Next(6) * 18, 0, 16, 16), Lighting.GetColor(i, j), offset + sin * 0.09f, new Vector2(12, 12), 1 + sin / 14f, 0, 0);
                }
            }
        }
    }
}