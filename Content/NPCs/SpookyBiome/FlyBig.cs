using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class FlyBig : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Giant Fly");
			Main.npcFrameCount[NPC.type] = 3;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 10;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 28;
			NPC.height = 26;
            NPC.npcSlots = 1;
            NPC.noGravity = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath47;
			NPC.aiStyle = 114;
			AIType = NPCID.BlackDragonfly;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Bigger and grosser flies that linger around in the spooky forest. Their wings look too small to allow flight, but they do anyways."),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<Biomes.SpookyBiome>()) && Main.dayTime)
                {
                    return 6f;
                }
            }

            return 0f;
        }
        
        public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 1;
            if (NPC.frameCounter > 3)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0;
            }
		}

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.07f;
        }

        public override bool CheckDead() 
		{
			for (int numDusts = 0; numDusts < 15; numDusts++)
            {
                int DustGore = Dust.NewDust(NPC.Center, NPC.width / 2, NPC.height / 2, DustID.Asphalt, 0f, 0f, 100, default, 2f);
                Main.dust[DustGore].scale = 0.75f;
                Main.dust[DustGore].velocity *= 1.2f;
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[DustGore].scale = 0.75f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }

			return true;
		}
	}
}