using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Cemetery
{
	public class Glooper1 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Glooper");
			Main.npcFrameCount[NPC.type] = 4;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 35;
            NPC.damage = 20;
			NPC.defense = 0;
			NPC.width = 12;
			NPC.height = 24;
            NPC.npcSlots = 1;
            NPC.noGravity = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = 67;
            AIType = NPCID.Snail;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Peaceful slimes that slide across the ground and walls. While they are not hostile, touching them may not be so pleasant."),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                if (player.InModBiome(ModContent.GetInstance<Biomes.CemeteryBiome>()))
                {
                    return 10f;
                }
            }

            return 0f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff(BuffID.OgreSpit, 300);
        }

        public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 1;
            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0;
            }
		}

        public override void HitEffect(int hitDirection, double damage) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDust = 0; numDust < 35; numDust++)
                {                                                                                  
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.KryptonMoss, 0f, -2f, 0, default(Color), 1.5f);
                    Main.dust[DustGore].noGravity = true;
                    Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;

                    if (Main.dust[DustGore].position != NPC.Center)
                    {
                        Main.dust[DustGore].velocity = NPC.DirectionTo(Main.dust[DustGore].position) * 2f;
                    }
                }
            }
        }
	}

    public class Glooper2 : Glooper1
	{
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Friendly blobs of venomous goo that wander around the cemetery. Make sure not to accidentally step on them!"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(int hitDirection, double damage) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDust = 0; numDust < 35; numDust++)
                {                                                                                  
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.ArgonMoss, 0f, -2f, 0, default(Color), 1.5f);
                    Main.dust[DustGore].noGravity = true;
                    Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;

                    if (Main.dust[DustGore].position != NPC.Center)
                    {
                        Main.dust[DustGore].velocity = NPC.DirectionTo(Main.dust[DustGore].position) * 2f;
                    }
                }
            }
        }
    }
}