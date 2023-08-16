using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
    public class MarigoldSpit : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 150;
            NPC.damage = 35;
            NPC.defense = 15;
            NPC.width = 42;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 2, 50);
            NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.MarigoldSpit"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {   
            Player player = Main.player[NPC.target];

            if (NPC.Distance(player.Center) > 200f)
            {
                //jumping up frame
                if (NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
                //falling down frame
                else if (NPC.velocity.Y > 0)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                //idle frame
                else
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }

            if (NPC.Distance(player.Center) <= 200f)
            {
                if (NPC.ai[1] < 45)
                {
                    NPC.frame.Y = 3 * frameHeight;
                }
                if (NPC.ai[1] >= 45)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.direction;

            //jumping ai
            if (NPC.Distance(player.Center) > 200f)
            {
                NPC.ai[0]++;
                NPC.ai[1] = 0;

                if (NPC.ai[0] >= 75)
                {
                    //set where the it should be jumping towards
                    Vector2 JumpTo = new(player.Center.X, NPC.Center.Y - 100);

                    if (NPC.Distance(player.Center) >= 300)
                    {
                        JumpTo = new(player.Center.X, NPC.Center.Y - 75);
                    }

                    //set velocity and speed
                    Vector2 velocity = JumpTo - NPC.Center;
                    velocity.Normalize();

                    float speed = MathHelper.Clamp(velocity.Length() / 36, 8, 12);

                    //actual jumping
                    if (NPC.velocity.X == 0)
                    {
                        if (NPC.velocity.Y == 0)
                        {
                            velocity.Y -= 0.25f;
                        }

                        velocity.X *= 1.2f;
                        NPC.velocity = velocity * speed;
                    }

                    //loop ai
                    if (NPC.ai[0] >= 100)
                    {
                        NPC.ai[0] = Main.rand.Next(0, 45);
                    }
                }
            }

            //spit spores when close
            if (NPC.Distance(player.Center) <= 200f)
            {
                NPC.ai[1]++;
                NPC.ai[0] = 0;

                //spit a spore
                if (NPC.ai[1] == 45)
                {
                    SoundEngine.PlaySound(SoundID.Item17, NPC.Center);

                    Vector2 ShootSpeed = player.Center - NPC.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed.X *= 4f;
                    ShootSpeed.Y *= 4f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, 
                        ShootSpeed.Y, ProjectileID.DandelionSeed, NPC.damage / 3, 1, NPC.target, 0, 0);
                    }
                }

                //loop ai
                if (NPC.ai[1] >= 70)
                {
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
                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/MarigoldSpitGore" + numGores).Type);
                    }
                }

                //separate flower petals
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/MarigoldSpitGore3").Type);
                    }
                }
            }
        }
    }
}