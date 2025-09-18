using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.NPCs.Tameable
{
	public class LittleDunkEggNPC : ModNPC
	{
		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 250;
			NPC.damage = 0;
			NPC.defense = 0;
            NPC.width = 44;
			NPC.height = 40;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override bool NeedSaving()
		{
			return true;
		}

		public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
		{
			if (NPC.wet)
			{
				NPC.ai[0]++;

				//7200 = 2 minutes
				if (NPC.ai[0] >= 7200)
				{
					Main.BestiaryTracker.Chats.SetWasChatWithDirectly(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[ModContent.NPCType<LittleDunk>()]);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int LittleDunk = NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LittleDunk>());

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: LittleDunk);
						}
					}

					NPC.life = 0;

					if (Main.netMode == NetmodeID.Server) 
					{
						NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI, 0f, 0f, 0f, 0);
					}
				}
			}
        }
    }
}