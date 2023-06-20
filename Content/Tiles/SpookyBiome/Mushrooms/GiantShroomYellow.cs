using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.SpookyBiome.Mushrooms
{
    public class GiantShroomYellow1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.DrawYOffset = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(208, 162, 44));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.25f;
            g = 0.2f;
            b = 0.0f;
        }

        public static Vector2 TileOffset => Vector2.One * 12;

        public static Vector2 TileCustomPosition(int i, int j, Vector2? off = null)
        {
            return ((new Vector2(i, j) + TileOffset) * 16) - Main.screenPosition - (off ?? new Vector2(0, -2));
        }

        public static void DrawMushroomCap(int i, int j, Texture2D tex, Rectangle? source, Vector2 scaleVec, Vector2? offset = null, Vector2? origin = null)
        {
            float sin = Main.GlobalTimeWrappedHourly * 0.08971428571f * 16;
            scaleVec = new Vector2(1f, -MathF.Sin(sin));

            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
            Color color = Lighting.GetColor(i, j);

            Main.spriteBatch.Draw(tex, drawPos, source, color, 0, origin ?? source.Value.Size() / 3f, 1f * (Vector2.One + (0.1f * scaleVec)), SpriteEffects.None, 0f);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Color col = Lighting.GetColor(i, j);
            float xOff = (float)Math.Sin((j * 19) * 0.04f) * 1.2f;

            if (xOff == 1 && (j / 4f) == 0)
            {
                xOff = 0;
            }

            int frameSize = 16;
            int frameOff = 0;
            int frameSizeY = 16;

            Vector2 pos = TileCustomPosition(i, j);

            Texture2D capTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Mushrooms/GiantShroomYellow1Cap").Value;

            //draw the mushroom cap, only draw it on the top of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
                Vector2 capOffset = new Vector2(6, 6);

                DrawMushroomCap(i - 1, j - 1, capTex, new Rectangle(0, 0, 26, 20), default, TileOffset.ToWorldCoordinates(), capOffset);
            }
        }
    }

    public class GiantShroomYellow2 : GiantShroomYellow1
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(208, 162, 44));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Color col = Lighting.GetColor(i, j);
            float xOff = (float)Math.Sin((j * 19) * 0.04f) * 1.2f;

            if (xOff == 1 && (j / 4f) == 0)
            {
                xOff = 0;
            }

            int frameSize = 16;
            int frameOff = 0;
            int frameSizeY = 16;

            Vector2 pos = TileCustomPosition(i, j);

            Texture2D capTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Mushrooms/GiantShroomYellow2Cap").Value;

            //draw the mushroom cap, only draw it on the top of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
                Vector2 capOffset = new Vector2(18, 8);

                DrawMushroomCap(i - 1, j - 1, capTex, new Rectangle(0, 0, 52, 26), default, TileOffset.ToWorldCoordinates(), capOffset);
            }
        }
    }

    public class GiantShroomYellow3 : GiantShroomYellow1
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.DrawYOffset = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(208, 162, 44));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Color col = Lighting.GetColor(i, j);
            float xOff = (float)Math.Sin((j * 19) * 0.04f) * 1.2f;

            if (xOff == 1 && (j / 4f) == 0)
            {
                xOff = 0;
            }

            int frameSize = 16;
            int frameOff = 0;
            int frameSizeY = 16;

            Vector2 pos = TileCustomPosition(i, j);

            Texture2D capTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Mushrooms/GiantShroomYellow3Cap").Value;

            //draw the mushroom cap, only draw it on the top of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 18 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
                Vector2 capOffset = new Vector2(32, 14);

                DrawMushroomCap(i - 1, j - 1, capTex, new Rectangle(0, 0, 62, 30), default, TileOffset.ToWorldCoordinates(), capOffset);
            }
        }
    }

    public class GiantShroomYellow4 : GiantShroomYellow1
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.DrawYOffset = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(208, 162, 44));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Color col = Lighting.GetColor(i, j);
            float xOff = (float)Math.Sin((j * 19) * 0.04f) * 1.2f;

            if (xOff == 1 && (j / 4f) == 0)
            {
                xOff = 0;
            }

            int frameSize = 16;
            int frameOff = 0;
            int frameSizeY = 16;

            Vector2 pos = TileCustomPosition(i, j);

            Texture2D capTex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Mushrooms/GiantShroomYellow4Cap").Value;

            //draw the mushroom cap, only draw it on the top of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 36 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
                Vector2 capOffset = new Vector2(68, 38);

                DrawMushroomCap(i - 1, j - 1, capTex, new Rectangle(0, 0, 122, 46), default, TileOffset.ToWorldCoordinates(), capOffset);
            }
        }
    }
}