using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Cemetery
{
    public class ZomboidCasket : ModNPC  
    {
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/WoodBreaking", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 55;
            NPC.damage = 20;
            NPC.defense = 10;
            NPC.width = 42;
			NPC.height = 58;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.25f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.Dig;
			NPC.DeathSound = DeathSound;
            NPC.aiStyle = 3;
			AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidCasket"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {   
            //running animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //jumping frame
            if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 2 * frameHeight;
            }
        }
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numDust = 0; numDust < 12; numDust++)
                {                                                                                  
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.WoodFurniture, 0f, -2f, 0, default, 1.5f);
                    Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                }

                for (int numZombinos = 0; numZombinos <= 2; numZombinos++)
                {
                    int Zombino = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-30, 30), 
                    (int)NPC.Center.Y - Main.rand.Next(10, 30), ModContent.NPCType<ZomboidGremlin>());

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {  
                        NetMessage.SendData(MessageID.SyncNPC, number: Zombino);
                    }
                }

                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidCasketGore" + numGores).Type);
                    }
                }
            }
        }
    }
}