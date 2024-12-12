using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Tiles.Cemetery
{
    public class CemeteryGrassWall : ModWall 
    {
        private static Asset<Texture2D> LeafTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(12, 62, 6));
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            LeafTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/CemeteryGrassWallLeaf");

            Vector2 pos = TileGlobal.TileCustomPosition(i, j);

            if (i > Main.screenPosition.X / 16 && i < Main.screenPosition.X / 16 + Main.screenWidth / 16 && j > Main.screenPosition.Y / 16 && j < Main.screenPosition.Y / 16 + Main.screenHeight / 16)
            {
                var rand = new Random(i + (j * 100000));

                float offset = i * j % 6.28f + (float)rand.NextDouble() / 8f;
                float sin = (float)Math.Sin(Main.GameUpdateCount / 45f + offset);

                spriteBatch.Draw(LeafTexture.Value, pos + new Vector2(6, 6) + new Vector2(1, 0.5f) * sin * 2.2f,
                new Rectangle(rand.Next(6) * 18, 0, 16, 16), Lighting.GetColor(i, j), sin * 0.09f, new Vector2(3, 6), 1 + sin / 14f, 0, 0);
            }
        }
    }

    public class CemeteryGrassWallSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Cemetery/CemeteryGrassWall";

        private static Asset<Texture2D> LeafTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(12, 62, 6));
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            LeafTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Cemetery/CemeteryGrassWallLeaf");

            Vector2 pos = TileGlobal.TileCustomPosition(i, j);

            if (i > Main.screenPosition.X / 16 && i < Main.screenPosition.X / 16 + Main.screenWidth / 16 && j > Main.screenPosition.Y / 16 && j < Main.screenPosition.Y / 16 + Main.screenHeight / 16)
            {
                var rand = new Random(i + (j * 100000));

                float offset = i * j % 6.28f + (float)rand.NextDouble() / 8f;
                float sin = (float)Math.Sin(Main.GameUpdateCount / 45f + offset);

                spriteBatch.Draw(LeafTexture.Value, pos + new Vector2(6, 6) + new Vector2(1, 0.5f) * sin * 2.2f,
                new Rectangle(rand.Next(6) * 18, 0, 16, 16), Lighting.GetColor(i, j), sin * 0.09f, new Vector2(3, 6), 1 + sin / 14f, 0, 0);
            }
        }
    }
}