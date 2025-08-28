using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Minibiomes.Christmas;
using Spooky.Content.UserInterfaces;

namespace Spooky.Content.NPCs.Friendly
{
    public class Krampus : ModNPC  
    {
		public static float Expression = 0;

		public Vector2 modifier = new(-200, 0);

		Player PlayerTalkingTo = null;

		public static readonly SoundStyle TalkSound = new("Spooky/Content/Sounds/Krampus/Talk", SoundType.Sound) { Volume = 0.35f, PitchVariance = 0.75f };

		private static Asset<Texture2D> HappyTexture;
		private static Asset<Texture2D> AngryTexture;
		private static Asset<Texture2D> SadTexture;
		private static Asset<Texture2D> EarHideTexture;

		public override void SetStaticDefaults()
        {
			Main.npcFrameCount[NPC.type] = 13;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                Position = new Vector2(0f, 75f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 55f
            };
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
            NPC.defense = 5;
            NPC.width = 88;
			NPC.height = 174;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().Type };
        }

        public override bool CheckActive()
		{
			return false;
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Krampus"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            HappyTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/KrampusHappy");
			AngryTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/KrampusAngry");
			SadTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/KrampusSad");
			EarHideTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/KrampusEarHide");

			Vector2 drawOrigin = new(HappyTexture.Width() * 0.5f, NPC.height * 0.5f);
			Vector2 drawPos = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY + 3);

			if (Expression > 0)
			{
				Main.EntitySpriteDraw(EarHideTexture.Value, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
			}

			//happy
			if (Expression == 1)
			{
				Main.EntitySpriteDraw(HappyTexture.Value, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
			}
			//angry
			if (Expression == 2)
			{
				Main.EntitySpriteDraw(AngryTexture.Value, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
			}
			//sad
			if (Expression == 3)
			{
				Main.EntitySpriteDraw(SadTexture.Value, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
			}
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 13)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

		public bool InventoryFull(Player player)
		{
			for (int i = 0; i < Main.InventorySlotsTotal; i++)
			{
				//if theres any empty slots at all then inventory is not full
				if (player.inventory[i].type < 1)
				{
					return false;
				}

				//if you have an encumbering stone, then count the inventory as full
				if (player.inventory[i].type == ItemID.EncumberingStone)
				{
					return true;
				}
			}

			return true;
		}

		public override void AI()
		{
			Lighting.AddLight(NPC.Center, 0.2f, 0.2f, 0.2f);

			foreach (var player in Main.ActivePlayers)
			{
				if (NPC.Hitbox.Intersects(new Rectangle((int)Main.MouseWorld.X - 1, (int)Main.MouseWorld.Y - 1, 1, 1)) &&
				NPC.Distance(player.Center) <= 150f && !Main.mapFullscreen && Main.myPlayer == player.whoAmI)
				{
					if (Main.mouseRight && Main.mouseRightRelease)
					{
						PlayerTalkingTo = player;

						if (!Main.dedServ)
						{
							if (!Flags.KrampusQuestGiven)
							{ 
								//main questline dialogue stuff
								if (!Flags.KrampusQuestlineDone)
								{
									//delivery 1
									if (!Flags.KrampusQuest1)
									{
										DialogueChain chain = new();
										chain.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue1-1"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse1-1"),
										TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue1-2"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse1-2"),
										TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue1-3"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse1-3"),
										TalkSound, 2f, 0f, modifier, Expression: 3, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue1-4"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse1-4"),
										TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue1-5"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse1-5"),
										TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: NPC.type))
										.Add(new(NPC, null, null, TalkSound, 2f, 0f, modifier, true));
										chain.OnPlayerResponseTrigger += PlayerResponse;
										chain.OnEndTrigger += SetKrampusQuestGiven;
										chain.OnEndTrigger += GivePlayerPresent;
										chain.OnEndTrigger += EndDialogue;
										DialogueUI.Visible = true;
										DialogueUI.Add(chain);
									}
									//delivery 2
									else if (Flags.KrampusQuest1 && !Flags.KrampusQuest2)
									{	
										DialogueChain chain = new();
										chain.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue2-1"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse2-1"),
										TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue2-2"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse2-2"),
										TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue2-3"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse2-3"),
										TalkSound, 2f, 0f, modifier, Expression: 2, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue2-4"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse2-4"),
										TalkSound, 2f, 0f, modifier, Expression: 2, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue2-5"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse2-5"),
										TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue2-6"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse2-6"),
										TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: NPC.type))
										.Add(new(NPC, null, null, TalkSound, 2f, 0f, modifier, true));
										chain.OnPlayerResponseTrigger += PlayerResponse;
										chain.OnEndTrigger += SetKrampusQuestGiven;
										chain.OnEndTrigger += GivePlayerPresent;
										chain.OnEndTrigger += GivePlayerReward;
										chain.OnEndTrigger += EndDialogue;
										DialogueUI.Visible = true;
										DialogueUI.Add(chain);
									}
									//delivery 3
									else if (Flags.KrampusQuest2 && !Flags.KrampusQuest3)
									{
										DialogueChain chain = new();
										chain.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue3-1"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse3-1"),
										TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue3-2"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse3-2"),
										TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue3-3"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse3-3"),
										TalkSound, 2f, 0f, modifier, Expression: 2, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue3-4"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse3-4"),
										TalkSound, 2f, 0f, modifier, Expression: 2, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue3-5"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse3-5"),
										TalkSound, 2f, 0f, modifier, Expression: 2, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue3-6"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse3-6"),
										TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
										.Add(new(NPC, null, null, TalkSound, 2f, 0f, modifier, true));
										chain.OnPlayerResponseTrigger += PlayerResponse;
										chain.OnEndTrigger += SetKrampusQuestGiven;
										chain.OnEndTrigger += GivePlayerPresent;
										chain.OnEndTrigger += GivePlayerReward;
										chain.OnEndTrigger += EndDialogue;
										DialogueUI.Visible = true;
										DialogueUI.Add(chain);
									}
									//delivery 4
									else if (Flags.KrampusQuest3 && !Flags.KrampusQuest4)
									{
										DialogueChain chain = new();
										chain.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue4-1"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse4-1"),
										TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue4-2"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse4-2"),
										TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue4-3"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse4-3"),
										TalkSound, 2f, 0f, modifier, Expression: 2, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue4-4"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse4-4"),
										TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue4-5"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse4-5"),
										TalkSound, 2f, 0f, modifier, Expression: 2, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue4-6"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse4-6"),
										TalkSound, 2f, 0f, modifier, Expression: 3, NPCID: NPC.type))
										.Add(new(NPC, null, null, TalkSound, 2f, 0f, modifier, true));
										chain.OnPlayerResponseTrigger += PlayerResponse;
										chain.OnEndTrigger += SetKrampusQuestGiven;
										chain.OnEndTrigger += GivePlayerPresent;
										chain.OnEndTrigger += GivePlayerReward;
										chain.OnEndTrigger += EndDialogue;
										DialogueUI.Visible = true;
										DialogueUI.Add(chain);
									}
									//delivery 5 (little eye)
									else if (Flags.KrampusQuest4 && !Flags.KrampusQuest5)
									{
										DialogueChain chain = new();
										chain.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue5-1"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse5-1"),
										TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue5-2"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse5-2"),
										TalkSound, 2f, 0f, modifier, Expression: 2, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue5-3"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse5-3"),
										TalkSound, 2f, 0f, modifier, Expression: 2, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue5-4"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse5-4"),
										TalkSound, 2f, 0f, modifier, Expression: 3, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue5-5"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse5-5"),
										TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue5-6"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse5-6"),
										TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
										.Add(new(NPC, null, null, TalkSound, 2f, 0f, modifier, true));
										chain.OnPlayerResponseTrigger += PlayerResponse;
										chain.OnEndTrigger += SetKrampusQuestGiven;
										chain.OnEndTrigger += GivePlayerPresent;
										chain.OnEndTrigger += GivePlayerReward;
										chain.OnEndTrigger += EndDialogue;
										DialogueUI.Visible = true;
										DialogueUI.Add(chain);
									}
									//post little eye delivery
									else if (Flags.KrampusQuest5)
									{
										DialogueChain chain = new();
										chain.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-1"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-1"),
										TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-2"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-2"),
										TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-3"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-3"),
										TalkSound, 2f, 0f, modifier, Expression: 2, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-4"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-4"),
										TalkSound, 2f, 0f, modifier, Expression: 2, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-5"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-5"),
										TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-6"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-6"),
										TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: NPC.type))
										.Add(new(NPC,
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-7"),
										Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-7"),
										TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: NPC.type))
										.Add(new(NPC, null, null, TalkSound, 2f, 0f, modifier, true));
										chain.OnPlayerResponseTrigger += PlayerResponse;
										chain.OnEndTrigger += GivePlayerReward;
										chain.OnEndTrigger += SetKrampusQuestlineComplete;
										chain.OnEndTrigger += EndDialogue;
										DialogueUI.Visible = true;
										DialogueUI.Add(chain);
									}
								}
								//if you have finished the main questline then do daily quest dialogue
								else
								{
									//if there is a daily quest to be completed
									if (Flags.KrampusDailyQuest)
									{
										//dialogue for krampus giving you the quest
										if (!Flags.KrampusDailyQuestDone)
										{
											int DialogueID = Main.rand.Next(1, 3);

											DialogueChain chain = new();
											chain.Add(new(NPC,
											Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DailyQuest" + DialogueID + "-1"),
											Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerDailyQuest" + DialogueID + "-1"),
											TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
											.Add(new(NPC,
											Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DailyQuest" + DialogueID + "-2"),
											Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerDailyQuest" + DialogueID + "-2"),
											TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
											.Add(new(NPC, null, null, TalkSound, 2f, 0f, modifier, true));
											chain.OnPlayerResponseTrigger += PlayerResponse;
											chain.OnEndTrigger += SetKrampusQuestGiven;
											chain.OnEndTrigger += GivePlayerPresent;
											chain.OnEndTrigger += EndDialogue;
											DialogueUI.Visible = true;
											DialogueUI.Add(chain);
										}
										//daily quest complete dialogue
										else
										{
											DialogueChain chain = new();
											chain.Add(new(NPC,
											Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DailyQuestComplete"),
											Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerDailyQuestComplete"),
											TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
											.Add(new(NPC, null, null, TalkSound, 2f, 0f, modifier, true));
											chain.OnPlayerResponseTrigger += PlayerResponse;
											chain.OnEndTrigger += GivePlayerReward;
											chain.OnEndTrigger += SetDailyQuestComplete;
											chain.OnEndTrigger += EndDialogue;
											DialogueUI.Visible = true;
											DialogueUI.Add(chain);
										}
									}
									//if theres no daily quest, krampus tells you to come back the next day
									else
									{
										if (Main.dayTime)
										{
											DialogueChain chain = new();
											chain.Add(new(NPC,
											Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.NoDailyQuestDay"),
											Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerNoDailyQuest"),
											TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
											.Add(new(NPC, null, null, TalkSound, 2f, 0f, modifier, true));
											chain.OnPlayerResponseTrigger += PlayerResponse;
											chain.OnEndTrigger += EndDialogue;
											DialogueUI.Visible = true;
											DialogueUI.Add(chain);
										}
										else
										{
											DialogueChain chain = new();
											chain.Add(new(NPC,
											Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.NoDailyQuestNight"),
											Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerNoDailyQuest"),
											TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
											.Add(new(NPC, null, null, TalkSound, 2f, 0f, modifier, true));
											chain.OnPlayerResponseTrigger += PlayerResponse;
											chain.OnEndTrigger += EndDialogue;
											DialogueUI.Visible = true;
											DialogueUI.Add(chain);
										}
									}
								}
							}
							else
							{
								//if you loose a present, then krampus gives you another one
								if (!PlayerTalkingTo.HasItem(ModContent.ItemType<QuestPresent1>()) &&
								!PlayerTalkingTo.HasItem(ModContent.ItemType<QuestPresent2>()) &&
								!PlayerTalkingTo.HasItem(ModContent.ItemType<QuestPresent3>()) &&
								!PlayerTalkingTo.HasItem(ModContent.ItemType<QuestPresentLittleEye>()))
								{
									DialogueChain chain = new();
									chain.Add(new(NPC,
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DialogueNoPresent-1"),
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerNoPresent-1"),
									TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
									.Add(new(NPC,
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DialogueNoPresent-2"),
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerNoPresent-2"),
									TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: NPC.type))
									.Add(new(NPC,
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DialogueNoPresent-3"),
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerNoPresent-3"),
									TalkSound, 2f, 0f, modifier, Expression: 2, NPCID: NPC.type))
									.Add(new(NPC,
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DialogueNoPresent-4"),
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerNoPresent-4"),
									TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
									.Add(new(NPC, null, null, TalkSound, 2f, 0f, modifier, true));
									chain.OnPlayerResponseTrigger += PlayerResponse;
									chain.OnEndTrigger += GivePlayerPresent;
									chain.OnEndTrigger += EndDialogue;
									DialogueUI.Visible = true;
									DialogueUI.Add(chain);
								}
								//if you have a present, then krampus tells you to go deliver it
								else
								{
									DialogueChain chain = new();
									chain.Add(new(NPC,
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DialoguePresent-1"),
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerPresent-1"),
									TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: NPC.type))
									.Add(new(NPC,
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DialoguePresent-2"),
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerPresent-2"),
									TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
									.Add(new(NPC,
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DialoguePresent-3"),
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerPresent-3"),
									TalkSound, 2f, 0f, modifier, Expression: 0, NPCID: NPC.type))
									.Add(new(NPC,
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DialoguePresent-4"),
									Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerPresent-4"),
									TalkSound, 2f, 0f, modifier, Expression: 2, NPCID: NPC.type))
									.Add(new(NPC, null, null, TalkSound, 2f, 0f, modifier, true));
									chain.OnPlayerResponseTrigger += PlayerResponse;
									chain.OnEndTrigger += EndDialogue;
									DialogueUI.Visible = true;
									DialogueUI.Add(chain);
								}
							}
						}
					}
				}
			}
		}

		private void PlayerResponse(Dialogue dialogue, string Text, int ID)
		{
			Dialogue newDialogue = new(PlayerTalkingTo, Text, null, SoundID.Item1, 2f, 0f, default, NotPlayer: false);
			DialogueUI.Visible = true;
			DialogueUI.Add(newDialogue);
		}

		private void GivePlayerReward(Dialogue dialogue, int ID)
		{
			int[] Presents = new int[] { ModContent.ItemType<KrampusRewardPresent1>(), ModContent.ItemType<KrampusRewardPresent2>(), ModContent.ItemType<KrampusRewardPresent3>() };
			int newItem = Item.NewItem(NPC.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, Main.rand.Next(Presents));

			if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
			{
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
			}
		}

		private void GivePlayerPresent(Dialogue dialogue, int ID)
		{
			if (InventoryFull(PlayerTalkingTo))
			{
				DialogueChain chain = new();
				chain.Add(new(NPC,
				Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DialogueInvFull-1"),
				Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerInvFull-1"),
				TalkSound, 2f, 0f, modifier, Expression: 0))
				.Add(new(NPC,
				Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DialogueInvFull-2"),
				Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerInvFull-2"),
				TalkSound, 2f, 0f, modifier, Expression: 0))
				.Add(new(NPC,
				Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.DialogueInvFull-3"),
				Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerInvFull-3"),
				TalkSound, 2f, 0f, modifier, Expression: 0))
				.Add(new(NPC, null, null, TalkSound, 2f, 0f, modifier, true));
				chain.OnPlayerResponseTrigger += PlayerResponse;
				chain.OnEndTrigger += GivePlayerPresentWithoutInvCheck;
				DialogueUI.Visible = true;
				DialogueUI.Add(chain);
			}
			else
			{
				int[] Items = new int[] { ModContent.ItemType<QuestPresent1>(), ModContent.ItemType<QuestPresent2>(), ModContent.ItemType<QuestPresent3>() };

				int Type = Flags.KrampusQuest4 ? ModContent.ItemType<QuestPresentLittleEye>() : Main.rand.Next(Items);
				int newItem = Item.NewItem(NPC.GetSource_DropAsItem(), PlayerTalkingTo.Hitbox, Type, 1);

				if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
				{
					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
				}
			}
		}

		private void GivePlayerPresentWithoutInvCheck(Dialogue dialogue, int ID)
		{
			int[] Items = new int[] { ModContent.ItemType<QuestPresent1>(), ModContent.ItemType<QuestPresent2>(), ModContent.ItemType<QuestPresent3>() };

			int Type = Flags.KrampusQuest4 ? ModContent.ItemType<QuestPresentLittleEye>() : Main.rand.Next(Items);
			int newItem = Item.NewItem(NPC.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, Type, 1);

			if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
			{
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
			}

			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)SpookyMessageType.KrampusQuestGiven);
				packet.Send();
			}
			else
			{
				Flags.KrampusQuestGiven = true;
			}
		}

		private void SetKrampusQuestGiven(Dialogue dialogue, int ID)
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)SpookyMessageType.KrampusQuestGiven);
				packet.Send();
			}
			else
			{
				Flags.KrampusQuestGiven = true;
			}
		}

		private void SetKrampusQuestlineComplete(Dialogue dialogue, int ID)
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)SpookyMessageType.KrampusQuestlineDone);
				packet.Send();
			}
			else
			{
				Flags.KrampusQuestlineDone = true;
			}
		}

		private void SetDailyQuestComplete(Dialogue dialogue, int ID)
		{
			//set daily quest done to true so krampus displays the dialogue after you have delivered the gift
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)SpookyMessageType.KrampusDailyQuestDone);
				packet.Send();
			}
			else
			{
				Flags.KrampusDailyQuestDone = true;
			}

			//set krampus daily quest to false so he doesnt keep giving you quests until the next day
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)SpookyMessageType.KrampusDailyQuestReset);
				packet.Send();
			}
			else
			{
				Flags.KrampusDailyQuest = false;
			}
		}

		private void EndDialogue(Dialogue dialogue, int ID)
		{
			DialogueUI.Visible = false;
			Expression = 0;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
    }
}