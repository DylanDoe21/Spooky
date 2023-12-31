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

    /*
    public class CatacombBrickWallBG1 : CatacombBrickWall1 
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrickWall1";

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int width = 8;
            int height = 6;
            int parallax = 10;
            Texture2D texture = ModContent.Request<Texture2D>(("Spooky/Content/Backgrounds/Catacomb/UpperCatacombBG"), (AssetRequestMode)2).Value;
            Vector2 zero = (Vector2)(Main.drawToScreen ? Vector2.Zero : new Vector2((float)Main.offScreenRange));
            Rectangle frame = new((int)((i * 16) - Main.screenPosition.X / (float)parallax) % (width * 16), (int)((float)(j * 16) - Main.screenPosition.Y / (float)parallax) % (height * 16), 16, 16);
            Main.spriteBatch.Draw(texture, new Vector2((float)(i * 16 - (int)Main.screenPosition.X), (float)(j * 16 - (int)Main.screenPosition.Y)) + zero, (Rectangle?)frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, (SpriteEffects)0, 0f);
            return false;
        }
    }

    public class CatacombBrickWallDaffodilBG : CatacombBrickWall1 
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrickWall1";

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int width = 8;
            int height = 6;
            int parallax = 10;
            Texture2D texture = ModContent.Request<Texture2D>(("Spooky/Content/Backgrounds/Catacomb/UpperCatacombBG"), (AssetRequestMode)2).Value;
            Vector2 zero = (Vector2)(Main.drawToScreen ? Vector2.Zero : new Vector2((float)Main.offScreenRange));
            Rectangle frame = new((int)((i * 16) - Main.screenPosition.X / (float)parallax) % (width * 16), (int)((float)(j * 16) - Main.screenPosition.Y / (float)parallax) % (height * 16), 16, 16);
            Main.spriteBatch.Draw(texture, new Vector2((float)(i * 16 - (int)Main.screenPosition.X), (float)(j * 16 - (int)Main.screenPosition.Y)) + zero, (Rectangle?)frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, (SpriteEffects)0, 0f);
            return false;
        }
    }
    */
}