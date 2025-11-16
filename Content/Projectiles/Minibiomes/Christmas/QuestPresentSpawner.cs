using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.NPCs.Minibiomes.Christmas;
using Spooky.Content.UserInterfaces;

namespace Spooky.Content.Projectiles.Minibiomes.Christmas
{
    public class QuestPresentSpawner : ModProjectile
    {
		bool Shake = false;

		public Vector2 modifier = new(-200, -75);

		public static readonly SoundStyle TalkSound = new("Spooky/Content/Sounds/Krampus/Talk", SoundType.Sound) { Volume = 0.35f, Pitch = 1.5f, PitchVariance = 0.75f };

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
        }

		public override bool? CanDamage()
		{
			return false;
		}

        public override void AI()
        {
			Projectile.frame = (int)Projectile.ai[1];

			NPC SelectedTownNPC = Main.npc[(int)Projectile.ai[2]];

			SelectedTownNPC.velocity.X = 0;

			Projectile.ai[0]++;
			if (Projectile.ai[0] == 1)
			{
				//set downed variables to true properly
				if (!Flags.KrampusQuest1)
				{
					Flags.KrampusQuest1 = true;
				}
				else if (Flags.KrampusQuest1 && !Flags.KrampusQuest2)
				{
					Flags.KrampusQuest2 = true;
				}
				else if (Flags.KrampusQuest2 && !Flags.KrampusQuest3)
				{
					Flags.KrampusQuest3 = true;
				}
				else if (Flags.KrampusQuest3 && !Flags.KrampusQuest4)
				{
					Flags.KrampusQuest4 = true;
				}

				if (Projectile.ai[1] == 3)
				{
					if (Flags.KrampusQuest4 && !Flags.KrampusQuest5)
					{
						Flags.KrampusQuest5 = true;
					}

					//set daily quest to true if the main questline is done
					if (Flags.KrampusQuestlineDone)
					{
						Flags.KrampusDailyQuestDone = true;
					}
				}

				//when the gift is used, the quest is no longer in progress
				Flags.KrampusQuestGiven = false;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}
			if (Projectile.ai[0] <= 60)
			{
				if (Shake)
				{
					Projectile.rotation += 0.1f;
					if (Projectile.rotation > 0.2f)
					{
						Shake = false;
						Projectile.netUpdate = true;
					}
				}
				else
				{
					Projectile.rotation -= 0.1f;
					if (Projectile.rotation < -0.2f)
					{
						Shake = true;
						Projectile.netUpdate = true;
					}
				}
			}
			else
			{
				SoundEngine.PlaySound(SoundID.ResearchComplete with { Pitch = 1f }, Projectile.Center);

				if (!Flags.KrampusQuestlineDone)
				{
					int TypeToSpawn = ModContent.NPCType<Marble>();

					if (Flags.KrampusQuest2 && !Flags.KrampusQuest3)
					{
						TypeToSpawn = ModContent.NPCType<JackInTheBox>();
					}
					else if (Flags.KrampusQuest3 && !Flags.KrampusQuest4)
					{
						TypeToSpawn = ModContent.NPCType<BuilderBot>();
					}
					else if (Flags.KrampusQuest4 && !Flags.KrampusQuest5)
					{
						TypeToSpawn = ModContent.NPCType<ChefRobot>();
					}
					else if (Flags.KrampusQuest5)
					{
						TypeToSpawn = ModContent.NPCType<TeddyBearGiant>();
					}

					int NPCSpawn = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, TypeToSpawn);

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.SyncNPC, number: NPCSpawn);
					}
				}

				for (int numGores = 1; numGores <= 12; numGores++)
				{
					if (Main.netMode != NetmodeID.Server)
					{
						Gore.NewGore(null, Projectile.position, new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, -2)), Main.rand.Next(276, 283));
					}
				}

				if (SelectedTownNPC.type == ModContent.NPCType<LittleEye>())
				{
					if (!Flags.KrampusQuestlineDone)
					{
						if (!Main.dedServ)
						{
							DialogueChain chain = new();
							chain.Add(new(ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUILittleEye").Value, SelectedTownNPC,
							Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.LittleEye1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerLittleEye1"),
							TalkSound, 2f, 0f, modifier))
							.Add(new(ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUILittleEye").Value, SelectedTownNPC,
							Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.LittleEye2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerLittleEye2"),
							TalkSound, 2f, 0f, modifier))
							.Add(new(ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUILittleEye").Value, SelectedTownNPC,
							Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.LittleEye3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.KrampusDialogue.PlayerLittleEye3"),
							TalkSound, 2f, 0f, modifier))
							.Add(new(ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUILittleEye").Value, SelectedTownNPC, null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogue;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}
					}
					else
					{
						int[] NPCsList = new int[] { ModContent.NPCType<Marble>(), ModContent.NPCType<JackInTheBox>(), ModContent.NPCType<BuilderBot>(), ModContent.NPCType<ChefRobot>() };

						int NPCSpawn = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y, Main.rand.Next(NPCsList));

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: NPCSpawn);
						}
					}
				}
				else
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						SelectedTownNPC.StrikeInstantKill();
					}
				}

				Projectile.Kill();
			}
        }

		private void PlayerResponse(Dialogue dialogue, string Text, int ID)
		{
			Dialogue newDialogue = new(ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUIPlayer").Value, Main.LocalPlayer, Text, null, SoundID.Item1, 2f, 0f, default, NotPlayer: false);
			DialogueUI.Visible = true;
			DialogueUI.Add(newDialogue);
		}

		private void EndDialogue(Dialogue dialogue, int ID)
		{
			DialogueUI.Visible = false;
		}
    }
}