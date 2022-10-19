using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class FluffBatSmall1 : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fuzz Bat");
            Main.npcFrameCount[NPC.type] = 4;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 30;
            NPC.damage = 18;
            NPC.width = 38;
			NPC.height = 26;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.8f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath4;
            NPC.aiStyle = 14;
			AIType = NPCID.GiantBat;
            AnimationType = NPCID.GiantBat;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyBiome>().Type,
            ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Cute and not very friendly, these bats dwell in the spooky forest, swarming any intruder."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                if ((player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiome>()) && !Main.dayTime) ||
                player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiomeUg>()))
                {
                    return 12f;
                }
            }

            return 0f;
        }

        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
        }

        /*
        public override void HitEffect(int hitDirection, double damage) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/FluffBatSmallOrange" + numGores).Type);
                }
            }
        }
        */
    }

    public class FluffBatSmall2 : FluffBatSmall1  
    {
        /*
        public override void HitEffect(int hitDirection, double damage) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/FluffBatSmallPurple" + numGores).Type);
                }
            }
        }
        */
    }
}