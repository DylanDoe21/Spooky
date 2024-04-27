using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.SpookyHell.NoseTemple
{
    [LegacyName("NoseTempleWallBG")]
    public class NoseTempleWallBGGreen : ModWall 
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(0, 0, 0));
            DustType = DustID.Stone;
        }

        public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override void KillWall(int i, int j, ref bool fail)
        {
            //fail = true;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int width = 13;
            int height = 8;
            int parallax = 8;
            Texture2D texture = ModContent.Request<Texture2D>(("Spooky/Content/Backgrounds/SpookyHell/NoseDungeonWallBGGreen")).Value;
            Vector2 zero = (Vector2)(Main.drawToScreen ? Vector2.Zero : new Vector2((float)Main.offScreenRange));
            Rectangle frame = new((int)((i * 16) - Main.screenPosition.X / (float)parallax) % ((int)width * 16), (int)((float)(j * 16) - Main.screenPosition.Y / (float)parallax) % ((int)height * 16), 16, 16);
            Main.spriteBatch.Draw(texture, new Vector2((float)(i * 16 - (int)Main.screenPosition.X), (float)(j * 16 - (int)Main.screenPosition.Y)) + zero, (Rectangle?)frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, (SpriteEffects)0, 0f);
            
            return false;
        }
    }

    public class NoseTempleWallBGPurple : NoseTempleWallBGGreen 
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(0, 0, 0));
            DustType = DustID.Stone;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int width = 13;
            int height = 8;
            int parallax = 8;
            Texture2D texture = ModContent.Request<Texture2D>(("Spooky/Content/Backgrounds/SpookyHell/NoseDungeonWallBGPurple")).Value;
            Vector2 zero = (Vector2)(Main.drawToScreen ? Vector2.Zero : new Vector2((float)Main.offScreenRange));
            Rectangle frame = new((int)((i * 16) - Main.screenPosition.X / (float)parallax) % ((int)width * 16), (int)((float)(j * 16) - Main.screenPosition.Y / (float)parallax) % ((int)height * 16), 16, 16);
            Main.spriteBatch.Draw(texture, new Vector2((float)(i * 16 - (int)Main.screenPosition.X), (float)(j * 16 - (int)Main.screenPosition.Y)) + zero, (Rectangle?)frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, (SpriteEffects)0, 0f);
            
            return false;
        }
    }

    public class NoseTempleWallBGRed : NoseTempleWallBGGreen 
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(0, 0, 0));
            DustType = DustID.Stone;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int width = 13;
            int height = 8;
            int parallax = 8;
            Texture2D texture = ModContent.Request<Texture2D>(("Spooky/Content/Backgrounds/SpookyHell/NoseDungeonWallBGRed")).Value;
            Vector2 zero = (Vector2)(Main.drawToScreen ? Vector2.Zero : new Vector2((float)Main.offScreenRange));
            Rectangle frame = new((int)((i * 16) - Main.screenPosition.X / (float)parallax) % ((int)width * 16), (int)((float)(j * 16) - Main.screenPosition.Y / (float)parallax) % ((int)height * 16), 16, 16);
            Main.spriteBatch.Draw(texture, new Vector2((float)(i * 16 - (int)Main.screenPosition.X), (float)(j * 16 - (int)Main.screenPosition.Y)) + zero, (Rectangle?)frame, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, (SpriteEffects)0, 0f);
            
            return false;
        }
    }
}