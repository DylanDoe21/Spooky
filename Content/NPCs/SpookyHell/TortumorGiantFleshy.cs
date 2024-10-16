using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Items.Food;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class TortumorGiantFleshy : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle ScreechSound = new("Spooky/Content/Sounds/TumorScreech2", SoundType.Sound) { Volume = 0.6f, PitchVariance = 0.6f };
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/TortumorDeath", SoundType.Sound) { Volume = 0.75f };

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 330;
            NPC.damage = 35;
            NPC.defense = 10;
            NPC.width = 90;
            NPC.height = 94;
            NPC.npcSlots = 5f;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 12, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TortumorGiantFleshy"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/TortumorGiantFleshyGlow");

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            //floating animation
            NPC.frameCounter++;
            if (NPC.ai[0] <= 300)
            {
                if (NPC.frameCounter > 8)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }

            //screaming animation
            if (NPC.ai[0] >= 300)
            {
                if (NPC.ai[0] == 300)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }

                if (NPC.frameCounter > 8)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 12)
                {
                    NPC.frame.Y = 11 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];

            NPC.rotation += (NPC.velocity.X / 45);
			
            if (NPC.Distance(player.Center) <= 500f || NPC.ai[0] >= 230)
            {
                NPC.ai[0]++;
            }

            if (NPC.ai[0] <= 300)
            {
                int MaxSpeed = 3;

                //flies to players X position
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeed - 1) 
                {
                    MoveSpeedX--;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeed + 1)
                {
                    MoveSpeedX++;
                }

                NPC.velocity.X += MoveSpeedX * 0.01f;
                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -MaxSpeed - 1, MaxSpeed + 1);
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -MaxSpeed)
                {
                    MoveSpeedY--;
                }
                else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= MaxSpeed)
                {
                    MoveSpeedY++;
                }

                NPC.velocity.Y += MoveSpeedY * 0.01f;
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MaxSpeed, MaxSpeed);
            }

            if (NPC.ai[0] >= 300)
            {
                if (NPC.ai[0] == 300)
                {
                    SoundEngine.PlaySound(ScreechSound, NPC.Center);
                }

                NPC.velocity *= 0.95f;

                //only summon tortumors if no other tortumors exist
                if (!NPC.AnyNPCs(ModContent.NPCType<TortumorFleshy>()))
                {
                    if (NPC.ai[0] == 400)
                    {
                        int TortumorSummon = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<TortumorFleshy>());
                        Main.npc[TortumorSummon].velocity = new Vector2(Main.rand.Next(-8, 9), Main.rand.Next(-8, 9));

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: TortumorSummon);
                        }

                        //use npc.ai[1] to prevent projectile shooting attack from running after summoning
                        NPC.ai[1] = 1;
                    }
                }
                else
                {
                    if (NPC.ai[0] >= 370 && NPC.ai[1] == 0)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            SoundEngine.PlaySound(SoundID.Item87, NPC.Center);

                            NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, Vector2.Zero, ModContent.ProjectileType<TumorOrb>(), NPC.damage, 4.5f, ai0: 179, ai1: NPC.whoAmI, ai2: Main.rand.Next(0, 3));
                        }
                    }
                }

                if (NPC.ai[0] >= 420)
                {
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TortumorStaff>(), 8));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EyeChocolate>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                //spawn gores
                for (int numGores = 1; numGores <= 15; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-18, 19), Main.rand.Next(-18, 19)), ModContent.Find<ModGore>("Spooky/TortumorFleshyGore" + Main.rand.Next(1, 4)).Type, Main.rand.NextFloat(1.2f, 1.5f));
                    }
                }

                //spawn blood explosion clouds
                for (int numExplosion = 0; numExplosion < 15; numExplosion++)
                {
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.65f, Main.rand.NextFloat(0.8f, 1.2f));
                    Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                    Main.dust[DustGore].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[DustGore].scale = 0.5f;
                        Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }
    }
}