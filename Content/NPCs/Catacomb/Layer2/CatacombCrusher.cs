using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Food;
using Spooky.Content.NPCs.Catacomb.Layer2.Projectiles;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
    public class CatacombCrusher1 : ModNPC  
    {
        bool hasCollidedWithFloor = false;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/Catacomb/Layer2/CatacombCrusher1"
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 500;
            NPC.damage = 60;
            NPC.defense = 25;
            NPC.width = 88;
            NPC.height = 60;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CatacombCrusher1"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            SoundEngine.PlaySound(SoundID.DD2_OgreAttack with { Pitch = SoundID.DD2_OgreAttack.Pitch * 0.5f }, NPC.Center);
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            if (NPC.alpha > 0)
            {
                NPC.alpha -= 5;
            }

            NPC.ai[0]++;

            if (NPC.ai[0] < 60)
            {
                NPC.velocity *= 0;
            }

            if (NPC.ai[0] == 60)
            {
                NPC.velocity.Y = 35;
            }

            //slam down
            if (NPC.ai[0] > 60 && !hasCollidedWithFloor)
            {
                /*
                //set tile collide to true to prevent platforms and stuff getting in the way
                if (NPC.position.Y >= player.Center.Y - 100 || player.Distance(NPC.Center) > 200f)
				{
                    NPC.noTileCollide = false;
                }
                else
                {
                    NPC.noTileCollide = true;
                }
                */

                //smash the ground
                if (NPC.velocity.Y <= 0.1f)
                {
                    SoundEngine.PlaySound(SoundID.Item70, NPC.Center);

                    if (player.velocity.Y == 0 && player.Distance(NPC.Center) <= 350f)
                    {
                        SpookyPlayer.ScreenShakeAmount = 8;
                    }

                    NPC.velocity *= 0;

                    NPC.noTileCollide = false;
                    hasCollidedWithFloor = true;
                }
            }

            if (hasCollidedWithFloor)
            {
                if (NPC.ai[1] == 0)
                {
                    NPC.velocity.Y = -7;

                    NPC.ai[1] = 1;
                }

                if (NPC.ai[0] > 300)
                {
                    hasCollidedWithFloor = false;

                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CatacombCrusherDeath1>(), NPC.damage / 2, 0, NPC.target);
            }
        }
    }

    public class CatacombCrusher2 : CatacombCrusher1  
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/Catacomb/Layer2/CatacombCrusher2"
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CatacombCrusher2"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CatacombCrusherDeath2>(), NPC.damage / 2, 0, NPC.target);
            }
        }
    }

    public class CatacombCrusher3 : CatacombCrusher1  
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/Catacomb/Layer2/CatacombCrusher3"
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CatacombCrusher3"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CatacombCrusherDeath3>(), NPC.damage / 2, 0, NPC.target);
            }
        }
    }

    public class CatacombCrusher4 : CatacombCrusher1  
    {
        public override void SetDefaults()
		{
            NPC.lifeMax = 700;
            NPC.damage = 60;
            NPC.defense = 25;
            NPC.width = 88;
            NPC.height = 60;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 3, 0, 0);
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/Catacomb/Layer2/CatacombCrusher4"
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CatacombCrusher4"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<CatacombCrusherDeath4>(), NPC.damage / 2, 0, NPC.target);
            }
        }
    }
}