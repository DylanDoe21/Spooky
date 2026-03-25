using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class PuttyPumpkin : ModNPC
	{
        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 2;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 200;
            NPC.damage = 40;
            NPC.defense = 5;
            NPC.width = 44;
            NPC.height = 34;
			NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.35f;
			NPC.value = Item.buyPrice(0, 0, 1, 50);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 1;
			AIType = NPCID.HoppinJack;
			AnimationType = NPCID.HoppinJack;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PuttyPumpkin"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 1, 1, 3));
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
			//randomly spawn pumpkinlings on hit
            if (Main.rand.NextBool(3))
            {
                int Pumpkin = NPC.NewNPC(NPC.GetSource_OnHit(NPC), (int)NPC.Center.X + Main.rand.Next(-20, 20), (int)NPC.Center.Y + Main.rand.Next(-10, 5), ModContent.NPCType<PuttyPumpkinling>());

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: Pumpkin);
				}
            }

            if (NPC.life <= 0) 
            {
				//spawn more pumpkinlings on death
				for (int numSlimes = 0; numSlimes < 3; numSlimes++)
				{
					int Pumpkin = NPC.NewNPC(NPC.GetSource_OnHit(NPC), (int)NPC.Center.X + Main.rand.Next(-20, 20), (int)NPC.Center.Y + Main.rand.Next(-10, 5), ModContent.NPCType<PuttyPumpkinling>());

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						NetMessage.SendData(MessageID.SyncNPC, number: Pumpkin);
					}
				}

                for (int numDusts = 0; numDusts < 15; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.Center, 1, 1, DustID.TintableDust, 0f, 0f, 100, Color.Orange, 1f);
                    Main.dust[DustGore].velocity *= 1.2f;
                }
            }
		}
    }
}