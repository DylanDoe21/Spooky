using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.SpookyBiome.Projectiles;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Items.SpookyBiome.Armor;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class SpookySkeletonWarlock : ModNPC  
    {
        int AIState = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spooky Warlock");
            Main.npcFrameCount[NPC.type] = 8;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 120;
            NPC.damage = 30;
            NPC.defense = 0;
            NPC.width = 25;
			NPC.height = 58;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyBiomeUg>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("The skeletal warlocks of the spooky biome possess great magic, allowing them to conjure spells at foes.")
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                //spawn underground
                if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyBiomeUg>()))
                {
                    return 10f;
                }
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            //use regular walking anim when in walking state
            if (AIState == 0)
            {
                //running animation
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //jumping frame when falling/jumping
                if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
            }
            //use casting animation during casting ai
            if (AIState == 1)
            {
                //casting animation
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

            switch ((int)AIState)
            {
                //walk towards the player (fighter ai lol)
                case 0:
                {
                    NPC.localAI[0]++;

                    NPC.aiStyle = 3;
			        AIType = NPCID.Crab;

                    if (NPC.localAI[0] >= 420)
                    {
                        NPC.localAI[0] = 0;
                        AIState++;
                    }

                    break;
                }
                //casting ai state
                case 1:
                {
                    NPC.localAI[0]++;

                    NPC.aiStyle = 0;

                    if (NPC.localAI[0] == 60 || NPC.localAI[0] == 80 || NPC.localAI[0] == 100)
                    {
                        SoundEngine.PlaySound(SoundID.Item8, NPC.position);

                        Vector2 ShootSpeed = Main.player[NPC.target].Center - NPC.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed.X *= 4.5f;
                        ShootSpeed.Y *= 4.5f;
                        
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X - 25, NPC.Center.Y, ShootSpeed.X, 
                            ShootSpeed.Y, ModContent.ProjectileType<WarlockSkull>(), NPC.damage / 4, 1, NPC.target, 0, 0);
                        }
                    }

                    if (NPC.localAI[0] >= 120)
                    {
                        NPC.localAI[0] = 0;
                        AIState = 0;
                    }

                    break;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WarlockHood>(), 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkullWispStaff>(), 10));
        }

        public override bool CheckDead() 
		{
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/SpookySkeletonGore1").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/SpookySkeletonGore2").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/SpookySkeletonGore3").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/SpookySkeletonGore4").Type);

            return true;
		}
    }
}