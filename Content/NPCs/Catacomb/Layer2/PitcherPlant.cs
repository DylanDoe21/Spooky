using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.NPCs.Catacomb.Layer2.Projectiles;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
	public class PitcherPlant1 : ModNPC
	{
        bool HasJumped = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(HasJumped);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			HasJumped = reader.ReadBoolean();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 170;
            NPC.damage = 40;
			NPC.defense = 15;
			NPC.width = 40;
			NPC.height = 64;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 25);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Grass;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 66;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PitcherPlant1"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            Player player = Main.player[NPC.target];

            //jumping animations
            if (NPC.Distance(player.Center) > 200f || NPC.velocity != Vector2.Zero)
            {
                //still frame
                if (NPC.velocity.Y == 0 && NPC.frame.Y <= frameHeight * 0)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //jumping up animation
                if (NPC.velocity.Y < 0)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 6)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frame.Y >= frameHeight * 3)
                    {
                        NPC.frame.Y = 2 * frameHeight;
                    }
                }

                //falling down animation
                if (NPC.velocity.Y > 0)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 4)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = 4 * frameHeight;
                    }
                }

                //landing frame
                if (NPC.velocity.Y == 0 && NPC.frame.Y > frameHeight * 0)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 2)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frame.Y >= frameHeight * 6)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                }
            }

            //spitting animation frames
            if (NPC.Distance(player.Center) <= 200f && NPC.velocity == Vector2.Zero)
            {
                if (NPC.ai[2] < 45)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
                if (NPC.ai[2] >= 45)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
            }
        }

		//hitting the pitcher plant with items and projectiles should reset its shooting timer so players dont get unfairly hit at close range
		public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
		{
			NPC.ai[2] = 0;
		}
		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			NPC.ai[2] = 0;
		}

		public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            AttackingAI(player, 200, ModContent.ProjectileType<PitcherSpitPoison>(), false);
        }

        public void AttackingAI(Player target, int JumpHeight, int SpitType, bool HydraPlant)
        {
            if (NPC.Distance(target.Center) > 200f)
            {
                //set where the it should be jumping towards
                Vector2 JumpTo = new(target.Center.X, NPC.Center.Y - JumpHeight);

                //set velocity and speed
                Vector2 velocity = JumpTo - NPC.Center;
                velocity.Normalize();

                int JumpSpeed = Main.rand.Next(13, 18);

                float speed = MathHelper.Clamp(velocity.Length() / 36, 10, JumpSpeed);

                NPC.velocity.X *= NPC.velocity.Y <= 0 ? 0.98f : 0.95f;

                //actual jumping attack
                NPC.ai[0]++;
                if (NPC.velocity == Vector2.Zero && !HasJumped)
                {
                    if (NPC.ai[0] == 10)
                    {
                        velocity.Y -= 0.25f;

                        NPC.velocity = velocity * speed;
                        
                        HasJumped = true;
                    }
                }

                //loop ai with separate timer
                NPC.ai[1]++;
                if (NPC.ai[1] >= 100)
                {
                    HasJumped = false;

                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                }
            }
            
            if (NPC.Distance(target.Center) <= 200f && NPC.velocity == Vector2.Zero)
            {
                NPC.ai[0] = -60;
                NPC.ai[1] = -60;

                NPC.ai[2]++;

                //spit poison
                if (NPC.ai[2] == 45)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath9, NPC.Center);

                    HasJumped = false;

                    Vector2 ShootPosition1 = new Vector2(NPC.Center.X + (NPC.direction == -1 ? -10 : 10), NPC.Center.Y - 10);
                    Vector2 ShootPosition2 = new Vector2(NPC.Center.X + (NPC.direction == -1 ? 20 : -20), NPC.Center.Y - 5);

                    if (HydraPlant)
                    {
                        ShootPosition1 = new Vector2(NPC.Center.X + (NPC.direction == -1 ? -18 : 18), NPC.Center.Y - 10);
                    }

                    Vector2 ShootSpeed = target.Center - NPC.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed.X *= 8f;
                    ShootSpeed.Y *= 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), ShootPosition1, ShootSpeed, SpitType, NPC.damage / 4, 0, NPC.target);
                    }

                    //shoot additional projectile for the hydra varaints
                    if (HydraPlant)
                    {
                        //spit the opposite projectile type as the front head
                        int HydraSpitType = SpitType == ModContent.ProjectileType<PitcherSpitPoison>() ? ModContent.ProjectileType<PitcherSpitVenom>() : ModContent.ProjectileType<PitcherSpitPoison>();

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), ShootPosition2, -ShootSpeed, HydraSpitType, NPC.damage / 4, 0, NPC.target);
                        }
                    }
                }

                //loop ai
                if (NPC.ai[2] >= 65)
                {
                    NPC.ai[2] = 0;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlant1Gore").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlantStemGore1").Type);
				}

                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlantRedGore" + numGores).Type);
                    }
                }
            }
        }
	}

    public class PitcherPlant2 : PitcherPlant1
	{
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PitcherPlant2"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            AttackingAI(player, 200, ModContent.ProjectileType<PitcherSpitVenom>(), false);
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlant2Gore").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlantStemGore1").Type);
				}

                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlantYellowGore" + numGores).Type);
                    }
                }
            }
        }
    }

    public class PitcherPlant3 : PitcherPlant1
	{
        public override void SetDefaults()
		{
            NPC.lifeMax = 220;
            NPC.damage = 40;
			NPC.defense = 15;
			NPC.width = 56;
			NPC.height = 64;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.25f;
            NPC.value = Item.buyPrice(0, 0, 1, 25);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Grass;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 66;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PitcherPlant3"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            AttackingAI(player, 200, ModContent.ProjectileType<PitcherSpitVenom>(), true);
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlant3Gore").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlantStemGore2").Type);
				}

                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlantRedGore" + numGores).Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlantYellowGore" + numGores).Type);
                    }
                }
            }
        }
    }

    public class PitcherPlant4 : PitcherPlant1
	{
        public override void SetDefaults()
		{
            NPC.lifeMax = 220;
            NPC.damage = 40;
			NPC.defense = 15;
			NPC.width = 56;
			NPC.height = 64;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.25f;
            NPC.value = Item.buyPrice(0, 0, 1, 25);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Grass;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 66;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PitcherPlant4"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            AttackingAI(player, 200, ModContent.ProjectileType<PitcherSpitPoison>(), true);
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlant4Gore").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlantStemGore2").Type);
				}

                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlantRedGore" + numGores).Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PitcherPlantYellowGore" + numGores).Type);
                    }
                }
            }
        }
    }
}