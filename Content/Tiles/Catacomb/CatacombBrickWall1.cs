using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Catacomb
{
    public class CatacombBrickWall1 : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(29, 24, 35));
            DustType = DustID.Stone;
        }

        public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }
    }

    public class CatacombBrickWallDaffodilBG : CatacombBrickWall1 
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrickWall1";

        private Asset<Texture2D> BGTexture;

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.One * 16 : Vector2.One * 12;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //wall background
            BGTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Catacomb/DaffodilArenaBG");

            float XParallax = (Main.LocalPlayer.Center.X / 16 - i) * 0.025f;
            float YParallax = (Main.LocalPlayer.Center.Y / 16 - j) * 0.025f;

            Vector2 DrawPosition = (new Vector2(i, j) - new Vector2((1454 / 2) / 16, (576 / 2) / 16) + TileOffset) * 16 - Main.screenPosition;
            Vector2 DrawPositionParallax = (new Vector2(i, j) - new Vector2((1454 / 2) / 16 + XParallax, (576 / 2) / 16 + YParallax) + TileOffset) * 16 - Main.screenPosition;
            
            spriteBatch.Draw(BGTexture.Value, DrawPosition, new Rectangle(0, 0, 1454, 576), new Color(47, 59, 55));
            spriteBatch.Draw(BGTexture.Value, DrawPositionParallax, new Rectangle(0, 0, 1454, 576), new Color(47, 59, 55));

            for (int X = i * 16 - 700; X <= i * 16 + 700; X += 20)
            {
                for (int Y = j * 16 - 300; Y <= j * 16 + 300; Y += 20)
                {
                    if (!Main.tile[X / 16, Y / 16].HasTile)
                    {
                        Lighting.AddLight(new Vector2(X, Y), new Vector3(0.47f, 0.59f, 0.55f));
                    }
                }
            }
        }
    }

    public class CatacombBrickWall1Safe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrickWall1";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(29, 24, 35));
            DustType = DustID.Stone;
        }
    }
}