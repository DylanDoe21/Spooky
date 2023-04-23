using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class TinyGhost1 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 2;
			Main.npcCatchable[NPC.type] = true;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 18;
			NPC.height = 24;
            NPC.npcSlots = 1;
            NPC.noGravity = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.aiStyle = 64;
			AIType = NPCID.Firefly;
            NPC.catchItem = (short)ModContent.ItemType<TinyGhostItem>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TinyGhost"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                if (player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiome>()) && !Main.dayTime)
                {
                    return 8f;
                }
            }

            return 0f;
        }
        
        public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 1;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 2)
            {
                NPC.frame.Y = 0;
            }
		}

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.07f;

            Lighting.AddLight(NPC.position, 0.2f, 0.2f, 0.2f);
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int GhostDust = Dust.NewDust(NPC.Center, NPC.width / 2, NPC.height / 2, DustID.GemDiamond, 0f, 0f, 100, default, 2f);
                    Main.dust[GhostDust].scale = 0.5f;
                    Main.dust[GhostDust].velocity *= 1.2f;
                    Main.dust[GhostDust].noGravity = true;

                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[GhostDust].scale = 0.5f;
                        Main.dust[GhostDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
		}
	}

    public class TinyGhost2 : TinyGhost1
	{
    }
}