using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.NPCs.NoseCult
{
	public class NoseCultistWingedIdle : ModNPC
	{
		public override void SetStaticDefaults()
		{	
			Main.npcFrameCount[NPC.type] = 3;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 5;
			NPC.damage = 0;
			NPC.defense = 0;
            NPC.width = 48;
			NPC.height = 64;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
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

			if (Parent.ai[1] == 1)
			{
				//int SpawnedNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + NPC.height / 2, ModContent.NPCType<NoseCultistWinged>());
                    
				//NetMessage.SendData(MessageID.SyncNPC, number: SpawnedNPC);

				NPC.active = false;
			}
        }
    }
}