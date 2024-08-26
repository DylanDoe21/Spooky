using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Linq;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.NoseCult
{
	public class NoseCultistGruntIdle : ModNPC
	{
		public override void SetStaticDefaults()
		{	
			Main.npcFrameCount[NPC.type] = 3;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;
			
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 5;
			NPC.damage = 0;
			NPC.defense = 0;
            NPC.width = 34;
			NPC.height = 42;
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
            if (NPC.frameCounter > 15)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
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

        public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			NPC.spriteDirection = NPC.Center.X > Parent.Center.X ? -1 : 1;

			int[] Idols = { ModContent.NPCType<MocoIdol1>(), ModContent.NPCType<MocoIdol2>(), ModContent.NPCType<MocoIdol3>(), ModContent.NPCType<MocoIdol4>(), ModContent.NPCType<MocoIdol5>() };

			if (!Parent.active || !Idols.Contains(Parent.type))
			{
				NPC.active = false;
			}

			if (Parent.ai[1] == 1)
			{
				NPC.ai[1]++;

				if (NPC.ai[1] == 30)
				{
					Dust.NewDustPerfect(new Vector2(NPC.Center.X, NPC.Center.Y - NPC.height), ModContent.DustType<CultistExclamation>(), Vector2.Zero, 0, default, 1f);
				}

				if (NPC.ai[1] >= 60)
				{
					int SpawnedNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + NPC.height / 2, ModContent.NPCType<NoseCultistGrunt>());
						
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.SyncNPC, number: SpawnedNPC);
					}

					NPC.active = false;
				}
			}
        }
    }
}