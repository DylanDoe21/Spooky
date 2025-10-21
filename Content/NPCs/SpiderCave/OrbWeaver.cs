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
using Spooky.Content.Items.SpiderCave;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.NPCs.SpiderCave.Projectiles;

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
            NPC.damage = 30;
			NPC.defense = 15;
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
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            if (Vector2.Distance(player.Center, NPC.Center) <= 450f || NPC.localAI[0] >= 250)
            {
                NPC.localAI[0]++;
            }

            if (NPC.localAI[0] <= 250)
            {
                NPC.aiStyle = 3;
			    AIType = NPCID.Crab;
            }
            else
            {
                NPC.aiStyle = 0;
            }

            if (NPC.localAI[0] >= 300)
            {
                //spawn two separate spreads of spike projectiles so it looks like they are spawning from the actual spikes on the orb weaver
                if (NPC.localAI[1] == 0)
                {
                    ShootSpikes(ModContent.ProjectileType<WeaverSpikeRed>(), 8f);

                    NPC.localAI[1] = 1;
                }
            }

            //cooldown before the spikes grow back
            if (NPC.localAI[1] == 1)
            {
                NPC.localAI[2]++;
                if (NPC.localAI[2] >= 75)
                {
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;
                }
            }
        }

        public void ShootSpikes(int Type, float Velocity)
        {
            SoundEngine.PlaySound(SoundID.Item17, NPC.Center);

            for (int numProjectiles = -3; numProjectiles <= -1; numProjectiles++)
            {
                NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X - 20, NPC.Center.Y), 
                Velocity * NPC.DirectionTo(new Vector2(NPC.Center.X, NPC.Center.Y - 100)).RotatedBy(MathHelper.ToRadians(10) * numProjectiles), Type, NPC.damage, 4.5f);
            }

            for (int numProjectiles = 1; numProjectiles <= 3; numProjectiles++)
            {
                NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + 20, NPC.Center.Y), 
                Velocity * NPC.DirectionTo(new Vector2(NPC.Center.X, NPC.Center.Y - 100)).RotatedBy(MathHelper.ToRadians(10) * numProjectiles), Type, NPC.damage, 4.5f);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiderChitin>(), 3, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OrbWeaverShield>(), 8));
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
            NPC.damage = 35;
			NPC.defense = 15;
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

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            if (Vector2.Distance(player.Center, NPC.Center) <= 450f || NPC.localAI[0] >= 250)
            {
                NPC.localAI[0]++;
            }

            if (NPC.localAI[0] <= 250)
            {
                NPC.aiStyle = 3;
			    AIType = NPCID.Crab;
            }
            else
            {
                NPC.aiStyle = 0;
            }

            if (NPC.localAI[0] >= 300)
            {
                //spawn two separate spreads of spike projectiles so it looks like they are spawning from the actual spikes on the orb weaver
                if (NPC.localAI[1] == 0)
                {
                    ShootSpikes(ModContent.ProjectileType<WeaverSpikeBlack>(), 10f);

                    NPC.localAI[1] = 1;
                }
            }
            
            //cooldown before the spikes grow back
            if (NPC.localAI[1] == 1)
            {
                NPC.localAI[2]++;
                if (NPC.localAI[2] >= 75)
                {
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiderChitin>(), 3, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OrbWeaverStaff>(), 8));
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
            NPC.damage = 30;
			NPC.defense = 15;
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

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            if (Vector2.Distance(player.Center, NPC.Center) <= 450f || NPC.localAI[0] >= 250)
            {
                NPC.localAI[0]++;
            }

            if (NPC.localAI[0] <= 250)
            {
                NPC.aiStyle = 3;
			    AIType = NPCID.Crab;
            }
            else
            {
                NPC.aiStyle = 0;
            }

            if (NPC.localAI[0] >= 300)
            {
                //spawn two separate spreads of spike projectiles so it looks like they are spawning from the actual spikes on the orb weaver
                if (NPC.localAI[1] == 0)
                {
                    ShootSpikes(ModContent.ProjectileType<WeaverSpikeGreen>(), 7f);

                    NPC.localAI[1] = 1;
                }
            }

            //cooldown before the spikes grow back
            if (NPC.localAI[1] == 1)
            {
                NPC.localAI[2]++;
                if (NPC.localAI[2] >= 75)
                {
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiderChitin>(), 3, 1, 3));
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
            NPC.damage = 60;
			NPC.defense = 30;
			NPC.width = 82;
			NPC.height = 60;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
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

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            if (Vector2.Distance(player.Center, NPC.Center) <= 550f || NPC.localAI[0] >= 250)
            {
                NPC.localAI[0]++;
            }

            if (NPC.localAI[0] <= 250)
            {
                NPC.aiStyle = 3;
			    AIType = NPCID.Crab;
            }
            else
            {
                NPC.aiStyle = 0;
            }

            if (NPC.localAI[0] >= 300)
            {
                //spawn two separate spreads of spike projectiles so it looks like they are spawning from the actual spikes on the orb weaver
                if (NPC.localAI[1] == 0)
                {
                    ShootSpikes(ModContent.ProjectileType<WeaverSpikeGiant>(), 13f);

                    NPC.localAI[1] = 1;
                }
            }

            //cooldown before the spikes grow back
            if (NPC.localAI[1] == 1)
            {
                NPC.localAI[2]++;
                if (NPC.localAI[2] >= 75)
                {
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OrbWeaverBoomerang>(), 8));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OrbWeaverGiantStaff>(), 8));
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