using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Minibiomes.Christmas.Projectiles;

namespace Spooky.Content.NPCs.Minibiomes.Christmas
{
    public class TeddyBearSnow : ModNPC  
    {
        bool Spawned = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(Spawned);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            Spawned = reader.ReadBoolean();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 100;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.width = 24;
			NPC.height = 36;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.2f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit15;
			NPC.DeathSound = SoundID.NPCDeath15;
            NPC.aiStyle = 3;
            AIType = NPCID.Crab;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().Type };
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TeddyBearSnow"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 6)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //jumping/falling frame
            if (NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 3 * frameHeight;
            }
            if (NPC.velocity.Y > 0)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            if (!Spawned)
            {
                for (int numBears = 0; numBears < 3; numBears++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int[] NPCs = { ModContent.NPCType<TeddyBear1>(), ModContent.NPCType<TeddyBear2>(), ModContent.NPCType<TeddyBear3>() };

                        int TeddyBear = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, Main.rand.Next(NPCs), ai0: NPC.whoAmI);
                        Main.npc[TeddyBear].velocity = new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-3, 0));

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: TeddyBear);
                        }
                    }
                }

                Spawned = true;
                NPC.netUpdate = true;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, Vector2.Zero, ModContent.ProjectileType<SnowCloud>(), NPC.damage, 0f);

                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        //Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CornGore" + numGores).Type);
                    }
                }
            }
        }
    }
}