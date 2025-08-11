using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Enums;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.Audio;
using ReLogic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.UserInterfaces
{
	//krampus dialogue code referenced from Mod of redemption dialogue system: https://github.com/Hallam9K/RedemptionAlpha/tree/3b2349a6be8d1e64929c84c4777b7fb0776752d0/UI/ChatUI
    public class KrampusDialogueUI : UIState
    {
        public static List<IDialogue> Dialogue;

		public static bool Visible = true;
		public static bool DisplayPlayerResponse = false;
		public static bool DisplayingPlayerResponse = false;

		public Texture2D BoxTex;
		public Texture2D PlayerBoxTex;

		public override void OnInitialize()
        {
            Dialogue = new();

			BoxTex = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/KrampusDialogueBox", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
			PlayerBoxTex = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/KrampusDialoguePlayerBox", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
		}

        public override void Update(GameTime gameTime)
        {
			if (!Visible || Dialogue.Count == 0)
			{
				return;
			}

            base.Update(gameTime);

            for (int i = 0; i < Dialogue.Count; i++)
            {
                IDialogue dialogue = Dialogue[i];
                dialogue.Update(gameTime);
            }
        }
		
        public override void Draw(SpriteBatch spriteBatch)
        {
			if (!Visible || Dialogue.Count == 0)
			{
				return;
			}

			Main.LocalPlayer.mouseInterface = true;

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			for (int i = 0; i < Dialogue.Count; i++)
            {
                Dialogue dialogue = Dialogue[i].Get();
				
                if (dialogue.displayingText.Length == 0)
				{
                    return;
				}

				DisplayingPlayerResponse = !dialogue.NotPlayer && dialogue.textFinished;

				//shift camera to the entity the dialogue belongs to, only if it is not a player
				if (dialogue.NotPlayer)
				{
					Main.instance.CameraModifiers.Add(new CameraPanning(dialogue.entity.Center, 20));
				}

				string[] drawnText = FormatText(dialogue.displayingText, FontAssets.MouseText.Value, !dialogue.NotPlayer, out int width, out int height);
				Vector2 pos = (dialogue.chain == null ? Vector2.Zero : dialogue.chain.modifier) + dialogue.modifier + 
				(dialogue.entity != null ? dialogue.entity.Center - Main.screenPosition - new Vector2((width - 350f) / 2f, dialogue.entity.height - 15) : 
				dialogue.chain.anchor != null ? dialogue.chain.anchor.VisualPosition : new Vector2(Main.screenWidth / 2f - width / 2f, Main.screenHeight * 0.8f - height / 2f));

				Vector2 PlayerOffset = dialogue.NotPlayer ? new Vector2(0, 0) : new Vector2(-190, 65);

				DrawPanel(spriteBatch, dialogue.NotPlayer ? BoxTex : PlayerBoxTex, pos + PlayerOffset - new Vector2(16f, 12f), Color.White, width, height);

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				Vector2 textPos = pos + PlayerOffset;
                for (int k = 0; k < drawnText.Length; k++)
                {
                    string text = drawnText[k];
                    if (text == null)
					{
                        continue;
					}

                    DrawStringEightWay(spriteBatch, text, textPos);

                    textPos.Y += FontAssets.MouseText.Value.MeasureString(text).Y - 6;
                }
			}

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
		}

		public static void DrawPanel(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, int width, int height)
        {
            // Top Left
            Vector2 topLeftPos = position;
            Rectangle topLeftRect = new(0, 0, 34, 34);
            spriteBatch.Draw(texture, topLeftPos, topLeftRect, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Top Middle
            Rectangle topMiddlePos = new((int)topLeftPos.X + topLeftRect.Width, (int)topLeftPos.Y, width, 34);
            Rectangle topMiddleRect = new(34, 0, 2, 34);
            spriteBatch.Draw(texture, topMiddlePos, topMiddleRect, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Top Right
            Vector2 topRightPos = topMiddlePos.TopRight();
            Rectangle topRightRect = new(36, 0, 34, 34);
            spriteBatch.Draw(texture, topRightPos, topRightRect, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);


            // Middle Left
            Rectangle middleLeftDest = new((int)topLeftPos.X, (int)topLeftPos.Y + topLeftRect.Height, 34, height - 34);
            Rectangle middleLeftRect = new(0, 34, 34, 2);
            spriteBatch.Draw(texture, middleLeftDest, middleLeftRect, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Middle Middle
            Rectangle middleMiddleDest = new((int)topLeftPos.X + topLeftRect.Width, (int)topLeftPos.Y + topLeftRect.Height, width, height - 34);
            Rectangle middleMiddleRect = new(34, 34, 2, 2);
            spriteBatch.Draw(texture, middleMiddleDest, middleMiddleRect, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Middle Right
            Rectangle middleRightDest = new((int)topRightPos.X, (int)topRightPos.Y + topRightRect.Height, 34, height - 34);
            Rectangle middleRightRect = new(36, 34, 34, 2);
            spriteBatch.Draw(texture, middleRightDest, middleRightRect, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);


            // Bottom Left
            Vector2 bottomLeftPos = middleLeftDest.BottomLeft();
            Rectangle bottomLeftRect = new(0, 36, 34, 34);
            spriteBatch.Draw(texture, bottomLeftPos, bottomLeftRect, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Bottom Middle
            Rectangle bottomMiddlePos = new((int)bottomLeftPos.X + bottomLeftRect.Width, (int)bottomLeftPos.Y, width, 34);
            Rectangle bottomMiddleRect = new(34, 36, 2, 34);
            spriteBatch.Draw(texture, bottomMiddlePos, bottomMiddleRect, color, 0f, Vector2.Zero, SpriteEffects.None, 0f);

            // Bottom Right
            Vector2 bottomRightPos = middleRightDest.BottomLeft();
            Rectangle bottomRightRect = new(36, 36, 34, 34);
            spriteBatch.Draw(texture, bottomRightPos, bottomRightRect, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

		public static string[] FormatText(string text, DynamicSpriteFont font, bool isPlayer, out int width, out int height)
        {
            width = (int)font.MeasureString(text).X;
			int Limit = isPlayer ? 135 : 600;
            if (width > Limit)
			{
                width = Limit;
			}

            height = 0;

            string[] displayingText = Utils.WordwrapString(text, font, width, 10, out int lines);

            int largestWidth = 0;
            for (int i = 0; i < displayingText.Length; i++)
            {
                if (displayingText?[i] == null)
				{
                    continue;
				}

                Vector2 stringSize = font.MeasureString(displayingText[i]);

                if (stringSize.X > largestWidth)
				{
                    largestWidth = (int)stringSize.X;
				}

                height += (int)stringSize.Y - 6;
            }

            width = largestWidth - 34;
			
            if (height < 22)
			{
                height = 22;
			}

            return displayingText;
        }

        public static void DrawStringEightWay(SpriteBatch spriteBatch, string text, Vector2 position)
        {
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, text, position, Color.White, 0, Vector2.Zero, Vector2.One);
        }

        public static void Add(IDialogue dialogue)
        {
            Dialogue.Add(dialogue);
        }

        public static void Remove(IDialogue dialogue)
        {
            Dialogue.Remove(dialogue);
        }

        public static void Clear()
        {
            Dialogue.Clear();
        }
    }

	public class KrampusDialogueUISystem : ModSystem
	{
		public override void Load()
		{
			if (Main.dedServ)
				KrampusDialogueUI.Dialogue = new();
		}

		public override void PreUpdateEntities()
		{
			if (Main.dedServ && KrampusDialogueUI.Visible && KrampusDialogueUI.Dialogue.Count > 0)
			{
				for (int i = 0; i < KrampusDialogueUI.Dialogue.Count; i++)
				{
					IDialogue dialogue = KrampusDialogueUI.Dialogue[i];
					dialogue.Update(Main.gameTimeCache);
				}
			}
		}
	}

	public class Dialogue : IDialogue
	{
		public DialogueChain chain;

		public delegate void PlayerResponseTrigger(Dialogue dialogue, string text, int id);
		public event PlayerResponseTrigger OnPlayerResponseTrigger;

		public delegate void EndTrigger(Dialogue dialogue, int id);
		public event EndTrigger OnEndTrigger;

		public Entity entity;
		public SoundStyle? sound;

		public Vector2 modifier;

		public bool textFinished;
		public bool IsLastDialogue;
		public bool NotPlayer;
		public bool ClickedOnResponse = false;

		public string text;
		public string displayingText;
		public string playerText;

		public float timer;
		public float charTime;
		public float originalCharTime;
		public float pauseTime;
		public float preFadeTime;
		public float fadeTime;
		public float fadeTimeMax;

		public Dialogue(Entity entity, string text, string playerText, SoundStyle sound, float charTime, float preFadeTime, float fadeTime, Vector2 modifier = default, bool IsLastDialogue = false, bool NotPlayer = true)
		{
			this.entity = entity;
			this.playerText ??= playerText;
			this.text = text ?? "";
			displayingText ??= "";

			this.IsLastDialogue = IsLastDialogue;
			this.NotPlayer = NotPlayer;

			this.charTime = charTime;
			this.originalCharTime = charTime;
			this.preFadeTime = preFadeTime;
			this.fadeTime = fadeTime;
			this.fadeTimeMax = fadeTime;
			this.modifier = modifier;

			if (this.charTime < 0)
				this.charTime = 0.01f;

			if (!Main.dedServ)
			{
				this.sound = NotPlayer ? sound : null;
			}
		}

		public int Timer = 0;
		public int dialogueSoundTimer = 0;

		public void Update(GameTime gameTime)
		{
			if (Main.gamePaused)
			{
				return;
			}

			if (!IsLastDialogue)
			{
				// Measure our progress via a modulo between our char time and
				// our timer, allowing us to decide how many chars to display
				if (pauseTime <= 0 && displayingText.Length != text.Length && timer >= charTime)
				{
					dialogueSoundTimer++;

					if (displayingText.Length != 0)
					{
						char trigger = displayingText[^1];
						if (!Main.dedServ && dialogueSoundTimer % 3 == 0 && sound != null && (trigger is not '.' and not ',' and not '!' and not '?'))
						{
							SoundEngine.PlaySound((SoundStyle)sound, entity.position);
						}
					}

					displayingText += text[displayingText.Length];
					timer = 0;

					//dont delay punctuation on the player dialogue
					if (NotPlayer)
					{
						char PunctuationCheck = displayingText[^1];
						if ((PunctuationCheck == '.' || PunctuationCheck == '!' || PunctuationCheck == '?'))
						{
							charTime += 0.15f;
						}
						else if (PunctuationCheck == ',')
						{
							charTime += 0.05f;
						}
						else
						{
							charTime = originalCharTime;
						}
					}
				}

				if (displayingText.Length == text.Length && !textFinished)
				{
					textFinished = true;

					if (NotPlayer)
					{
						KrampusDialogueUI.DisplayPlayerResponse = true;
					}
				}

				if (textFinished && entity.active && KrampusDialogueUI.DisplayPlayerResponse && NotPlayer)
				{
					TriggerPlayerResponse(playerText, 0);
					chain?.TriggerPlayerResponse(playerText, 0);
					KrampusDialogueUI.DisplayPlayerResponse = false;
				}
			}

			if (textFinished && KrampusDialogueUI.DisplayingPlayerResponse && Main.mouseLeft && Main.mouseLeftRelease || !entity.active || IsLastDialogue)
			{
				if (IsLastDialogue)
				{
					TriggerEnd(0);
					chain?.TriggerEnd(0);
				}

				if (chain != null)
				{
					chain.Dialogue.Remove(this);
				}
				else
				{
					KrampusDialogueUI.Dialogue.Remove(this);
				}
			}

			float passedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			if (Main.FrameSkipMode == FrameSkipMode.Subtle)
				passedTime = 1f / 60f;

			pauseTime -= passedTime;
			if (pauseTime <= 0)
				timer += passedTime;
			if (textFinished)
				preFadeTime -= passedTime;
			if (preFadeTime <= 0)
				fadeTime -= passedTime;
		}

		public Dialogue Get() => this;

		public void TriggerEnd(int ID) => OnEndTrigger?.Invoke(this, ID);

		public void TriggerPlayerResponse(string Text, int ID) => OnPlayerResponseTrigger?.Invoke(this, Text, ID);
	}

	public class DialogueChain : IDialogue
	{
		public delegate void EndTrigger(Dialogue dialogue, int id);
		public event EndTrigger OnEndTrigger;

		public delegate void PlayerResponseTrigger(Dialogue dialogue, string text, int id);
		public event PlayerResponseTrigger OnPlayerResponseTrigger;

		public List<Dialogue> Dialogue;
		public Entity anchor;
		public Vector2 modifier;

		public DialogueChain(Entity anchor = null, Vector2 modifier = default)
		{
			Dialogue = new List<Dialogue>();
			this.anchor = anchor;
			this.modifier = modifier;
		}
		public void Update(GameTime gameTime)
		{
			Dialogue[0].Update(gameTime);
			if (Dialogue.Count == 0)
				KrampusDialogueUI.Dialogue.Remove(this);
		}
		public DialogueChain Add(Dialogue dialogue)
		{
			dialogue.chain = this;
			Dialogue.Add(dialogue);
			return this;
		}

		public Dialogue Get() => Dialogue[0];

		public void TriggerEnd(int ID) => OnEndTrigger?.Invoke(Dialogue[0], ID);

		public void TriggerPlayerResponse(string Text, int ID) => OnPlayerResponseTrigger?.Invoke(Dialogue[0], Text, ID);
	}

	public interface IDialogue
	{
		public void Update(GameTime gameTime);
		public Dialogue Get();
	}
}