using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class SpiderWarRemoteNuke : ModProjectile
    {
        int PreviousTarget = 0;

        float FlashOpacity = 0f;

        bool isAttacking = false;

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> AuraTexture;
        private static Asset<Texture2D> FlashTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 60;
            Projectile.height = 46;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.minionSlots = 2;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            AuraTexture ??= ModContent.Request<Texture2D>(Texture + "Aura");
            FlashTexture ??= ModContent.Request<Texture2D>(Texture + "Flash");

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (Projectile.ai[2] >= 60)
			{
				Color AuraColor = new Color(125, 125, 125, 0).MultiplyRGBA(Color.Green);

				float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;
                float time2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                for (int i = 0; i < 360; i += 30)
                {
                    Vector2 circular = Vector2.One.RotatedBy(MathHelper.ToRadians(i));

                    Main.EntitySpriteDraw(AuraTexture.Value, vector + circular, rectangle, AuraColor * 0.1f, Projectile.rotation + i,
                    drawOrigin, Projectile.ai[1] / 37 + (Projectile.ai[1] < 600 ? time : time2), SpriteEffects.None, 0);
                }
			}
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            //flashing texture when exploding
			if (FlashOpacity > 0f)
            {
                Main.EntitySpriteDraw(FlashTexture.Value, vector, rectangle, Color.White * FlashOpacity, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }

			return false;
		}

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<SpiderWarRemoteNukeBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<SpiderWarRemoteNukeBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            if (FlashOpacity > 0f)
            {
                FlashOpacity -= 0.025f;
            }

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 1500f)
                {
                    AttackingAI(Target);

                    break;
                }
                else
                {
                    isAttacking = false;
                }

                NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 750f)
                {
                    AttackingAI(NPC);

                    break;
                }
                else
                {
                    isAttacking = false;
                }
            }

            if (!isAttacking)
            {
                IdleAI(player);
            }

            //prevent Projectiles clumping together
            for (int k = 0; k < Main.projectile.Length; k++)
            {
                Projectile other = Main.projectile[k];
                if (k != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    const float pushAway = 0.1f;
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.X += pushAway;
                    }
                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.Y += pushAway;
                    }
                }
            }
		}

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? -1 : 1;

            if (Projectile.Distance(target.Center) > 50f)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 25;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }

            Projectile.ai[2]++;
            if (Projectile.ai[2] >= 60)
            {
                if (Projectile.ai[1] < 600)
                {
                    Projectile.ai[1] += 25;
                }
                else
                {
                    if (Projectile.ai[2] % 15 == 0)
					{
						FlashOpacity = 1f;
					}
                }
            }

            if (Projectile.ai[2] >= 120)
            {
                Player player = Main.player[Projectile.owner];

                SoundEngine.PlaySound(SoundID.Item74 with { Volume = 0.75f, Pitch = -0.25f }, Projectile.Center);

				Screenshake.ShakeScreenWithIntensity(Projectile.Center, 5f, 300f);

                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.active && npc.CanBeChasedBy(this) && !npc.friendly && !npc.dontTakeDamage && !NPCID.Sets.CountsAsCritter[npc.type] && npc.Distance(Projectile.Center) <= Projectile.ai[1] * 0.65f + time)
                    {
                        player.ApplyDamageToNPC(npc, Projectile.damage * 10, 0, 0, false, null, true);
                    }
                }

                float maxAmount = 25;
                int currentAmount = 0;
                while (currentAmount <= maxAmount)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(2f, 25f), Main.rand.NextFloat(2f, 25f));
                    Vector2 Bounds = new Vector2(Main.rand.NextFloat(2f, 25f), Main.rand.NextFloat(2f, 25f));
                    float intensity = Main.rand.NextFloat(2f, 25f);

                    Vector2 vector12 = Vector2.UnitX * 0f;
                    vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                    vector12 = vector12.RotatedBy(velocity.ToRotation(), default);

                    int Fire = Dust.NewDust(Projectile.Center, 0, 0, DustID.GreenTorch, 0f, 0f, 100, default, 5f);
                    Main.dust[Fire].noGravity = true;
                    Main.dust[Fire].noLight = true;
                    Main.dust[Fire].position = Projectile.Center + vector12;
                    Main.dust[Fire].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;

                    if (currentAmount % 2 == 0)
                    {
                        int Smoke = Dust.NewDust(Projectile.Center, 0, 0, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.5f, 2f));
                        Main.dust[Smoke].noGravity = true;
                        Main.dust[Smoke].position = Projectile.Center + vector12;
                        Main.dust[Smoke].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity * 0.2f;
                    }

                    currentAmount++;
                }

                Projectile.ai[1] = 0;
                Projectile.ai[2] = Main.rand.Next(-60, 1);
                Projectile.netUpdate = true;
            }
        }

        public void IdleAI(Player player)
		{
            Projectile.ai[1] = 0;
            Projectile.ai[2] = 0;

            Projectile.spriteDirection = player.Center.X > Projectile.Center.X ? -1 : 1;

            //movement stuff
            if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
            {
                Projectile.ai[0] = 1f;
            }

            float speed = 8f;
            if (Projectile.ai[0] == 1f)
            {
                speed = 15f;
            }

            Vector2 center = Projectile.Center;
            Vector2 direction = player.Center - center;
            Projectile.netUpdate = true;
            
            direction.Y -= 25f;
            float distanceTo = direction.Length();
            if (distanceTo > 400f && speed < 9f)
            {
                speed = 9f;
            }
            if (distanceTo < 300f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (distanceTo > 2000f)
            {
                Projectile.Center = player.Center;
            }
            if (distanceTo > 100f)
            {
                direction.Normalize();
                direction *= speed;
                float temp = 40 / 2f;
                Projectile.velocity = (Projectile.velocity * temp + direction) / (temp + 1);
            }
            else
            {
                Projectile.velocity *= (float)Math.Pow(0.9, 40.0 / 40);
            }

            if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
            {
                return;
            }
        }
    }
}