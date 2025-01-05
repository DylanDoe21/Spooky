using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Buffs;
using Spooky.Content.Dusts;
using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.NPCs.NoseCult
{
	public class NoseCultistLeaderIdle : ModNPC
	{
		public static readonly SoundStyle SneezeSound = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound);

		public override void SetStaticDefaults()
		{	
			Main.npcFrameCount[NPC.type] = 9;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;
			NPCID.Sets.ShimmerTownTransform[Type] = false;
			NPCID.Sets.NoTownNPCHappiness[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 5;
			NPC.damage = 0;
			NPC.defense = 0;
            NPC.width = 108;
			NPC.height = 124;
			NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
		}

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 9)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override bool CheckActive()
        {
            return false;
        }

		public override bool CanChat()
		{
			return Main.LocalPlayer.GetModPlayer<SpookyPlayer>().NoseCultistDisguise1 && Main.LocalPlayer.GetModPlayer<SpookyPlayer>().NoseCultistDisguise2;
		}

		public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("Mods.Spooky.Dialogue.NoseLeader.Button");
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shopName)
		{
			if (firstButton)
			{
				if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<SnotGlob>()))
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.NoseLeader.NoseBlessing");

					SoundEngine.PlaySound(SneezeSound, NPC.Center);

					Main.LocalPlayer.AddBuff(ModContent.BuffType<NoseBlessingBuff>(), 36000);

					float maxAmount = 30;
					int currentAmount = 0;
					while (currentAmount <= maxAmount)
					{
						Vector2 velocity = new Vector2(5f, 5f);
						Vector2 Bounds = new Vector2(3f, 3f);
						float intensity = 5f;

						Vector2 vector12 = Vector2.UnitX * 0f;
						vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
						vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
						int num104 = Dust.NewDust(Main.LocalPlayer.Center, 0, 0, DustID.KryptonMoss, 0f, 0f, 100, default, 3f);
						Main.dust[num104].noGravity = true;
						Main.dust[num104].position = Main.LocalPlayer.Center + vector12;
						Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
						currentAmount++;
					}
				}
				else
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.NoseLeader.NoseBlessingBroke");
				}
			}
		}

		public override string GetChat()
		{
			return Language.GetTextValue("Mods.Spooky.Dialogue.NoseLeader.Dialogue" + Main.rand.Next(1, 7));
		}

		

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			NPC.spriteDirection = NPC.Center.X > Parent.Center.X ? -1 : 1;

			if (!Parent.active && Parent.type != ModContent.NPCType<MocoIdol6>())
			{
				NPC.active = false;
			}

			if (Parent.ai[1] == 1)
			{
				NPC.ai[1]++;

				if (NPC.ai[1] == 30)
				{
					Dust.NewDustPerfect(new Vector2(NPC.Center.X, NPC.Center.Y - NPC.height), ModContent.DustType<CultistExclamation>(), Vector2.Zero, 0, default, 2f);
				}

				if (NPC.ai[1] >= 60)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int SpawnedNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + NPC.height / 2, ModContent.NPCType<NoseCultistLeader>(), ai3: NPC.ai[0]);
						Main.npc[SpawnedNPC].alpha = 255;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: SpawnedNPC);
						}
					}

					NPC.active = false;
				}
			}
        }
    }
}