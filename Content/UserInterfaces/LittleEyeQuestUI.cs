using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.UserInterfaces
{
    public class LittleEyeQuestUI
    {
        public static int LittleEye = -1;
        public static bool CurrentlyViewing = false;
        public static bool IsHoveringOverAnyButton = false;

        public static readonly Vector2 UITopLeft = new Vector2(Main.screenWidth / 2 - 275f, Main.screenHeight / 2 - 140f);

        public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);

        public static void Draw(SpriteBatch spriteBatch)
        {
            //dont draw at all if the UI isnt open
            if (!CurrentlyViewing)
            {
                LittleEye = -1;
                return;
            }

            //stop the UI from being open if the player is doing other stuff
            if (Main.LocalPlayer.chest != -1 || Main.LocalPlayer.sign != -1 || Main.LocalPlayer.talkNPC == -1 || !InRangeOfNPC() || Main.InGuideCraftMenu)
            {
                CurrentlyViewing = false;
                return;
            }

            Main.npcChatText = string.Empty;

            Texture2D UIBoxTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestUIBar").Value;
            Vector2 UIBoxScale = Vector2.One * Main.UIScale;

            //draw the main UI box
            spriteBatch.Draw(UIBoxTexture, UITopLeft, null, Color.White, 0f, Vector2.Zero, UIBoxScale, SpriteEffects.None, 0f);

            //prevent any mouse interactions while the mouse is hovering over this UI
            if (IsMouseOverUI((int)UITopLeft.X, (int)UITopLeft.Y, UIBoxTexture, UIBoxScale))
            {
                IsHoveringOverAnyButton = false;

                Main.LocalPlayer.mouseInterface = false;
                Main.blockMouse = true;
            }

            Point ButtonTopLeft = (UITopLeft + new Vector2(-250f, 32f) * UIBoxScale).ToPoint();

            //draw each bounty icon and display text when hovering over it

            //eye gremlin display stuff
            Vector2 Icon1TopLeft = ButtonTopLeft.ToVector2() + new Vector2(315f, -24f) * Main.UIScale;

            Texture2D Icon1Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BountyIcon1NotDone").Value;
            DrawIcon(spriteBatch, Icon1TopLeft, Icon1Texture);

            if (IsMouseOverUI((int)Icon1TopLeft.X, (int)Icon1TopLeft.Y, Icon1Texture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                string Quest1Text = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Bounty1");
                DrawTextDescription(spriteBatch, UITopLeft + new Vector2(22f, 110f) * UIBoxScale, Quest1Text);

                if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    //TODO: implement accepting bounty here
                }
            }

            //chalupo display stuff
            Vector2 Icon2TopLeft = ButtonTopLeft.ToVector2() + new Vector2(400f, -24f) * Main.UIScale;

            Texture2D Icon2Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BountyIcon1NotDone").Value;
            DrawIcon(spriteBatch, Icon2TopLeft, Icon2Texture);

            if (IsMouseOverUI((int)Icon2TopLeft.X, (int)Icon2TopLeft.Y, Icon2Texture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                string QuestIcon2Text = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Bounty2");
                DrawTextDescription(spriteBatch, UITopLeft + new Vector2(22f, 110f) * UIBoxScale, QuestIcon2Text);

                if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    //TODO: implement accepting bounty here
                }
            }

            //spider grotto display stuff
            Vector2 Icon3TopLeft = ButtonTopLeft.ToVector2() + new Vector2(485f, -24f) * Main.UIScale;

            Texture2D Icon3Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BountyIcon1NotDone").Value;
            DrawIcon(spriteBatch, Icon3TopLeft, Icon3Texture);

            if (IsMouseOverUI((int)Icon3TopLeft.X, (int)Icon3TopLeft.Y, Icon3Texture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                string QuestIcon3Text = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Bounty3");
                DrawTextDescription(spriteBatch, UITopLeft + new Vector2(22f, 110f) * UIBoxScale, QuestIcon3Text);

                if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    //TODO: implement accepting bounty here
                }
            }

            //eye wizard display stuff
            Vector2 Icon4TopLeft = ButtonTopLeft.ToVector2() + new Vector2(570f, -24f) * Main.UIScale;

            Texture2D Icon4Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BountyIcon1NotDone").Value;
            DrawIcon(spriteBatch, Icon4TopLeft, Icon4Texture);

            if (IsMouseOverUI((int)Icon4TopLeft.X, (int)Icon4TopLeft.Y, Icon4Texture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                string QuestIcon4Text = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Bounty4");
                DrawTextDescription(spriteBatch, UITopLeft + new Vector2(22f, 110f) * UIBoxScale, QuestIcon4Text);

                if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    //TODO: implement accepting bounty here
                }
            }

            Vector2 OrroboroIconTopLeft = ButtonTopLeft.ToVector2() + new Vector2(655f, -24f) * Main.UIScale;

            Texture2D OrroboroIconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BountyIcon5Locked").Value;
            DrawIcon(spriteBatch, OrroboroIconTopLeft, OrroboroIconTexture);

            if (IsMouseOverUI((int)OrroboroIconTopLeft.X, (int)OrroboroIconTopLeft.Y, OrroboroIconTexture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                bool downedAllMechs = NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;

                string OrroboroIconText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Bounty5Locked");
                
                if (downedAllMechs)
                {
                    OrroboroIconText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Bounty5");
                }

                DrawTextDescription(spriteBatch, UITopLeft + new Vector2(22f, 110f) * UIBoxScale, OrroboroIconText);

                if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    //TODO: implement accepting bounty here
                }
            }

            /*
            //still dunno if im even going to bother implementing this, dont really think its necessary ngl
            if (!IsHoveringOverAnyButton)
            {
                string NoSelectedText = "[c/8284FF: So, you want to help me with some tasks?]"
                + "\n[c/8284FF: Well, you can help me find some of my lost... experiments.]"
                + "\n[c/8284FF: If you can track them down, I can give you some pretty crazy goodies.]";
                DrawTextDescription(spriteBatch, UITopLeft + new Vector2(300f, 120f) * UIBoxScale, NoSelectedText);
            }
            */
        }

        public static bool InRangeOfNPC()
        {
            if (!Main.npc.IndexInRange(LittleEye) || !Main.npc[LittleEye].active)
            {
                return false;
            }

            Rectangle validTalkArea = Utils.CenteredRectangle(Main.LocalPlayer.Center, new Vector2(Player.tileRangeX * 3f, Player.tileRangeY * 2f) * 16f);
            
            return validTalkArea.Intersects(Main.npc[LittleEye].Hitbox);
        }

        public static void DrawTextDescription(SpriteBatch spriteBatch, Vector2 nameDrawCenter, string Text)
        {
            Vector2 scale = new Vector2(0.85f, 0.85f) * Main.UIScale;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, Text, nameDrawCenter, Color.White, 0f, Vector2.Zero, scale);
        }

        //used to draw individual icons over the main UI box
        public static void DrawIcon(SpriteBatch spriteBatch, Vector2 drawPositionTopLeft, Texture2D texture)
        {
            spriteBatch.Draw(texture, drawPositionTopLeft, null, Color.White, 0f, Vector2.Zero, Main.UIScale, SpriteEffects.None, 0f);
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