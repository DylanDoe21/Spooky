using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpiderCave
{
    public class DampGrassWall : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(13, 55, 10));
            DustType = ModContent.DustType<DampGrassDust>();
            HitSound = SoundID.Grass;
        }
        
        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i, j + 1].WallType > 0 || Main.tile[i - 1, j].WallType > 0 || Main.tile[i, j - 1].WallType > 0 || Main.tile[i + 1, j].WallType > 0)
            {
                if (i > Main.screenPosition.X / 16 && i < Main.screenPosition.X / 16 + Main.screenWidth / 16 && j > Main.screenPosition.Y / 16 && j < Main.screenPosition.Y / 16 + Main.screenHeight / 16)
                {
                    Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpiderCave/DampGrassWallLeaf").Value;
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