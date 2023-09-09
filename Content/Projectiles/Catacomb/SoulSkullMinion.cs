using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class SoulSkullMinion : ModProjectile
    {
        int charge = 0;

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
            if (Projectile.velocity.X != 0 && Projectile.velocity.Y != 0)
            {
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
                Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

                for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
                {
                    var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(Color.Green) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                    Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                    Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
                }
            }
			
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Lighting.AddLight(Projectile.Center, 0f, 0.25f, 0f);

			if (player.dead)
			{
				player.GetModPlayer<SpookyPlayer>().SoulSkull = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().SoulSkull)
			{
				Projectile.timeLeft = 2;
			}

            for (int i = 0; i < 200; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this, false) && !NPCID.Sets.CountsAsCritter[Target.type])
                {
                    Shoot(Target);

                    break;
                }

                NPC NPC = Main.npc[i];
                if (NPC.active && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 550f)
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

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y,
                    perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<SoulSkullBolt>(), Projectile.damage, 0f, Main.myPlayer, 0f, 0f);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, Projectile.Center);

                    Vector2 ChargeDirection = target.Center - Projectile.Center;
                    ChargeDirection.Normalize();

                    ChargeDirection.X *= 20;
                    ChargeDirection.Y *= 20;
                    Projectile.velocity.X = ChargeDirection.X;
                    Projectile.velocity.Y = ChargeDirection.Y;

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