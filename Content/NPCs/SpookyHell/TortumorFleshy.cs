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
using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class TortumorFleshy : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle ScreechSound = new("Spooky/Content/Sounds/TumorScreech1", SoundType.Sound) { Volume = 0.6f, PitchVariance = 0.6f };
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/TortumorDeath", SoundType.Sound) { Volume = 0.6f };

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 80;
            NPC.damage = 32;
            NPC.defense = 10;
            NPC.width = 58;
            NPC.height = 62;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.75f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
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
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TortumorFleshy"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/TortumorFleshyGlow");

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            //floating animation
            NPC.frameCounter++;
            if (NPC.ai[0] <= 240)
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
            if (NPC.ai[0] >= 240)
            {
                if (NPC.ai[0] == 240)
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

            NPC.rotation += (NPC.velocity.X / 30);

            if (NPC.Distance(player.Center) <= 500f || NPC.ai[0] >= 230)
            {
                NPC.ai[0]++;
            }
            
            if (NPC.ai[0] <= 240)
            {
                int MaxSpeed = 2;

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

            if (NPC.ai[0] >= 240)
            {
                if (NPC.ai[0] == 240)
                {
                    SoundEngine.PlaySound(ScreechSound, NPC.Center);
                }

                NPC.velocity *= 0.95f;

                if (NPC.ai[0] == 315)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath12, NPC.Center);

                    for (int numProjectiles = 0; numProjectiles <= 1; numProjectiles++)
                    {
                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, Vector2.Zero, ModContent.ProjectileType<TumorOrb>(), NPC.damage, 4.5f, ai1: NPC.whoAmI, ai2: Main.rand.Next(0, 2));
                    }
                }

                if (NPC.ai[0] >= 330)
                {
                    NPC.ai[0] = 0;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<EyeChocolate>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                //spawn gores
                for (int numGores = 1; numGores <= 8; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-12, 13), Main.rand.Next(-12, 13)), ModContent.Find<ModGore>("Spooky/TortumorFleshyGore" + Main.rand.Next(1, 4)).Type, Main.rand.NextFloat(1f, 1.35f));
                    }
                }

                //spawn blood explosion clouds
                for (int numExplosion = 0; numExplosion < 8; numExplosion++)
                {
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.65f, Main.rand.NextFloat(0.5f, 0.8f));
                    Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-2f, 2f);
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