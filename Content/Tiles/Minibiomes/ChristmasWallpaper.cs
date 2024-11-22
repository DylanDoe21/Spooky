using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.Minibiomes
{
    public class ChristmasWallpaperBlue : ModWall 
    {
        private static Asset<Texture2D> MergeTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(57, 55, 82));
            DustType = -1;
        }

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/ChristmasWallpaperMerge");

            //wall merges
            if (Main.tile[i, j + 1].WallType == ModContent.WallType<ChristmasBrickWall>() || Main.tile[i - 1, j].WallType == ModContent.WallType<ChristmasBrickWall>() ||
            Main.tile[i, j - 1].WallType == ModContent.WallType<ChristmasBrickWall>() || Main.tile[i + 1, j].WallType == ModContent.WallType<ChristmasBrickWall>())
            {
                //down wall merge
                if (Main.tile[i, j + 1].WallType == ModContent.WallType<ChristmasBrickWall>())
                {
                    spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 0, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //left wall merge
                if (Main.tile[i - 1, j].WallType == ModContent.WallType<ChristmasBrickWall>())
                {
                    spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 1, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //up wall merge
                if (Main.tile[i, j - 1].WallType == ModContent.WallType<ChristmasBrickWall>())
                {
                    spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 2, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //right wall merge
                if (Main.tile[i + 1, j].WallType == ModContent.WallType<ChristmasBrickWall>())
                {
                    spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 3, 0, 16, 16), Lighting.GetColor(i, j));
                }
            }
        }
    }

    public class ChristmasWallpaperGreen : ModWall 
    {
        private static Asset<Texture2D> MergeTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(61, 77, 55));
            DustType = -1;
        }

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/ChristmasWallpaperMerge");

            //wall merges
            if (Main.tile[i, j + 1].WallType == ModContent.WallType<ChristmasBrickWall>() || Main.tile[i - 1, j].WallType == ModContent.WallType<ChristmasBrickWall>() ||
            Main.tile[i, j - 1].WallType == ModContent.WallType<ChristmasBrickWall>() || Main.tile[i + 1, j].WallType == ModContent.WallType<ChristmasBrickWall>())
            {
                //down wall merge
                if (Main.tile[i, j + 1].WallType == ModContent.WallType<ChristmasBrickWall>())
                {
                    spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 0, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //left wall merge
                if (Main.tile[i - 1, j].WallType == ModContent.WallType<ChristmasBrickWall>())
                {
                    spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 1, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //up wall merge
                if (Main.tile[i, j - 1].WallType == ModContent.WallType<ChristmasBrickWall>())
                {
                    spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 2, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //right wall merge
                if (Main.tile[i + 1, j].WallType == ModContent.WallType<ChristmasBrickWall>())
                {
                    spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 3, 0, 16, 16), Lighting.GetColor(i, j));
                }
            }
        }
    }

    public class ChristmasWallpaperRed : ModWall 
    {
        private static Asset<Texture2D> MergeTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(102, 58, 58));
            DustType = -1;
        }

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/ChristmasWallpaperMerge");

            //wall merges
            if (Main.tile[i, j + 1].WallType == ModContent.WallType<ChristmasBrickWall>() || Main.tile[i - 1, j].WallType == ModContent.WallType<ChristmasBrickWall>() ||
            Main.tile[i, j - 1].WallType == ModContent.WallType<ChristmasBrickWall>() || Main.tile[i + 1, j].WallType == ModContent.WallType<ChristmasBrickWall>())
            {
                //down wall merge
                if (Main.tile[i, j + 1].WallType == ModContent.WallType<ChristmasBrickWall>())
                {
                    spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 0, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //left wall merge
                if (Main.tile[i - 1, j].WallType == ModContent.WallType<ChristmasBrickWall>())
                {
                    spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 1, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //up wall merge
                if (Main.tile[i, j - 1].WallType == ModContent.WallType<ChristmasBrickWall>())
                {
                    spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 2, 0, 16, 16), Lighting.GetColor(i, j));
                }
                //right wall merge
                if (Main.tile[i + 1, j].WallType == ModContent.WallType<ChristmasBrickWall>())
                {
                    spriteBatch.Draw(MergeTexture.Value, (new Vector2(i, j) + TileOffset) * 16 - Main.screenPosition, new Rectangle(18 * 3, 0, 16, 16), Lighting.GetColor(i, j));
                }
            }
        }
    }
}