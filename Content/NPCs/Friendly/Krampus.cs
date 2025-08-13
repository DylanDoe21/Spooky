using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.UserInterfaces;
using Spooky.Content.Items.Minibiomes.Christmas;

namespace Spooky.Content.NPCs.Friendly
{
    public class Krampus : ModNPC  
    {
		int QuestConvoID = 0;

		public bool Yapping = false;

		public Vector2 modifier = new(-200, 0);

		Player PlayerTalkingTo = null;

		public static readonly SoundStyle TalkSound = new("Spooky/Content/Sounds/Krampus/Talk", SoundType.Sound) { Volume = 0.35f, PitchVariance = 0.75f };

		public override void SetStaticDefaults()
        {
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            Main.npcFrameCount[NPC.type] = 13;

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
            NPC.width = 40;
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

		public override void AI()
		{
			Lighting.AddLight(NPC.Center, 0.2f, 0.2f, 0.2f);

			if (Yapping)
			{
				if (PlayerTalkingTo != null)
				{
					PlayerTalkingTo.GetModPlayer<SpookyPlayer>().YappingWithKrampus = true;
				}
			}
			else
			{
				foreach (var player in Main.ActivePlayers)
				{
					if (NPC.Hitbox.Intersects(new Rectangle((int)Main.MouseWorld.X - 1, (int)Main.MouseWorld.Y - 1, 1, 1)) &&
					NPC.Distance(player.Center) <= 150f && !Main.mapFullscreen && Main.myPlayer == player.whoAmI)
					{
						/*Main.instance.MouseText(NPC.GivenOrTypeName + "\n" +
						Language.GetTextValue("Mods.Spooky.UI.KrampusDialogue.HoverTextTalk") + "\n" +
						Language.GetTextValue("Mods.Spooky.UI.KrampusDialogue.HoverTextQuest"));
						Main.mouseText = true;*/

						if (Main.mouseRight && Main.mouseRightRelease)
						{
							PlayerTalkingTo = player;

							Yapping = true;

							if (!Flags.KrampusQuest1)
							{
								DialogueChain chain = new();
								chain.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue1-1"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse1-1"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue1-2"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse1-2"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue1-3"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse1-3"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue1-4"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse1-4"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue1-5"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse1-5"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC, null, null, TalkSound, 0.045f, 2f, 0f, modifier, true));
								chain.OnPlayerResponseTrigger += PlayerResponse;
								chain.OnEndTrigger += QuestVariableSetting;
								KrampusDialogueUI.Visible = true;
								KrampusDialogueUI.Add(chain);
							}
							else if (Flags.KrampusQuest1 && !Flags.KrampusQuest2)
							{	
								DialogueChain chain = new();
								chain.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue2-1"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse2-1"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue2-2"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse2-2"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue2-3"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse2-3"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue2-4"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse2-4"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue2-5"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse2-5"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue2-6"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse2-6"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC, null, null, TalkSound, 0.045f, 2f, 0f, modifier, true));
								chain.OnPlayerResponseTrigger += PlayerResponse;
								chain.OnEndTrigger += QuestVariableSetting;
								KrampusDialogueUI.Visible = true;
								KrampusDialogueUI.Add(chain);
							}
							else if (Flags.KrampusQuest2 && !Flags.KrampusQuest3)
							{
								DialogueChain chain = new();
								chain.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue3-1"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse3-1"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue3-2"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse3-2"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue3-3"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse3-3"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue3-4"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse3-4"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue3-5"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse3-5"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue3-6"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse3-6"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC, null, null, TalkSound, 0.045f, 2f, 0f, modifier, true));
								chain.OnPlayerResponseTrigger += PlayerResponse;
								chain.OnEndTrigger += QuestVariableSetting;
								KrampusDialogueUI.Visible = true;
								KrampusDialogueUI.Add(chain);
							}
							else if (Flags.KrampusQuest3 && !Flags.KrampusQuest4)
							{
								DialogueChain chain = new();
								chain.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue4-1"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse4-1"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue4-2"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse4-2"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue4-3"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse4-3"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue4-4"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse4-4"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue4-5"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse4-5"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue4-6"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse4-6"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC, null, null, TalkSound, 0.045f, 2f, 0f, modifier, true));
								chain.OnPlayerResponseTrigger += PlayerResponse;
								chain.OnEndTrigger += QuestVariableSetting;
								KrampusDialogueUI.Visible = true;
								KrampusDialogueUI.Add(chain);
							}
							else if (Flags.KrampusQuest4 && !Flags.KrampusQuest5)
							{
								DialogueChain chain = new();
								chain.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue5-1"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse5-1"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue5-2"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse5-2"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue5-3"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse5-3"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue5-4"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse5-4"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue5-5"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse5-5"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue5-6"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse5-6"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC, null, null, TalkSound, 0.045f, 2f, 0f, modifier, true));
								chain.OnPlayerResponseTrigger += PlayerResponse;
								chain.OnEndTrigger += QuestVariableSetting;
								KrampusDialogueUI.Visible = true;
								KrampusDialogueUI.Add(chain);
							}
							else if (Flags.KrampusQuest5)
							{
								DialogueChain chain = new();
								chain.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-1"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-1"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-2"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-2"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-3"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-3"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-4"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-4"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-5"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-5"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-6"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-6"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC,
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.Dialogue6-7"),
								Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerResponse6-7"),
								TalkSound, 0.045f, 2f, 0f, modifier))
								.Add(new(NPC, null, null, TalkSound, 0.045f, 2f, 0f, modifier, true));
								chain.OnPlayerResponseTrigger += PlayerResponse;
								chain.OnEndTrigger += QuestVariableSetting;
								KrampusDialogueUI.Visible = true;
								KrampusDialogueUI.Add(chain);
							}
						}
					}
				}
			}
		}

		private void PlayerResponse(Dialogue dialogue, string Text, int ID)
		{
			Dialogue newDialogue = new(PlayerTalkingTo, Text, null, SoundID.Item1, 0.01f, 2f, 0f, default, NotPlayer: false);
			KrampusDialogueUI.Visible = true;
			KrampusDialogueUI.Add(newDialogue);
		}

		private void StopDialogueDefault(Dialogue dialogue, int ID)
		{
			Yapping = false;
			KrampusDialogueUI.Visible = false;
		}

		private void QuestVariableSetting(Dialogue dialogue, int ID)
		{
			Yapping = false;
			KrampusDialogueUI.Visible = false;

			int[] Items = new int[] { ModContent.ItemType<QuestPresent1>(), ModContent.ItemType<QuestPresent2>(), ModContent.ItemType<QuestPresent3>() };

			int newItem = Item.NewItem(NPC.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, Main.rand.Next(Items), 1);

			if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
			{
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
			}
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
    }
}