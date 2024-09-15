using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Sentient
{
    public class OrganicChunk : ModProjectile
    {
        int Bounces = 0;

        bool IsStickingToTarget = false;

		private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
			Projectile.width = 26;
            Projectile.height = 26;
            Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 300;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			float stretch = Projectile.velocity.Y * 0.1f;

			stretch = Math.Abs(stretch);

			//limit how much it can stretch
			if (stretch > 0.2f)
			{
				stretch = 0.2f;
			}

			//limit how much it can squish
			if (stretch < -0.2f)
			{
				stretch = -0.2f;
			}

			Vector2 scaleStretch = new Vector2(1f - stretch, 1f + stretch);

            if (Projectile.velocity.Y <= 0)
			{
				scaleStretch = new Vector2(1f + stretch, 1f - stretch);
			}
			if (Projectile.velocity.Y > 0)
			{
				scaleStretch = new Vector2(1f - stretch, 1f + stretch);
			}

            int height = ProjTexture.Height() / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameHeight, ProjTexture.Width(), height);

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, height / 2f), scaleStretch, SpriteEffects.None, 0);

			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.frame < 3)
            {
                Projectile.Kill();
            }

            if (Projectile.frame == 4)
            {
                int WhoAmI = target.whoAmI;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC NPC = Main.npc[i];
                    if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(target.Center, NPC.Center) <= 550f)
                    {
                        //dont allow the projectile to bounce to the same enemy it already hit
                        if (i == WhoAmI)
                        {
                            continue;
                        }

                        Vector2 ChargeDirection = NPC.Center - target.Center;
                        ChargeDirection.Normalize();
                        ChargeDirection *= 20f;
                        Projectile.velocity = ChargeDirection;

                        break;
                    }
                }
            }

            if (!IsStickingToTarget && Projectile.frame == 5)
            {
                Projectile.timeLeft = 120;
                Projectile.ai[1] = target.whoAmI;
                Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
                IsStickingToTarget = true;
                Projectile.netUpdate = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            Bounces++;
			if (Bounces >= 3)
			{
				Projectile.Kill();
			}
			else
			{
                //SoundEngine.PlaySound(SoundID.NPCHit13 with { Pitch = 1.25f }, Projectile.Center);

                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                    Projectile.velocity.X = -oldVelocity.X * 0.75f;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                    Projectile.velocity.Y = -oldVelocity.Y * 0.75f;
                }
            }

			return false;
		}
		
		public override void AI()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            //different behaviors for each different variant
            switch (Projectile.frame)
            {
                //normal, just flies in one direction
                case 0:
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.22f;
                    break;
                }
                //same as the first frame
                case 1:
                {
                    goto case 0;
                }
                //eye variant can home in on enemies
                case 2:
                {
                    int foundTarget = HomeOnTarget();
                    if (foundTarget != -1)
                    {
                        NPC target = Main.npc[foundTarget];
                        Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 25;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                    }

                    Projectile.velocity.Y = Projectile.velocity.Y + 0.22f;

                    break;
                }
                //mouth variant accelerates for a bit
                case 3:
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.22f;
                    Projectile.velocity *= 1.01f;

                    break;
                }
                //artery variant bounces between enemies on hit
                case 4:
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.22f;
                    
                    break;
                }
                //tooth variant sticks to enemies for a bit and damages them
                case 5:
                {
                    if (!IsStickingToTarget)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + 0.22f;
                    }

                    break;
                }
            }

            if (IsStickingToTarget) 
            {
				Projectile.ignoreWater = true;
                Projectile.tileCollide = false;

                int npcTarget = (int)Projectile.ai[1];
                if (npcTarget < 0 || npcTarget >= 200) 
                {
                    Projectile.Kill();
                }
                else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage) 
                {
                    Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
                }
                else 
                {
                    Projectile.Kill();
                }
			}
		}

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 200;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.Center);

            for (int numDusts = 0; numDusts < 10; numDusts++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;

				if (Main.dust[dust].position != Projectile.Center)
                {
				    Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
                }
			}
        }
    }
}