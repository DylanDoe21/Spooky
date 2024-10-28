using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Dusts;
using Spooky.Content.Items.Food;
using Spooky.Content.Items.Pets;
using Spooky.Content.Tiles.Blooms;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
	public class PollinatorBeeDamage : ModNPC
	{
        public int SaveDirection;

        public static List<int> BuffableNPCs = new List<int>() 
        {
            ModContent.NPCType<FlushBush1>(),
            ModContent.NPCType<FlushBush2>(),
            ModContent.NPCType<JumpingSeed1>(),
            ModContent.NPCType<JumpingSeed2>(),
            ModContent.NPCType<JumpingSeed3>(),
            ModContent.NPCType<LilySlime1Big>(),
            ModContent.NPCType<LilySlime1Small>(),
            ModContent.NPCType<LilySlime2Big>(),
            ModContent.NPCType<LilySlime2Small>(),
            ModContent.NPCType<OrchidPinkBig>(),
            ModContent.NPCType<OrchidPinkSmall>(),
            ModContent.NPCType<OrchidPurpleBig>(),
            ModContent.NPCType<OrchidPurpleSmall>(),
            ModContent.NPCType<PitcherPlant1>(),
            ModContent.NPCType<PitcherPlant2>(),
            ModContent.NPCType<PitcherPlant3>(),
            ModContent.NPCType<PitcherPlant4>(),
            ModContent.NPCType<Smelly>(),
            ModContent.NPCType<Sunflower1>(),
            ModContent.NPCType<Sunflower2>(),
            ModContent.NPCType<Sunflower3>()
        };

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/BeeHit", SoundType.Sound) { Volume = 0.8f };
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/BeeDeath", SoundType.Sound) { PitchVariance = 0.6f };

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 200;
            NPC.damage = 30;
			NPC.defense = 0;
			NPC.width = 55;
			NPC.height = 60;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCDeath35;
			NPC.DeathSound = DeathSound;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PollinatorBeeDamage"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;
            if (NPC.frameCounter > 1)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 6)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(HitSound, NPC.Center);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(HitSound, NPC.Center);
        }

        public override void AI()
        {
            NPC.rotation = NPC.velocity.X * 0.07f;
            
            switch ((int)NPC.ai[0])
            {
                //fly around back and fourth aimlessly
                case 0:
                {
                    NPC.ai[1]++;

                    //change y-velocity randomly at certain intervals
                    if (NPC.ai[1] >= 15)
                    {
                        NPC.velocity.Y += Main.rand.NextFloat(-0.4f, 0.4f);

                        NPC.ai[2] = 0;
                    }

                    //constantly increase x-velocity
                    NPC.velocity.X += NPC.spriteDirection == 1 ? 0.1f : -0.1f;

                    //cap velocity so it doesnt start zooming around
                    NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -3, 3);
                    NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -2, 2);

                    //bounce off of walls and turn around
                    if (NPC.ai[2] == 0 && NPC.collideX)
                    {
                        NPC.velocity = -NPC.oldVelocity;

                        NPC.spriteDirection = -NPC.spriteDirection;

                        NPC.ai[2] = 1;
                    }

                    //go to pollinating attack after 5 seconds
                    if (NPC.ai[1] >= 420)
                    {
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                    }

                    break;
                }

                //pollinate an enemy
                case 1:
                {
                    //find an npc to buff
                    if (NPC.ai[1] == 0)
                    {
                        for (int i = 0; i <= Main.maxNPCs; i++)
                        {
                            NPC TargetNPC = Main.npc[i];

                            //if a valid npc is found, save its whoAmI and increase ai to start pollinating it
                            if (BuffableNPCs.Contains(TargetNPC.type) && TargetNPC.active && !TargetNPC.HasBuff(ModContent.BuffType<BeeDamageBuff>()) && !TargetNPC.HasBuff(ModContent.BuffType<BeeHealingBuff>()) && 
                            !TargetNPC.HasBuff(ModContent.BuffType<BeePollinationProcessBuff>()) && Vector2.Distance(NPC.Center, TargetNPC.Center) <= 500f)
                            {
                                NPC.ai[3] = TargetNPC.whoAmI;
                                NPC.ai[1]++;

                                break;
                            }
                            else
                            {
                                if (i >= Main.maxNPCs)
                                {
                                    NPC.ai[0] = 0;
                                    NPC.ai[1] = 0;
                                    NPC.ai[2] = 0;
                                    NPC.ai[3] = 0;
                                }
                            }
                        }
                    }
                    //when an npc is found, go to it, produce pollen clouds, and then buff it
                    else
                    {
                        //give the npc the bee pollination process buff
                        //this buff does absolutely nothing, but is used to prevent multiple bees from trying to pollinate the same enemy at once
                        Main.npc[(int)NPC.ai[3]].AddBuff(ModContent.BuffType<BeePollinationProcessBuff>(), 2);

                        if (NPC.ai[1] <= 1)
                        {
                            NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;
                            SaveDirection = NPC.spriteDirection;
                        }
                        
                        //go to the npc center
                        Vector2 GoTo = Main.npc[(int)NPC.ai[3]].Center;
                        GoTo.Y -= 50;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                        if (Vector2.Distance(NPC.Center, Main.npc[(int)NPC.ai[3]].Center) <= 120f && NPC.ai[1] <= 1)
                        {
                            NPC.ai[1]++;
                        }

                        //if the bee is unable to reach its target after a few seconds for whatever reason, reset back to its flying ai
                        NPC.ai[2]++;
                        if (NPC.ai[2] >= 240 && NPC.ai[1] <= 1)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                        }
                        
                        //produce pollen particles, and then buff the enemy
                        if (NPC.ai[1] > 1)
                        {
                            NPC.spriteDirection = SaveDirection;

                            NPC.ai[1]++;

                            if (NPC.ai[1] % 6 == 2)
                            {
                                int DustEffect = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Gold * 0.5f, 0.25f);
                                Main.dust[DustEffect].velocity.X *= 0;
                                Main.dust[DustEffect].velocity.Y += 2;
                                Main.dust[DustEffect].alpha = 100;
                            }

                            if (NPC.ai[1] >= 180)
                            {
                                Main.npc[(int)NPC.ai[3]].AddBuff(ModContent.BuffType<BeeDamageBuff>(), 600);

                                NPC.ai[0] = 0;
                                NPC.ai[1] = 0;
                                NPC.ai[2] = 0;
                            }
                        }
                    }
                    
                    break;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DandelionSeed>(), 10));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Honeycomb>(), 20));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PollinatorBeeDamageGore" + numGores).Type);
                    }
                }

                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PollinatorBeeWingGore" + numGores).Type);
                    }
                }
            }
        }
	}

    public class PollinatorBeeHealing : PollinatorBeeDamage
	{
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PollinatorBeeHealing"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void AI()
        {
            NPC.rotation = NPC.velocity.X * 0.07f;
            
            switch ((int)NPC.ai[0])
            {
                //fly around back and fourth aimlessly
                case 0:
                {
                    NPC.ai[1]++;

                    //change y-velocity randomly at certain intervals
                    if (NPC.ai[1] >= 15)
                    {
                        NPC.velocity.Y += Main.rand.NextFloat(-0.4f, 0.4f);

                        NPC.ai[2] = 0;
                    }

                    //constantly increase x-velocity
                    NPC.velocity.X += NPC.spriteDirection == 1 ? 0.1f : -0.1f;

                    //cap velocity so it doesnt start zooming around
                    NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -3, 3);
                    NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -2, 2);

                    //bounce off of walls and turn around
                    if (NPC.ai[2] == 0 && NPC.collideX)
                    {
                        NPC.velocity = -NPC.oldVelocity;

                        NPC.spriteDirection = -NPC.spriteDirection;

                        NPC.ai[2] = 1;
                    }

                    //go to pollinating attack after 7 seconds
                    if (NPC.ai[1] >= 420)
                    {
                        NPC.ai[0]++;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                    }

                    break;
                }

                //pollinate an enemy
                case 1:
                {
                    //find an npc to buff
                    if (NPC.ai[1] == 0)
                    {
                        for (int i = 0; i <= Main.maxNPCs; i++)
                        {
                            NPC TargetNPC = Main.npc[i];

                            //if a valid npc is found, save its whoAmI and increase ai to start pollinating it
                            if (BuffableNPCs.Contains(TargetNPC.type) && TargetNPC.active && !TargetNPC.HasBuff(ModContent.BuffType<BeeDamageBuff>()) && !TargetNPC.HasBuff(ModContent.BuffType<BeeHealingBuff>()) && 
                            !TargetNPC.HasBuff(ModContent.BuffType<BeePollinationProcessBuff>()) && Vector2.Distance(NPC.Center, TargetNPC.Center) <= 500f)
                            {
                                NPC.ai[3] = TargetNPC.whoAmI;
                                NPC.ai[1]++;

                                break;
                            }
                            else
                            {
                                if (i >= Main.maxNPCs)
                                {
                                    NPC.ai[0] = 0;
                                    NPC.ai[1] = 0;
                                    NPC.ai[2] = 0;
                                    NPC.ai[3] = 0;
                                }
                            }
                        }
                    }
                    //when an npc is found, go to it, produce pollen clouds, and then buff it
                    else
                    {
                        //give the npc the bee pollination process buff
                        //this buff does absolutely nothing, but is used to prevent multiple bees from trying to pollinate the same enemy at once
                        Main.npc[(int)NPC.ai[3]].AddBuff(ModContent.BuffType<BeePollinationProcessBuff>(), 2);

                        if (NPC.ai[1] <= 1)
                        {
                            NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;
                            SaveDirection = NPC.spriteDirection;
                        }
                        
                        //go to the npc center
                        Vector2 GoTo = Main.npc[(int)NPC.ai[3]].Center;
                        GoTo.Y -= 50;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 6, 12);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                        if (Vector2.Distance(NPC.Center, Main.npc[(int)NPC.ai[3]].Center) <= 120f && NPC.ai[1] <= 1)
                        {
                            NPC.ai[1]++;
                        }

                        //if the bee is unable to reach its target after a few seconds for whatever reason, reset back to its flying ai
                        NPC.ai[2]++;
                        if (NPC.ai[2] >= 240 && NPC.ai[1] <= 1)
                        {
                            NPC.ai[0] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                            NPC.ai[3] = 0;
                        }
                        
                        //produce pollen particles, and then buff the enemy
                        if (NPC.ai[1] > 1)
                        {
                            NPC.spriteDirection = SaveDirection;

                            NPC.ai[1]++;

                            if (NPC.ai[1] % 12 == 2)
                            {
                                int DustEffect = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Gold * 0.5f, 0.25f);
                                Main.dust[DustEffect].velocity.X *= 0;
                                Main.dust[DustEffect].velocity.Y += 2;
                                Main.dust[DustEffect].alpha = 100;
                            }

                            if (NPC.ai[1] >= 180)
                            {
                                Main.npc[(int)NPC.ai[3]].AddBuff(ModContent.BuffType<BeeHealingBuff>(), 600);

                                NPC.ai[0] = 0;
                                NPC.ai[1] = 0;
                                NPC.ai[2] = 0;
                            }
                        }
                    }
                    
                    break;
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
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PollinatorBeeHealingGore" + numGores).Type);
                    }
                }

                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PollinatorBeeWingGore" + numGores).Type);
                    }
                }
            }
        }
    }
}