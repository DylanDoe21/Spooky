using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.NPCs.Minibiomes.Christmas.Projectiles;

namespace Spooky.Content.NPCs.Minibiomes.Christmas
{
    public class ToyRobotTank : ModNPC  
    {
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 75;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.width = 36;
			NPC.height = 62;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.25f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.Item14;
            NPC.aiStyle = 3;
            AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().Type };
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ToyRobotTank"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.localAI[0] >= 300 && NPC.localAI[0] < 360)
            {
                GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Christmas/ToyRobotTankBeamGlow");

                Vector2 frameOrigin = NPC.frame.Size() / 2f;
                Vector2 drawPos = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY + 4);

                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 100f)) / 3f + 0.5f;

                float time = Main.GlobalTimeWrappedHourly;

                time %= 4f;
                time /= 2f;

                time = time * 0.5f + 0.5f;

                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Red) * 0.5f;

                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    Main.EntitySpriteDraw(GlowTexture.Value, drawPos + new Vector2(0f, 1f).RotatedBy(radians) * time, NPC.frame, color * fade, NPC.rotation, frameOrigin, NPC.scale, effects, 0);
                }
            }
        }

		public override void FindFrame(int frameHeight)
        {
            if (NPC.localAI[0] < 300)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 5)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }
        
        public override void AI()
		{
            NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] < 300)
            {
                NPC.aiStyle = 3;
                AIType = NPCID.GoblinScout;
            }
            else
            {
                NPC.aiStyle = 0;
            }

            Vector2 NPCShootFromPos = NPC.Center + new Vector2(NPC.spriteDirection == -1 ? -29 : 29, -2);

            if (NPC.localAI[0] == 300)
            {   
                NPC.velocity.X = 0;
            }

            if (NPC.localAI[0] >= 300 && NPC.localAI[0] < 360)
            {
                int Amount = Main.rand.Next(6, 13);
                for (int numDusts = 0; numDusts < Amount; numDusts++)
                {
                    float distance = Main.rand.NextFloat(0.5f, 1f);

                    Vector2 dustPos = (Vector2.One * new Vector2(NPC.width / 3f, NPC.height / 3f) * distance).RotatedBy((double)((numDusts - (Amount / 2 - 1)) * 6.28318548f / Amount), default) + NPCShootFromPos;
                    Vector2 velocity = (dustPos - NPCShootFromPos) * 0.5f;

                    if (Main.rand.NextBool())
                    {
                        int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, 183, velocity.X * 2f, velocity.Y * 2f, 100, default, 1f);
                        Main.dust[dustEffect].noGravity = true;
                        Main.dust[dustEffect].noLight = false;
                        Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * -1f;
                        Main.dust[dustEffect].fadeIn = 1.2f;
                    }
                }
            }

            if (NPC.localAI[0] == 360)
            {
                SoundEngine.PlaySound(SoundID.Item92 with { Volume = 0.75f, Pitch = -0.25f }, NPC.Center);

                NPCGlobalHelper.ShootHostileProjectile(NPC, NPCShootFromPos, new Vector2(NPC.spriteDirection == -1 ? -18 : 18, 0), ModContent.ProjectileType<RobotLaser>(), NPC.damage, 4.5f);
                NPC.velocity = new Vector2(NPC.spriteDirection == 1 ? -5 : 5, 0);
            }

            if (NPC.localAI[0] >= 420)
            {
                NPC.localAI[0] = 0;
                NPC.netUpdate = true;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0)
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ToyRobotTankGore" + numGores).Type);
                    }
                }
            }
        }
    }
}