using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class SoulSkullMinion : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 26;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
			Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
			Projectile.minionSlots = 1;
            Projectile.aiStyle = 62;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Green);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int numEffect = 0; numEffect < 5; numEffect++)
            {
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (numEffect / 5 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(-1, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale * 1.25f, effects, 0);
            }
			
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Lighting.AddLight(Projectile.Center, 0f, 0.25f, 0f);

			if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<SoulSkullBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<SoulSkullBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type])
                {
                    Shoot(Target);

                    break;
                }

                NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 550f)
                {
                    Shoot(NPC);

                    break;
                }
            }

            //prevent Projectiles clumping together
            for (int num = 0; num < Main.projectile.Length; num++)
			{
				Projectile other = Main.projectile[num];
				if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
				{
					const float pushAway = 0.08f;
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

        public void Shoot(NPC target)
        {
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] == 60)
            {
                if (Projectile.localAI[1] < 5)
                {
                    SoundEngine.PlaySound(SoundID.Zombie53, Projectile.Center);

                    float Speed = 25f;
                    Vector2 vector = new(Projectile.position.X + (Projectile.width / 2), Projectile.position.Y + (Projectile.height / 2));
                    float rotation = (float)Math.Atan2(vector.Y - (target.position.Y + (target.height * 0.5f)), vector.X - (target.position.X + (target.width * 0.5f)));
                    Vector2 perturbedSpeed = new Vector2((float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1));

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, ModContent.ProjectileType<SoulSkullBolt>(), Projectile.damage, 0f, Projectile.owner);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.Center);

                    Vector2 ChargeDirection = target.Center - Projectile.Center;
                    ChargeDirection.Normalize();
                    ChargeDirection *= 35;
                    Projectile.velocity = ChargeDirection;

                    Projectile.localAI[1] = 0;
                }
            }

            if (Projectile.localAI[0] >= 60)
            {
                Projectile.localAI[0] = 0;
                Projectile.localAI[1]++;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}
    }
}