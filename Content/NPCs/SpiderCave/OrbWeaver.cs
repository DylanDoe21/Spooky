using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpiderCave
{
	public class OrbWeaver1 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 75;
            NPC.damage = 20;
			NPC.defense = 20;
			NPC.width = 74;
			NPC.height = 52;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.NPCDeath32;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OrbWeaver1"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            if (NPC.localAI[2] > 0)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
		}

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[2] == 0)
            {
                NPC.aiStyle = 3;
			    AIType = NPCID.Crab;
            }
            else
            {
                NPC.aiStyle = 0;
            }

            if (NPC.localAI[0] >= 600 && Vector2.Distance(player.Center, NPC.Center) <= 200)
            {
                if (NPC.localAI[2] == 0)
                {
                    //shoot spikes up here

                    NPC.localAI[2] = 1;
                }

                //cooldown before the spikes grow back
                NPC.localAI[1]++;
                if (NPC.localAI[1] >= 75)
                {
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OrbWeaverWhiteGore" + numGores).Type);
                    }
                }
            }
        }
	}

    public class OrbWeaver2 : OrbWeaver1
	{
		public override void SetDefaults()
		{
            NPC.lifeMax = 90;
            NPC.damage = 20;
			NPC.defense = 20;
			NPC.width = 82;
			NPC.height = 56;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.NPCDeath32;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OrbWeaver2"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OrbWeaverYellowGore" + numGores).Type);
                    }
                }
            }
        }
    }

    public class OrbWeaver3 : OrbWeaver1
	{
		public override void SetDefaults()
		{
            NPC.lifeMax = 90;
            NPC.damage = 20;
			NPC.defense = 20;
			NPC.width = 62;
			NPC.height = 46;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.NPCDeath32;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OrbWeaver3"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OrbWeaverBrownGore" + numGores).Type);
                    }
                }
            }
        }
    }

    public class OrbWeaverGiant : OrbWeaver1
	{
		public override void SetDefaults()
		{
            NPC.lifeMax = 900;
            NPC.damage = 55;
			NPC.defense = 40;
			NPC.width = 82;
			NPC.height = 60;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 25, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.NPCDeath32;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OrbWeaverGiant"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OrbWeaverGiantGore" + numGores).Type);
                    }
                }
            }
        }
    }
}