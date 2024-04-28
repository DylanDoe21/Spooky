using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.UserInterfaces
{
    public class BloomBuffUI
    {
        public static bool UIOpen = false;

        public static float Transparency = 0f;

        public static readonly Vector2 UITopLeft = new Vector2(Main.screenWidth / 2 - 116f, 75f);

        public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);

        public static void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            //dont draw this ui if its transparent
            if (Transparency <= 0f)
            {
                return;
            }

            Texture2D UIBoxTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffUIBox").Value;
            Vector2 UIBoxScale = Vector2.One * Main.UIScale;

            //draw the main UI box
            spriteBatch.Draw(UIBoxTexture, UITopLeft, null, Color.White * Transparency, 0f, Vector2.Zero, UIBoxScale, SpriteEffects.None, 0f);

            //bloom buff icon drawing for each slot
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[0] != string.Empty)
            {
                DrawIcon(spriteBatch, player, UITopLeft + new Vector2(22f, 34f) * Main.UIScale, 0, player.GetModPlayer<BloomBuffsPlayer>().Duration1);
            }
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[1] != string.Empty)
            {
                DrawIcon(spriteBatch, player, UITopLeft + new Vector2(70f, 34f) * Main.UIScale, 1, player.GetModPlayer<BloomBuffsPlayer>().Duration2);
            }
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[2] != string.Empty)
            {
                DrawIcon(spriteBatch, player, UITopLeft + new Vector2(118f, 34f) * Main.UIScale, 2, player.GetModPlayer<BloomBuffsPlayer>().Duration3);
            }
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[3] != string.Empty)
            {
                DrawIcon(spriteBatch, player, UITopLeft + new Vector2(166f, 34f) * Main.UIScale, 3, player.GetModPlayer<BloomBuffsPlayer>().Duration4);
            }

            //draw locked icons
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[2] == string.Empty && !player.GetModPlayer<BloomBuffsPlayer>().UnlockedSlot3)
            {
                DrawIcon(spriteBatch, player, UITopLeft + new Vector2(118f, 34f) * Main.UIScale, 2, player.GetModPlayer<BloomBuffsPlayer>().Duration3);
            }
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[3] == string.Empty && !player.GetModPlayer<BloomBuffsPlayer>().UnlockedSlot4)
            {
                DrawIcon(spriteBatch, player, UITopLeft + new Vector2(166f, 34f) * Main.UIScale, 3, player.GetModPlayer<BloomBuffsPlayer>().Duration4);
            }
        }

        //used to draw individual buff icons over the main UI box
        public static void DrawIcon(SpriteBatch spriteBatch, Player player, Vector2 IconTopLeft, int SlotToCheckFor, int DurationToCheckFor)
        {
            Texture2D IconTexture = null;

            string BuffDisplayName = string.Empty;

            switch (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[SlotToCheckFor])
            {
                case "WinterBlackberry": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/WinterBlackberryIcon").Value;
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.WinterBlackberry.DisplayName");
                    break;
                case "WinterBlueberry": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/WinterBlueberryIcon").Value;
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.WinterBlueberry.DisplayName");
                    break;
                case "WinterGooseberry": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/WinterGooseberryIcon").Value;
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.WinterGooseberry.DisplayName");
                    break;
                case "WinterStrawberry": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/WinterStrawberryIcon").Value;
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.WinterStrawberry.DisplayName");
                    break;
                case "SummerLemon": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/SummerLemonIcon").Value; 
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.SummerLemon.DisplayName");
                    break;
                case "SummerOrange": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/SummerOrangeIcon").Value;
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.SummerOrange.DisplayName");
                    break;
                case "SummerPineapple": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/SummerPineappleIcon").Value;
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.SummerPineapple.DisplayName");
                    break;
                case "SummerSunflower":
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/SummerSunflowerIcon").Value;
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.SummerSunflower.DisplayName");
                    break;
                case "Dragonfruit": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/DragonfruitIcon").Value; 
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.Dragonfruit.DisplayName");
                    break;
            }

            if (!player.GetModPlayer<BloomBuffsPlayer>().UnlockedSlot3 && SlotToCheckFor == 2)
            {
                IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/BloomBuffSlotLocked").Value; 
                BuffDisplayName = Language.GetTextValue("Mods.Spooky.UI.BloomSlotLocked");
            }

            if (!player.GetModPlayer<BloomBuffsPlayer>().UnlockedSlot4 && SlotToCheckFor == 3)
            {
                IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/BloomBuffSlotLocked").Value; 
                BuffDisplayName = Language.GetTextValue("Mods.Spooky.UI.BloomSlotLocked");
            }

            if (IconTexture != null)
            {
                spriteBatch.Draw(IconTexture, IconTopLeft, null, Color.White * Transparency, 0f, Vector2.Zero, Main.UIScale, SpriteEffects.None, 0f);

                if (IsMouseOverUI((int)IconTopLeft.X, (int)IconTopLeft.Y, IconTexture, Vector2.One * Main.UIScale))
                {
                    if (IconTexture != ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/BloomBuffSlotLocked").Value)
                    {
                        Main.instance.MouseText(BuffDisplayName + "\n" + DurationToCheckFor.ToString());
                    }
                    else
                    {
                        Main.instance.MouseText(BuffDisplayName);
                    }
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