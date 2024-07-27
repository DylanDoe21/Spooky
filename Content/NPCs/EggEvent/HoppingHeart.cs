using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.EggEvent
{
	public class HoppingHeart : ModNPC
	{
        bool HasJumped = false;

        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 13;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 450;
            NPC.damage = 45;
            NPC.defense = 10;
            NPC.width = 62;
            NPC.height = 68;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.aiStyle = 66;
			SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.HoppingHeart"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/HoppingHeartGlow");

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 11)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            NPC.rotation = NPC.velocity.Y * (NPC.direction == -1 ? 0.04f : -0.04f);

            JumpToTarget(player, 350, 30);
        }

        public void JumpToTarget(Player target, int JumpHeight, int TimeBeforeNextJump)
        {
            NPC.ai[0]++;

            //set where the it should be jumping towards
            Vector2 JumpTo = new(target.Center.X, NPC.Center.Y - JumpHeight);

            //set velocity and speed
            Vector2 velocity = JumpTo - NPC.Center;
            velocity.Normalize();

            int JumpSpeed = Main.rand.Next(25, 35);

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

                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 12; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        //Gore.NewGore(NPC.GetSource_Death(), NPC.Center, (NPC.velocity * 0.5f) + new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, -1)), ModContent.Find<ModGore>("Spooky/MonsterEyeChunk").Type);
                    }
                }
            }
        }
	}
}