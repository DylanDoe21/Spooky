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

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Minibiomes.Christmas
{
    public class ToyRobot : ModNPC  
    {
        bool Exploding = false;
        bool ShouldExplode = false;
        bool Initialized = false;

        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(Exploding);
            writer.Write(ShouldExplode);
            writer.Write(Initialized);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            Exploding = reader.ReadBoolean();
            ShouldExplode = reader.ReadBoolean();
            Initialized = reader.ReadBoolean();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 75;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.width = 36;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCHit4;
            NPC.aiStyle = 3;
            AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().Type };
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ToyRobot"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Exploding)
            {
                GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Christmas/ToyRobotExplosionGlow");

                Vector2 frameOrigin = NPC.frame.Size() / 2f;
                Vector2 drawPos = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY + 4);

                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 50f)) / 3f + 0.5f;

                float time = Main.GlobalTimeWrappedHourly;

                time %= 4f;
                time /= 2f;

                time = time * 0.5f + 0.5f;

                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.OrangeRed) * 0.5f;

                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    Main.EntitySpriteDraw(GlowTexture.Value, drawPos + new Vector2(0f, 1f).RotatedBy(radians) * time, NPC.frame, color * fade, NPC.rotation, frameOrigin, NPC.scale, effects, 0);
                }
            }
        }

		public override void FindFrame(int frameHeight)
        {
            if (!Exploding)
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
        }

        public override bool CheckDead()
        {
            if (ShouldExplode)
            {
                Exploding = true;

                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                NPC.life = 1;

                NPC.netUpdate = true;

                return false;
            }

            return true;
        }
        
        public override void AI()
		{
            NPC.spriteDirection = NPC.direction;

            if (!Initialized)
            {
                if (Main.rand.NextBool())
                {
                    ShouldExplode = true;
                }

                Initialized = true;
            }

            if (Exploding)
            {
                NPC.aiStyle = -1;
                NPC.velocity.X = 0;

                NPC.ai[0]++;

                if (NPC.ai[0] > 80)
                {
                    SoundEngine.PlaySound(SoundID.Item14 with { Pitch = -0.5f }, NPC.Center);

                    //spawn gores
					for (int numGores = 1; numGores <= 6; numGores++)
					{
						if (Main.netMode != NetmodeID.Server)
						{
							Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-8, 8), Main.rand.Next(-8, -3)), ModContent.Find<ModGore>("Spooky/ToyRobotGore" + numGores).Type);
						}
					}

                    //flame dusts
					for (int numDust = 0; numDust < 45; numDust++)
					{
						int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork, 0f, -2f, 0, default, 1.5f);
						Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-7f, 8f);
						Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-4f, 5f);
						Main.dust[dustGore].noGravity = true;
					}

					//explosion smoke
					for (int numExplosion = 0; numExplosion < 5; numExplosion++)
					{
						int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, 0.45f);
						Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-1f, 2f);
						Main.dust[DustGore].velocity.Y *= Main.rand.NextFloat(-1f, 2f);
						Main.dust[DustGore].noGravity = true;
					}

                    foreach (Player player in Main.ActivePlayers)
					{
						if (!player.dead && player.Distance(NPC.Center) <= 80f)
						{
							player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetText("Mods.Spooky.DeathReasons.ToyRobotExplosion").ToNetworkText(player.name)), NPC.damage + Main.rand.Next(0, 30), 0);
						}
					}

                    NPC.active = false;
                }
            }
            else
            {
                NPC.aiStyle = 3;
                AIType = NPCID.GoblinScout;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (!ShouldExplode && NPC.life <= 0)
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ToyRobotGore" + numGores).Type);
                    }
                }
            }
        }
    }
}