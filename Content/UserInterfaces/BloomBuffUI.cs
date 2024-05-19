using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using Spooky.Core;

namespace Spooky.Content.UserInterfaces
{
    public class BloomBuffUI
    {
        public static bool UIOpen = false;
        public static bool IsDragging = false;

        public static float Transparency = 0f;

        private static Asset<Texture2D> BarTexture;

        public static void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            //dont draw this ui if its transparent
            if (Transparency <= 0f)
            {
                return;
            }

            BarTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffUIBox");
            Vector2 UIBoxScale = Vector2.One * Main.UIScale * 0.9f;

            //UI dragging 
            MouseState mouse = Mouse.GetState();

            //only allow UI dragging if the config option is on and the player is not in the inventory
            if (ModContent.GetInstance<SpookyConfig>().CanDragBloomBuffUI && !Main.playerInventory)
            {
                //if the player is hovering over the UI panel and presses left click then allow dragging
                if (IsMouseOverUI((int)player.GetModPlayer<BloomBuffsPlayer>().UITopLeft.X, (int)player.GetModPlayer<BloomBuffsPlayer>().UITopLeft.Y, BarTexture.Value, UIBoxScale) && !IsDragging && mouse.LeftButton == ButtonState.Pressed)
                {
                    IsDragging = true;
                }

                //if the player is dragging and continues to hold mouse left, then move the ui to the center of the mouse
                if (IsDragging && mouse.LeftButton == ButtonState.Pressed)
                {
                    player.mouseInterface = true;
                    player.GetModPlayer<BloomBuffsPlayer>().UITopLeft = Main.MouseScreen - (BarTexture.Size() / 2) * UIBoxScale;
                }

                //if the player lets go of mouse left, stop dragging the UI panel
                if (IsDragging && mouse.LeftButton == ButtonState.Released)
                {
                    IsDragging = false;
                }
            }
            else
            {
                //set dragging to false here just to be safe
                IsDragging = false;
            }

            //draw the main UI box
            spriteBatch.Draw(BarTexture.Value, player.GetModPlayer<BloomBuffsPlayer>().UITopLeft, null, Color.White * Transparency, 0f, Vector2.Zero, UIBoxScale, SpriteEffects.None, 0f);

            //bloom buff icon drawing for each slot
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[0] != string.Empty)
            {
                DrawIcon(spriteBatch, player, player.GetModPlayer<BloomBuffsPlayer>().UITopLeft + new Vector2(19.8f, 30.5f) * Main.UIScale, 0, player.GetModPlayer<BloomBuffsPlayer>().Duration1);
            }
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[1] != string.Empty)
            {
                DrawIcon(spriteBatch, player, player.GetModPlayer<BloomBuffsPlayer>().UITopLeft + new Vector2(63.2f, 30.5f) * Main.UIScale, 1, player.GetModPlayer<BloomBuffsPlayer>().Duration2);
            }
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[2] != string.Empty)
            {
                DrawIcon(spriteBatch, player, player.GetModPlayer<BloomBuffsPlayer>().UITopLeft + new Vector2(106.1f, 30.5f) * Main.UIScale, 2, player.GetModPlayer<BloomBuffsPlayer>().Duration3);
            }
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[3] != string.Empty)
            {
                DrawIcon(spriteBatch, player, player.GetModPlayer<BloomBuffsPlayer>().UITopLeft + new Vector2(149.3f, 30.5f) * Main.UIScale, 3, player.GetModPlayer<BloomBuffsPlayer>().Duration4);
            }

            //draw locked icons if the player doesnt have those respective slots unlocked yet
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[2] == string.Empty && !player.GetModPlayer<BloomBuffsPlayer>().UnlockedSlot3)
            {
                DrawIcon(spriteBatch, player, player.GetModPlayer<BloomBuffsPlayer>().UITopLeft + new Vector2(106.1f, 30.5f) * Main.UIScale, 2, player.GetModPlayer<BloomBuffsPlayer>().Duration3);
            }
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[3] == string.Empty && !player.GetModPlayer<BloomBuffsPlayer>().UnlockedSlot4)
            {
                DrawIcon(spriteBatch, player, player.GetModPlayer<BloomBuffsPlayer>().UITopLeft + new Vector2(149.3f, 30.5f) * Main.UIScale, 3, player.GetModPlayer<BloomBuffsPlayer>().Duration4);
            }
        }

        //used to draw individual buff icons over the main UI box
        public static void DrawIcon(SpriteBatch spriteBatch, Player player, Vector2 IconTopLeft, int SlotToCheckFor, int DurationToCheckFor)
        {
            Texture2D IconTexture = null;

            string BuffDisplayName = string.Empty;

            switch (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[SlotToCheckFor])
            {
                case "FallGourd": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/FallGourdIcon").Value; 
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.FallGourd.DisplayName");
                    break;
                case "FallSoulPumpkin": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/FallSoulPumpkinIcon").Value;
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.FallSoulPumpkin.DisplayName");
                    break;
                case "FallWaterGourd": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/FallWaterGourdIcon").Value; 
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.FallWaterGourd.DisplayName");
                    break;
                case "FallZucchini": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/FallZucchiniIcon").Value; 
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.FallZucchini.DisplayName");
                    break;
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
                case "SpringHeartFlower": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/SpringHeartFlowerIcon").Value; 
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.SpringHeartFlower.DisplayName");
                    break;
                case "SpringIris": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/SpringIrisIcon").Value; 
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.SpringIris.DisplayName");
                    break;
                case "SpringOrchid": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/SpringOrchidIcon").Value; 
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.SpringOrchid.DisplayName");
                    break;
                case "SpringRose": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/SpringRoseIcon").Value; 
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.SpringRose.DisplayName");
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
                case "DandelionHerd": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/DandelionHerdIcon").Value; 
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.DandelionHerd.DisplayName");
                    break;
                case "DandelionMapleSeed": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/DandelionMapleSeedIcon").Value; 
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.DandelionMapleSeed.DisplayName");
                    break;
                case "DandelionTumbleweed": 
                    IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/DandelionTumbleweedIcon").Value;
                    BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items.DandelionTumbleweed.DisplayName");
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
                spriteBatch.Draw(IconTexture, IconTopLeft, null, Color.White * Transparency, 0f, Vector2.Zero, Vector2.One * Main.UIScale * 0.9f, SpriteEffects.None, 0f);

                //only display text if the player is hovering over the UI, and the inventory is not open
                if (IsMouseOverUI((int)IconTopLeft.X, (int)IconTopLeft.Y, IconTexture, Vector2.One * Main.UIScale * 0.9f) && !Main.playerInventory)
                {
                    if (IconTexture != ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/BloomBuffSlotLocked").Value)
                    {
						//convert the buff time to minutes and seconds by dividing the time by 60 and then changing it to Minutes:Seconds format
						TimeSpan time = TimeSpan.FromSeconds(DurationToCheckFor / 60);
						string answer = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);

                        //if the player has the dragon fruit buff, then also display the dragon fruit buff stacks as part of the description
                        if (IconTexture == ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffIcons/DragonfruitIcon").Value)
                        {
                            Main.instance.MouseText(BuffDisplayName + "\n" + answer + "\nStacks: " + player.GetModPlayer<BloomBuffsPlayer>().DragonfruitStacks);
                        }
                        else
                        {
						    Main.instance.MouseText(BuffDisplayName + "\n" + answer);
                        }
                    }
                    else
                    {
                        Main.instance.MouseText(BuffDisplayName);
                    }
                }
            }
        }

        public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);

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