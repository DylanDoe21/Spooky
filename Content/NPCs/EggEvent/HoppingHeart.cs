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
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.EggEvent;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.EggEvent.Projectiles;

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

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(HasJumped);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            HasJumped = reader.ReadBoolean();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 450;
            NPC.damage = 50;
            NPC.defense = 10;
            NPC.width = 62;
            NPC.height = 68;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.25f;
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

            if (NPC.localAI[0] <= 420)
            {
                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 10)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            
            if (NPC.localAI[0] > 420 && NPC.velocity.Y == 0)
            {
                if (NPC.frame.Y < frameHeight * 10)
                {
                    NPC.frame.Y = 10 * frameHeight;
                }

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 13)
                {
                    NPC.frame.Y = 11 * frameHeight;
                }
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            NPC.rotation = NPC.velocity.Y * (NPC.direction == -1 ? 0.04f : -0.04f);

            NPC.localAI[0]++;
            if (NPC.localAI[0] <= 420)
            {
                JumpToTarget(player, 350, 30);
            }
            else
            {
                if (NPC.velocity.Y == 0)
                {
                    if (NPC.localAI[0] % 20 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item17, NPC.Center);

                        int[] Types = new int[] { ModContent.ProjectileType<HeartGlob1>(), ModContent.ProjectileType<HeartGlob2>() };

                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + (NPC.direction == -1 ? -5 : 5), NPC.Center.Y - 18), 
                        new Vector2((NPC.direction == -1 ? Main.rand.Next(-10, -4) : Main.rand.Next(4, 10)), Main.rand.Next(-7, -3)), Main.rand.Next(Types), NPC.damage, 4.5f);
                    }

                    if (NPC.localAI[0] >= 570)
                    {
                        NPC.localAI[0] = 0;
                        
                        NPC.netUpdate = true;
                    }
                }
            }
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
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 5, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<VeinChain>(), 65));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
				{
                    //spawn splatter
                    for (int i = 0; i < 10; i++)
                    {
                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(Main.rand.Next(-4, 5), Main.rand.Next(-4, -1)), ModContent.ProjectileType<RedSplatter>(), 0, 0f);
                    }
                }

                for (int numGores = 1; numGores <= 7; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/HoppingHeartGore" + numGores).Type);
                    }
                }
            }
        }
	}
}