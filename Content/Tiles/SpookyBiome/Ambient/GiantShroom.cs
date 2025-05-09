using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class GiantShroom1 : ModTile
    {
		private Asset<Texture2D> CapTexture;

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
            AddMapEntry(new Color(196, 188, 217));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.25f;
            g = 0.15f;
            b = 0.25f;
        }

        public static void DrawMushroomCap(int i, int j, Texture2D tex, Rectangle? source, Vector2 scaleVec, Vector2? offset = null, Vector2? origin = null)
        {
            float cos = Main.GlobalTimeWrappedHourly * 0.08971428571f * 16;
            scaleVec = new Vector2(1f, -MathF.Cos(-i / 8f + cos));

            Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));
            Color color = TileGlobal.GetTileColorWithPaint(i + 1, j + 1, Lighting.GetColor(i + 1, j + 1));

            Main.spriteBatch.Draw(tex, drawPos, source, color, 0, origin ?? source.Value.Size() / 3f, 1f * (Vector2.One + (0.1f * scaleVec)), SpriteEffects.None, 0f);
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
			CapTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Ambient/GiantShroom1Cap");

            //draw the mushroom cap, only draw it on the very first frame of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
				//reminder: offset negative numbers are right and down, while positive is left and up
				Vector2 offset = new Vector2((CapTexture.Width() / 3) - 4, CapTexture.Height() - 24);

				DrawMushroomCap(i - 1, j - 1, CapTexture.Value, new Rectangle(0, 0, 52, 26), default, TileGlobal.TileOffset, offset);
            }
        }
    }

    public class GiantShroom2 : GiantShroom1
    {
		private Asset<Texture2D> CapTexture;

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
            AddMapEntry(new Color(196, 188, 217));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
			CapTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Ambient/GiantShroom2Cap");

            //draw the mushroom cap, only draw it on the very first frame of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
				Vector2 offset = new Vector2((CapTexture.Width() / 4) - 5, CapTexture.Height() - 27);

				DrawMushroomCap(i - 1, j - 1, CapTexture.Value, new Rectangle(0, 0, 40, 20), default, TileGlobal.TileOffset, offset);
            }
        }
    }

    public class GiantShroom3 : GiantShroom1
    {
		private Asset<Texture2D> CapTexture;

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
            AddMapEntry(new Color(196, 188, 217));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
			CapTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Ambient/GiantShroom3Cap");

            //draw the mushroom cap, only draw it on the very first frame of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 18 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
				Vector2 offset = new Vector2((CapTexture.Width() / 3) + 4, CapTexture.Height() - 24);

				DrawMushroomCap(i - 1, j - 1, CapTexture.Value, new Rectangle(0, 0, 62, 30), default, TileGlobal.TileOffset, offset);
            }
        }
    }

    public class GiantShroom4 : GiantShroom1
    {
		private Asset<Texture2D> CapTexture;

		public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.DrawYOffset = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(196, 188, 217));
            DustType = DustID.Slush;
            HitSound = SoundID.Dig;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
			CapTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Ambient/GiantShroom4Cap");

            //draw the mushroom cap, only draw it on the very first frame of the tile so it only draws once
            if (Framing.GetTileSafely(i, j).TileFrameX == 36 && Framing.GetTileSafely(i, j).TileFrameY == 0)
            {
				Vector2 offset = new Vector2((CapTexture.Width() / 2), CapTexture.Height() - 18);

				DrawMushroomCap(i - 1, j - 1, CapTexture.Value, new Rectangle(0, 0, 84, 38), default, TileGlobal.TileOffset, offset);
            }
        }
    }
}