using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class RootZombie1 : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Root Walker");
            Main.npcFrameCount[NPC.type] = 5;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 45;
            NPC.damage = 15;
            NPC.defense = 5;
            NPC.width = 25;
			NPC.height = 50;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 3;
			AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyBiome>().Type,
            ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("A rotten, animated corpse that has strange pumpkin roots coming out of it's body.")
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                //spawn on the surface during the day, or underground
                if ((player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiome>()) && Main.dayTime) ||
                player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiomeUg>()))
                {
                    return 25f;
                }
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {   
            NPC.frameCounter += 1;
            //running animation
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //jumping frame
            if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
        }
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
        }

        public override bool CheckDead() 
		{
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/RootZombieGore1").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/RootZombieGore3").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/RootZombieGore4").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/RootZombieGore5").Type);

            return true;
		}
    }
}