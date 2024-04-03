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

        public static Vector2 TileOffset => Lighting.LegacyEngine.Mode > 1 && Main.GameZoomTarget == 1 ? Vector2.Zero : Vector2.One * 12;

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //wall merges
            Texture2D Background = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Catacomb/DaffodilArenaBG").Value;

            float XParallax = (Main.LocalPlayer.Center.X / 16 - i) * 0.025f;
            float YParallax = (Main.LocalPlayer.Center.Y / 16 - j) * 0.025f;

            Vector2 DrawPosition = (new Vector2(i, j) - new Vector2((1454 / 2) / 16, (576 / 2) / 16) + TileOffset) * 16 - Main.screenPosition;
            Vector2 DrawPositionParallax = (new Vector2(i, j) - new Vector2((1454 / 2) / 16 + XParallax, (576 / 2) / 16 + YParallax) + TileOffset) * 16 - Main.screenPosition;
            
            spriteBatch.Draw(Background, DrawPosition, new Rectangle(1456 * 0, 0, 1454, 576), new Color(70, 46, 46));
            spriteBatch.Draw(Background, DrawPositionParallax, new Rectangle(1456 * 0, 0, 1454, 576), new Color(70, 46, 46));

            //new Vector2((1454 / 2) / 16, (576 / 2) / 16)

            /*
            //wall merges
            Texture2D Background = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Catacomb/DaffodilArenaBG").Value;

            Vector2 drawOrigin = (new Vector2(i, j) - new Vector2((1454 / 2) / 16, (576 / 2) / 16) + TileOffset) * 16 - Main.screenPosition;
            Vector2 drawPos = (new Vector2(i, j) - new Vector2((1454 / 2) / 16, (576 / 2) / 16) + TileOffset) * 16 - Main.screenPosition;

            float XParallax = (Main.LocalPlayer.Center.X / 16 - drawOrigin.X) * 0.015f;
            float YParallax = (Main.LocalPlayer.Center.Y / 16 - drawOrigin.Y) * 0.015f;

            drawPos.X += XParallax;
            drawPos.Y += YParallax;
            
            spriteBatch.Draw(Background, drawPos, new Rectangle(1456 * 0, 0, 1454, 576), new Color(80, 80, 80));
            */
        }
    }
}