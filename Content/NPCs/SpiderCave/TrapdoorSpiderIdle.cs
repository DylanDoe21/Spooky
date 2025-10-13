using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpiderCave
{
	public class TrapdoorSpiderIdle1 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 350;
            NPC.damage = 45;
            NPC.defense = 25;
			NPC.width = 74;
			NPC.height = 22;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit29;
			NPC.DeathSound = SoundID.NPCDeath31;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

        public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

			if (NPC.Distance(player.Center) <= 200f || NPC.life < NPC.lifeMax || player.GetModPlayer<SpookyPlayer>().WhipSpiderAggression)
            {
				SoundEngine.PlaySound(SoundID.Zombie74, NPC.Center);

				int AwakeSpider = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TrapdoorSpider1>());
				Main.npc[AwakeSpider].velocity.Y = -5;

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
                BestiaryParent.SetDefaults(ModContent.NPCType<TrapdoorSpider1>());
                Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);
            }
        }
    }

	public class TrapdoorSpiderIdle2 : TrapdoorSpiderIdle1
	{
		public override void SetDefaults()
		{
			NPC.lifeMax = 500;
            NPC.damage = 45;
            NPC.defense = 25;
			NPC.width = 62;
			NPC.height = 16;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit29;
			NPC.DeathSound = SoundID.NPCDeath31;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

        public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

			if (NPC.Distance(player.Center) <= 200f || NPC.life < NPC.lifeMax || player.GetModPlayer<SpookyPlayer>().WhipSpiderAggression)
            {
				SoundEngine.PlaySound(SoundID.Zombie74, NPC.Center);

				int AwakeSpider = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<TrapdoorSpider2>());
				Main.npc[AwakeSpider].velocity.Y = -5;

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
                BestiaryParent.SetDefaults(ModContent.NPCType<TrapdoorSpider2>());
                Main.BestiaryTracker.Kills.RegisterKill(BestiaryParent);
            }
        }
	}
}