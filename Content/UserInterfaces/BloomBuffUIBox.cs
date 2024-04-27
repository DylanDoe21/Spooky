using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.UserInterfaces
{
    public class BloomBuffUIBox
    {
        public static bool UIOpen = false;

        public static readonly Vector2 UITopLeft = new Vector2(Main.screenWidth / 2 - 146f, 100f);

        public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);

        public static void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            if (!UIOpen)
            {
                return;
            }

            Texture2D UIBoxTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffUIBox").Value;
            Vector2 UIBoxScale = Vector2.One * Main.UIScale;

            //draw the main UI box
            spriteBatch.Draw(UIBoxTexture, UITopLeft, null, Color.White, 0f, Vector2.Zero, UIBoxScale, SpriteEffects.None, 0f);

            //bloom buff slot 1
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[0] != string.Empty)
            {
                DrawIcon(spriteBatch, player, 0);
            }
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[1] != string.Empty)
            {
                DrawIcon(spriteBatch, player, 1);
            }
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[2] != string.Empty)
            {
                DrawIcon(spriteBatch, player, 2);
            }
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[3] != string.Empty)
            {
                DrawIcon(spriteBatch, player, 3);
            }
        }

        //used to draw individual icons over the main UI box
        public static void DrawIcon(SpriteBatch spriteBatch, Player player, int SlotToCheckFor)
        {
            Vector2 IconTopLeft = UITopLeft + new Vector2(0f, 0f) * Main.UIScale;

            //TODO: modify these so each box draws perfectly over the UI
            switch (SlotToCheckFor)
            {
                case 0:
                {
                    IconTopLeft = UITopLeft + new Vector2(6f, 6f) * Main.UIScale;
                    break;
                }
                case 1:
                {
                    IconTopLeft = UITopLeft + new Vector2(40f, 6f) * Main.UIScale;
                    break;
                }
                case 2:
                {
                    IconTopLeft = UITopLeft + new Vector2(80f, 6f) * Main.UIScale;
                    break;
                }
                case 3:
                {
                    IconTopLeft = UITopLeft + new Vector2(120f, 6f) * Main.UIScale;
                    break;
                }
            }

            Texture2D IconTexture = null;

            //TODO: add icon textures as they added and their respective string values
            switch (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[SlotToCheckFor])
            {
                case "SunHappyFlower": IconTexture = ModContent.Request<Texture2D>("Spooky/Content/Buffs/BloomBuffIcons/BloomBuff1").Value; break;
                case "SunLemon": IconTexture = ModContent.Request<Texture2D>("Spooky/Content/Buffs/BloomBuffIcons/BloomBuff2").Value; break;
                case "SunOrange": IconTexture = ModContent.Request<Texture2D>("Spooky/Content/Buffs/BloomBuffIcons/BloomBuff3").Value; break;
                case "SunPineapple": IconTexture = ModContent.Request<Texture2D>("Spooky/Content/Buffs/BloomBuffIcons/BloomBuff4").Value; break;
            }

            if (IconTexture != null)
            {
                spriteBatch.Draw(IconTexture, IconTopLeft, null, Color.White, 0f, Vector2.Zero, Main.UIScale, SpriteEffects.None, 0f);

                if (IsMouseOverUI((int)UITopLeft.X, (int)UITopLeft.Y, IconTexture, Vector2.One * Main.UIScale))
                {
                    Main.instance.MouseText(player.GetModPlayer<BloomBuffsPlayer>().Duration1.ToString());
                }
            }
        }

        //check if the mouse is hovering over a specific button or UI box
        public static bool IsMouseOverUI(int TopLeft, int TopRight, Texture2D texture, Vector2 backgroundScale)
        {
            Rectangle backgroundArea = new Rectangle(TopLeft, TopRight, (int)(texture.Width * backgroundScale.X), (int)(texture.Width * backgroundScale.Y));

            if (MouseScreenArea.Intersects(backgroundArea))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}