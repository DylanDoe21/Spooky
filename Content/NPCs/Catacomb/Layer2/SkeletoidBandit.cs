using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Costume;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
    public class SkeletoidBandit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 400;
            NPC.damage = 35;
            NPC.defense = 5;
            NPC.width = 52;
			NPC.height = 54;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.25f;
            NPC.value = Item.buyPrice(0, 0, 25, 0);
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.aiStyle = 3;
            AIType = NPCID.Crab;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SkeletoidBandit"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            //walking animation
            if (NPC.localAI[0] == 0)
            {
                if (NPC.frameCounter > 9)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //frame when falling/jumping
                if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
            }
            //shooting animation
            if (NPC.localAI[0] == 1)
            {
                if (NPC.frame.Y < frameHeight * 6)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }

                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            int Damage = Main.masterMode ? 40 / 3 : Main.expertMode ? 30 / 2 : 20;

            switch ((int)NPC.localAI[0])
            {
                case 0:
                {
                    NPC.aiStyle = 3;
                    AIType = NPCID.Crab;

                    if (player.Distance(NPC.Center) <= 250f || NPC.localAI[1] >= 80)
                    {
                        NPC.localAI[1]++;
                    }

                    if (NPC.localAI[1] >= 180)
                    {
                        NPC.localAI[1] = 0;
                        NPC.localAI[0]++;
                    }

                    break;
                }

                case 1:
                {
                    NPC.aiStyle = 0;
                    
                    NPC.localAI[1]++;
                    if (NPC.localAI[1] == 60)
                    {
                        Vector2 ShootSpeed = player.Center - NPC.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed.X *= 35;
                        ShootSpeed.Y *= 0;

                        Vector2 positonToShootFrom = new Vector2(NPC.Center.X + (NPC.direction == -1 ? -20 : 20), NPC.Center.Y + 2);

                        NPCGlobalHelper.ShootHostileProjectile(NPC, positonToShootFrom, ShootSpeed, ProjectileID.BulletSnowman, NPC.damage, 4.5f);
                    }

                    if (NPC.localAI[1] >= 120)
                    {
                        NPC.localAI[1] = 0;
                        NPC.localAI[0] = 0;
                    }

                    break;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BanditHat>(), 5));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SkeletoidBanditGore" + numGores).Type);
                    }
                }

                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SkeletoidBanditCloth" + Main.rand.Next(1, 3)).Type);
                    }
                }
            }
        }
    }
}