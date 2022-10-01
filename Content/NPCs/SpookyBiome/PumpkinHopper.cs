using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class PumpkinHopper : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hopping Gourd");
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 35;
            NPC.damage = 20;
            NPC.width = 28;
			NPC.height = 30;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.75f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Small, fragile little gourds that come to life at night. Due to their small size, they can disguise themselves with other pumpkins."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                //spawn on the surface during the night
                if (player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiome>()) && !Main.dayTime)
                {
                    return 25f;
                }
            }

            return 0f;
        }
        
        public override void AI()
		{
			Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.04f;

            NPC.ai[0]++;

			if (!NPC.collideY) 
            {
				NPC.velocity.X *= 1.045f;
			}
			if (NPC.velocity.Y == 0)
			{
				if (NPC.ai[0] >= 120)
				{
					NPC.velocity.Y = -7;
					NPC.velocity.X = NPC.direction * 5;
                    NPC.ai[0] = 0;
				}
            }
        }

        public override bool CheckDead() 
		{
            if (Main.netMode != NetmodeID.Server) 
            {
                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PumpkinHopperGore" + numGores).Type);
                }
            }

            for (int numDust = 0; numDust < 20; numDust++)
            {
                int DustGore = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y), NPC.width / 2, NPC.height / 2, 288, 0f, 0f, 100, default, 2f);

                Main.dust[DustGore].velocity *= 3f;
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }

            return true;
		}
    }
}