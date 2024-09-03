using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SentientPaladinsHammerProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/SpookyHell/Sentient/SentientPaladinsHammer";

        bool isAttacking = false;
        bool hasHitEnemy = false;

        private static Asset<Texture2D> ProjTexture;

        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/SentientPaladinsHammerHit", SoundType.Sound) { PitchVariance = 0.6f, Volume = 0.5f };

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 54;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lime * 0.5f) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length) * 0.5f;
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, effects, 0);
            }
            
            return true;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] == 1 && Projectile.ai[1] >= 20 && !hasHitEnemy;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(HitSound, target.Center);

            SpookyPlayer.ScreenShakeAmount = 4;

            for (int numDusts = 0; numDusts < 10; numDusts++)
			{                                                                   
				int newDust = Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<CartoonStar>(), 0f, -2f, 0, default, 1f);
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                Main.dust[newDust].noGravity = true;
                
				if (Main.dust[newDust].position != target.Center)
				{
					Main.dust[newDust].velocity = target.DirectionTo(Main.dust[newDust].position) * 1.2f;
				}
			}

            hasHitEnemy = true;
        }

        public override void AI()
        {
            Projectile.timeLeft = 2;

            Player owner = Main.player[Projectile.owner];

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation += 0.65f * (float)Projectile.direction;

            Projectile.localAI[0]++;

            //target an enemy
            for (int i = 0; i < 200; i++)
            {
                NPC NPC = Main.npc[i];

                //prioritize bosses over normal enemies
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && NPC.boss && Vector2.Distance(Projectile.Center, NPC.Center) <= 550f)
                {
                    AttackingAI(NPC, owner);
                    break;
                }
                //if no boss is found, target other enemies normally
                else if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 550f)
                {
                    AttackingAI(NPC, owner);
                    break;
                }
                else
                {
                    isAttacking = false;
                }
            }

            //if no enemy is being targeted, then return to the player
            if (!isAttacking && Projectile.localAI[0] >= 15)
            {
                ReturnToPlayer(owner);
            }
        }

        public void AttackingAI(NPC target, Player owner)
        {
            isAttacking = true;

            switch ((int)Projectile.ai[0])
            {
                //chase the targeted npc
                case 0:
                {
                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 150;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);

                    //once the projectile reaches the target, begin charging at it
                    if (Projectile.Hitbox.Intersects(target.Hitbox))
                    {
                        Projectile.ai[0]++;
                    }

                    break;
                }

                //charge at the enemy 3 times
                case 1:
                {
                    if (Projectile.ai[2] < 3)
                    {
                        Projectile.ai[1]++;

                        if (Projectile.ai[1] < 20)
                        {
                            Vector2 GoTo = target.Center;
                            GoTo.Y -= 200;

                            float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 12, 25);
                            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (Projectile.ai[1] == 20)
                        {
                            Vector2 ChargeDirection = target.Center - Projectile.Center;
                            ChargeDirection.Normalize();
                            ChargeDirection *= 45;
                            Projectile.velocity = ChargeDirection;
                        }

                        if (Projectile.ai[1] >= 35)
                        {
                            Projectile.velocity *= 0.3f;
                        }

                        if (Projectile.ai[1] >= 40)
                        {
                            hasHitEnemy = false;
                            Projectile.ai[1] = 0;
                            Projectile.ai[2]++;
                        }
                    }
                    else
                    {
                        Projectile.ai[1] = 0;
                        Projectile.ai[2] = 0;
                        Projectile.ai[0]++;
                    }

                    break;
                }

                //go back to the player
                case 2:
                {
                    ReturnToPlayer(owner);

                    break;
                }
            }
        }

        public void ReturnToPlayer(Player owner)
        {
            Projectile.knockBack = 0;

            Vector2 ReturnSpeed = owner.Center - Projectile.Center;
            ReturnSpeed.Normalize();
            ReturnSpeed *= 40;

            Projectile.velocity = ReturnSpeed;

            if (Projectile.Hitbox.Intersects(owner.Hitbox))
            {
                Projectile.Kill();
            }
        }
    }
}