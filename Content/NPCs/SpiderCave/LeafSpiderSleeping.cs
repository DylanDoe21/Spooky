using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpiderCave
{
	public class LeafSpiderSleeping : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;

			NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 90;
            NPC.damage = 25;
			NPC.defense = 15;
			NPC.width = 74;
			NPC.height = 22;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit32;
			NPC.DeathSound = SoundID.NPCDeath35;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 1;
            if (NPC.frameCounter > 8)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0;
            }
		}

        public override void AI()
		{
			Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.direction;

			if (NPC.Distance(player.Center) <= 200f || NPC.life < NPC.lifeMax || player.GetModPlayer<SpookyPlayer>().WhipSpiderAggression)
            {
				int AwakeSpider = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<LeafSpider>());

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{   
					NetMessage.SendData(MessageID.SyncNPC, number: AwakeSpider);
				}

				NPC.netUpdate = true;
				NPC.active = false;
			}
        }

		public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                NPC BestiaryParent = new();
                BestiaryParent.SetDefaults(ModContent.NPCType<LeafSpider>());
                Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);
            }
        }
    }
}