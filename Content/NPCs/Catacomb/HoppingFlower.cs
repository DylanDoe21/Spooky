using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb
{
    public class HoppingFlower : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Marigold");
            Main.npcFrameCount[NPC.type] = 5;
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
            NPC.lifeMax = Main.hardMode ? 165 : 55;
            NPC.damage = Main.hardMode ? 60 : 30;
            NPC.defense = Main.hardMode ? 25 : 12;
            NPC.width = 42;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("These blind and malicious marigold creatures hop around the catacombs, spitting deadly spores as a parting gift to any unwelcomed guests."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (player.InModBiome(ModContent.GetInstance<Biomes.CatacombBiome>()))
            {
                return 20f;
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {   
            if (NPC.ai[0] == 0)
            {
                //falling up
                if (NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
                //falling down
                else if (NPC.velocity.Y > 0)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                else
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }

            if (NPC.ai[0] == 1)
            {
                if (NPC.localAI[1] < 45)
                {
                    NPC.frame.Y = 3 * frameHeight;
                }
                if (NPC.localAI[1] >= 45)
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

            if (NPC.Distance(player.Center) <= 200f) 
            {
                NPC.ai[0] = 1;
            }
            else
            {
                NPC.ai[0] = 0;
            }

            //jumping ai
            if (NPC.ai[0] == 0)
            {
                NPC.localAI[0]++;
                NPC.localAI[1] = 0;

                if (NPC.localAI[0] >= 75)
                {
                    Vector2 JumpTo = new(player.Center.X, NPC.Center.Y - 60);
                    Vector2 velocity = JumpTo - NPC.Center;
                    velocity.Normalize();

                    float speed = MathHelper.Clamp(velocity.Length() / 36, 8, 18);
                 
                    if (NPC.velocity.X == 0)
                    {
                        if (NPC.velocity.Y == 0)
                        {
                            velocity.Y -= 0.18f;
                        }

                        velocity.X *= 1.5f;
                        NPC.velocity = velocity * speed;
                    }

                    if (NPC.localAI[0] >= 100)
                    {
                        NPC.localAI[0] = 0;
                    }
                }
            }

            //spit spores
            if (NPC.ai[0] == 1)
            {
                NPC.localAI[1]++;
                NPC.localAI[0] = 0;

                if (NPC.localAI[1] == 45)
                {
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

                if (NPC.localAI[1] >= 70)
                {
                    NPC.localAI[1] = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
        }

        public override void HitEffect(int hitDirection, double damage) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/HoppingFlowerGore" + numGores).Type);
                }
            }
        }
    }
}