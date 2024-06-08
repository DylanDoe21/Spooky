using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.NoseCult
{
	public class SnotSlime : ModNPC
	{
        bool HasJumped = false;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 100;
            NPC.damage = 25;
			NPC.defense = 0;
			NPC.width = 38;
			NPC.height = 30;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 66;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.NoseTempleBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SnotSlime"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.Y == 0)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 7)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 2)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                NPC.frame.Y = 2 * frameHeight;
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            JumpToTarget(player, 250, 50, 0);
        }

        public void JumpToTarget(Player target, int JumpHeight, int TimeBeforeNextJump, int DelayBeforeNextJump)
        {
            NPC.ai[0]++;

            //set where the it should be jumping towards
            Vector2 JumpTo = new(target.Center.X, NPC.Center.Y - JumpHeight);

            //set velocity and speed
            Vector2 velocity = JumpTo - NPC.Center;
            velocity.Normalize();

            int JumpSpeed = Main.rand.Next(13, 18);

            float speed = MathHelper.Clamp(velocity.Length() / 36, 10, JumpSpeed);

            NPC.velocity.X *= NPC.velocity.Y <= 0 ? 0.98f : 0.95f;

            //actual jumping
            if (NPC.ai[0] >= TimeBeforeNextJump)
            {
                NPC.ai[1]++;

                if (NPC.velocity == Vector2.Zero && !HasJumped)
                {
                    if (NPC.ai[1] == 10)
                    {
                        if (target.Distance(NPC.Center) <= 450f)
                        {
                            SoundEngine.PlaySound(SoundID.GlommerBounce, NPC.Center);
                        }
                        
                        velocity.Y -= 0.25f;
                        
                        HasJumped = true;
                    }
                }
                
                if (NPC.ai[1] < 15 && HasJumped)
                {
                    NPC.velocity = velocity * speed;
                }
            }

            //loop ai
            if (NPC.ai[0] >= TimeBeforeNextJump + 100)
            {
                HasJumped = false;

                NPC.ai[0] = DelayBeforeNextJump;
                NPC.ai[1] = 0;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int newDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.KryptonMoss, 0f, -2f, 0, default, 1.5f);
                    Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                    Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                    Main.dust[newDust].noGravity = true;
                    
                    if (Main.dust[newDust].position != NPC.Center)
                    {
                        Main.dust[newDust].velocity = NPC.DirectionTo(Main.dust[newDust].position) * 2f;
                    }
                }
            }
        }
	}
}