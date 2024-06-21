using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using SteelSeries.GameSense;

namespace Spooky.Content.UserInterfaces
{
    public class LittleEyeQuestUI
    {
        public static int LittleEye = -1;
        public static bool UIOpen = false;
        public static bool IsHoveringOverAnyButton = false;

        public static readonly Vector2 UITopLeft = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

        public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);

        public static void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            //dont draw at all if the UI isnt open
            if (!UIOpen)
            {
                LittleEye = -1;
                return;
            }

            //stop the UI from being open if the player is doing other stuff
            if (player.chest != -1 || player.sign != -1 || player.talkNPC == -1 || !InRangeOfNPC() || Main.InGuideCraftMenu)
            {
                UIOpen = false;
                return;
            }

            Main.npcChatText = string.Empty;

            Texture2D UIBoxTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestUIBar").Value;
            Vector2 UIBoxScale = Vector2.One * Main.UIScale;

            //draw the main UI box
            spriteBatch.Draw(UIBoxTexture, UITopLeft, null, Color.White, 0f, UIBoxTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

            //prevent any mouse interactions while the mouse is hovering over this UI
            if (IsMouseOverUI(UITopLeft, UIBoxTexture, UIBoxScale))
            {
                IsHoveringOverAnyButton = false;

                player.mouseInterface = false;
                Main.blockMouse = true;
            }

            Point ButtonTopLeft = (UITopLeft + new Vector2(-525f, -110f) * UIBoxScale).ToPoint();

			string QuestIcon1Text = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty1");
			string Quest1ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty1Condition");

			string QuestIcon2Text = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty2");
			string Quest2ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty2Condition");

			string QuestIcon3Text = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty3");
			string Quest3ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty3Condition");

			string QuestIcon4Text = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty4");
			string Quest4ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty4Condition");

			string QuestIcon5LockedText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Locked");
			string QuestIcon5Text = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5");
			string Quest5ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Condition");

			string QuestAcceptText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyAccept");
			string QuestWarningText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyWarning");
			string Quest5AcceptText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Accept");
			string Quest5WarningText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Warning");

			string QuestCompleteText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyCompleted");

			//draw each bounty icon and display text when hovering over it

			//eye gremlin display stuff
			Vector2 Icon1TopLeft = ButtonTopLeft.ToVector2() + new Vector2(315f, -24f) * Main.UIScale;

            Texture2D Icon1Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon1NotDone").Value;
            DrawIcon(spriteBatch, Icon1TopLeft, Icon1Texture);

            if (IsMouseOverUI(Icon1TopLeft, Icon1Texture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

				DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, 
				QuestIcon1Text, Quest1ConditionText, QuestAcceptText, QuestWarningText, Color.OrangeRed);

				if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    //TODO: implement accepting bounty here
                }
            }

            //chalupo display stuff
            Vector2 Icon2TopLeft = ButtonTopLeft.ToVector2() + new Vector2(400f, -24f) * Main.UIScale;

            Texture2D Icon2Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon1NotDone").Value;
            DrawIcon(spriteBatch, Icon2TopLeft, Icon2Texture);

            if (IsMouseOverUI(Icon2TopLeft, Icon2Texture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

				DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale,
				QuestIcon2Text, Quest2ConditionText, QuestAcceptText, QuestWarningText, Color.SeaGreen);

				if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    //TODO: implement accepting bounty here
                }
            }

            //spider grotto display stuff
            Vector2 Icon3TopLeft = ButtonTopLeft.ToVector2() + new Vector2(485f, -24f) * Main.UIScale;

            Texture2D Icon3Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon1NotDone").Value;
            DrawIcon(spriteBatch, Icon3TopLeft, Icon3Texture);

            if (IsMouseOverUI(Icon3TopLeft, Icon3Texture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

				DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale,
				QuestIcon3Text, Quest3ConditionText, QuestAcceptText, QuestWarningText, Color.Chocolate);

				if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    //TODO: implement accepting bounty here
                }
            }

            //eye wizard display stuff
            Vector2 Icon4TopLeft = ButtonTopLeft.ToVector2() + new Vector2(570f, -24f) * Main.UIScale;

            Texture2D Icon4Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon4NotDone").Value;
            DrawIcon(spriteBatch, Icon4TopLeft, Icon4Texture);

            if (IsMouseOverUI(Icon4TopLeft, Icon4Texture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

				DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale,
				QuestIcon4Text, Quest4ConditionText, QuestAcceptText, QuestWarningText, Color.HotPink);

				if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    //TODO: implement accepting bounty here
                }
            }

            //orroboro display stuff
            Vector2 OrroboroIconTopLeft = ButtonTopLeft.ToVector2() + new Vector2(655f, -24f) * Main.UIScale;
            Texture2D OrroboroIconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon5Locked").Value;

            bool downedAllMechs = NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;

            if (downedAllMechs)
            {
                if (!Flags.downedOrroboro)
                {
                    OrroboroIconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon5NotDone").Value;
                }
                else
                {
                    OrroboroIconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon5Done").Value;
                }
            }

            DrawIcon(spriteBatch, OrroboroIconTopLeft, OrroboroIconTexture);

            if (IsMouseOverUI(OrroboroIconTopLeft, OrroboroIconTexture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;
                
				//quest text
                if (downedAllMechs)
                {
					//display the actual quest text if you havent killed orro-boro but you killed the mechs
                    if (!Flags.downedOrroboro)
                    {
						DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale,
						QuestIcon5Text, Quest5ConditionText, Quest5AcceptText, Quest5WarningText, Color.Magenta);
					}
					//if you have killed orro-boro display the quest as complete
                    else
                    {
						DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale,
						QuestCompleteText, string.Empty, string.Empty, string.Empty, Color.White);
					}
                }
				//if you havent killed all 3 mechs, then display the quest as locked
				else
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale,
					string.Empty, QuestIcon5LockedText, string.Empty, string.Empty, Color.Red);
				}

                if (Main.mouseLeftRelease && Main.mouseLeft)
                {
                    //TODO: implement accepting bounty here
                }
            }

            /*
            //might not even be necessary to implement
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

		public static void DrawTextDescription(SpriteBatch spriteBatch, Vector2 TextTopLeft, string Description, string Condition, string Accept, string Warning, Color ConditionColor)
		{
			Vector2 scale = new Vector2(0.9f, 0.925f) * MathHelper.Clamp(Main.screenHeight / 1440f, 0.825f, 1f) * Main.UIScale;

			//this probably doesnt look pretty, but since text wrapping didnt work with manually coloring the text in localization I had to do this instead
			//basically it takes all 4 parts of the text for each bounty and just draws each one separately below each other with their own individual colors

			//first draw the mission description
			foreach (string TextLine in Utils.WordwrapString(Description, FontAssets.MouseText.Value, 700, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}
				
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, new Color(130, 132, 255), 0f, Vector2.Zero, scale);
				TextTopLeft.Y += Main.UIScale * 16f;
			}

			//then draw the condition text for the biome you find the miniboss in
			foreach (string TextLine in Utils.WordwrapString(Condition, FontAssets.MouseText.Value, 700, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, ConditionColor, 0f, Vector2.Zero, scale);
				TextTopLeft.Y += Main.UIScale * 16f;
			}

			//draw the text to tell players they have to click the button to accept the bounty
			foreach (string TextLine in Utils.WordwrapString(Accept, FontAssets.MouseText.Value, 700, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, Color.Lime, 0f, Vector2.Zero, scale);
				TextTopLeft.Y += Main.UIScale * 16f;
			}

			//finally display the warning that you cant accept another bounty until the selected one is done
			foreach (string TextLine in Utils.WordwrapString(Warning, FontAssets.MouseText.Value, 700, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, Color.Red, 0f, Vector2.Zero, scale);
				TextTopLeft.Y += Main.UIScale * 16f;
			}
		}

		//used to draw individual icons over the main UI box
		public static void DrawIcon(SpriteBatch spriteBatch, Vector2 drawPositionTopLeft, Texture2D texture)
        {
            spriteBatch.Draw(texture, drawPositionTopLeft, null, Color.White, 0f, Vector2.Zero, Main.UIScale, SpriteEffects.None, 0f);
        }

        //check if the mouse is hovering over a specific button or UI box
        public static bool IsMouseOverUI(Vector2 TopLeft, Texture2D texture, Vector2 backgroundScale)
        {
            Rectangle backgroundArea = new Rectangle((int)TopLeft.X, (int)TopLeft.Y, (int)(texture.Width * backgroundScale.X), (int)(texture.Width * backgroundScale.Y));

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