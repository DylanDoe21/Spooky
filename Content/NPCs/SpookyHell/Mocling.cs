using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.NPCs.NoseCult.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class Mocling : ModNPC
    {
        Vector2 GoToPosition;
        Vector2 SavePosition;

        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle SneezeSound = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound) { Pitch = 0.7f, Volume = 0.5f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 70;
            NPC.damage = 25;
            NPC.defense = 0;
            NPC.width = 38;
            NPC.height = 36;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit22 with { Pitch = 0.45f };
			NPC.DeathSound = SoundID.NPCDeath16;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Mocling"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/MoclingGlow");

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 2)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            if (NPC.ai[0] > 0 && NPC.localAI[0] > 215 && NPC.localAI[0] < 245)
            {
                NPC.frame.Y = 8 * frameHeight;
            }
        }

		public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
		{
			NPC Parent = Main.npc[(int)NPC.ai[1]];
			Parent.active = false;
		}

		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			NPC Parent = Main.npc[(int)NPC.ai[1]];
			Parent.active = false;
		}

		public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC Parent = Main.npc[(int)NPC.ai[1]];

			switch ((int)NPC.ai[0])
            {
                //fly towards the player
                case 0:
                {
                    NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;

                    NPC.localAI[0]++;

                    //randomly go to a position around the invisible parent npc
                    if (NPC.localAI[0] == 1 || NPC.localAI[0] % 20 == 0)
                    {
                        GoToPosition = new Vector2(Parent.Center.X + Main.rand.Next(-75, 75), Parent.Center.Y - Main.rand.Next(35, 75));
                    }

                    Vector2 GoTo = GoToPosition;

                    float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 4, 7);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                    //if the "swarm" this mocling is attached to is approached and dissappears, set the mocling to its attacking behavior
                    if (!Parent.active || Parent.type != ModContent.NPCType<MoclingSwarm>())
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //shoot snot at the player
                case 1:
                {
                    NPC.spriteDirection = NPC.direction;

                    //EoC rotation
                    Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float RotateX = player.Center.X - vector.X;
                    float RotateY = player.Center.Y - vector.Y;
                    NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 1 || NPC.localAI[0] % 20 == 0)
                    {
                        GoToPosition = new Vector2(player.Center.X + Main.rand.Next(-200, 200), player.Center.Y - Main.rand.Next(80, 135));
                    }

                    if (NPC.localAI[0] <= 180)
                    {
                        Vector2 GoTo = GoToPosition;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 4, 7);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    //save npc center
                    if (NPC.localAI[0] == 180)
                    {
                        NPC.velocity *= 0;

                        SavePosition = NPC.Center;
                    }

                    //shake before shooting
                    if (NPC.localAI[0] > 180 && NPC.localAI[0] < 210)
                    {
                        NPC.Center = new Vector2(SavePosition.X, SavePosition.Y);
                        NPC.Center += Main.rand.NextVector2Square(-7, 7);
                    }

                    if (NPC.localAI[0] >= 215)
                    {
                        NPC.velocity *= 0.8f;
                    }

                    if (NPC.localAI[0] == 215)
                    {
                        SoundEngine.PlaySound(SneezeSound, NPC.Center);

                        Vector2 Recoil = player.Center - NPC.Center;
                        Recoil.Normalize(); 
                        Recoil *= -10;
                        NPC.velocity = Recoil;

                        Vector2 ShootSpeed = player.Center - NPC.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= 5.5f;

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, ShootSpeed, ModContent.ProjectileType<NoseCultistGruntSnot>(), NPC.damage / 4, 0, NPC.target);
                    }

                    if (NPC.localAI[0] >= 250)
                    {
                        NPC.localAI[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }
            }

            for (int num = 0; num < Main.maxNPCs; num++)
			{
				NPC other = Main.npc[num];
				if (num != NPC.whoAmI && other.type == NPC.type && other.active && Math.Abs(NPC.position.X - other.position.X) + Math.Abs(NPC.position.Y - other.position.Y) < NPC.width)
				{
					const float pushAway = 0.2f;
					if (NPC.position.X < other.position.X)
					{
						NPC.velocity.X -= pushAway;
					}
					else
					{
						NPC.velocity.X += pushAway;
					}
					if (NPC.position.Y < other.position.Y)
					{
						NPC.velocity.Y -= pushAway;
					}
					else
					{
						NPC.velocity.Y += pushAway;
					}
				}
			}
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/MoclingGore").Type);
                }
            }
        }
    }
}