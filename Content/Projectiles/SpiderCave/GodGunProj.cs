using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Dusts;


namespace Spooky.Content.Projectiles.SpiderCave
{
	public class GodGunProj : ModProjectile
	{
        int ExtraUseTime = 0;
        int OverheatTimer = 0;
        int playerCenterOffset = 4;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
            Projectile.width = 70;
            Projectile.height = 138;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 20;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}

        public override bool? CanDamage()
        {
			return false;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
		{
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead || player.noItems || player.CCed) 
            {
                Projectile.Kill();
            }

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
                ProjDirection.Normalize();
                Projectile.ai[0] = ProjDirection.X;
				Projectile.ai[1] = ProjDirection.Y;
            }

            Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            Projectile.direction = Projectile.spriteDirection = direction.X > 0 ? 1 : -1;

            if (Projectile.direction >= 0)
            {
                Projectile.rotation = direction.ToRotation() - 1.57f * (float)Projectile.direction;
            }
            else
            {
                Projectile.rotation = direction.ToRotation() + 1.57f * (float)Projectile.direction;
            }

            player.itemRotation = Projectile.rotation;

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 2 - Projectile.height / 2);

            if (direction.X > 0) 
            {
                player.direction = 1;
            }
            else 
            {
                player.direction = -1;
            }

			if (player.channel) 
            {
                Projectile.timeLeft = 2;

				player.velocity.X *= 0.98f;

                OverheatTimer++;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime - ExtraUseTime)
                {
                    Projectile.frame++;

                    //spawn smoke when overheating
                    if (OverheatTimer > 360 && Main.rand.NextBool(5))
                    {
                        Vector2 Offset = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
                        Offset.Normalize();
                        Offset *= 20;

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(Offset.X, Offset.Y)) * 45f;

                        int DustGore = Dust.NewDust(player.position + muzzleOffset, player.width / 2, player.height / 2, 
                        ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.2f, 0.6f));
                        Main.dust[DustGore].velocity.X *= 0.2f;
                        Main.dust[DustGore].velocity.Y *= Main.rand.NextFloat(0f, 1f);
                        Main.dust[DustGore].noGravity = true;

                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[DustGore].scale = 0.5f;
                            Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }

                    //explode and damage the player
                    if (OverheatTimer > 660)
                    {
                        player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " " + Language.GetTextValue("Mods.Spooky.DeathReasons.GodGunExplode")), 150 + Main.rand.Next(-30, 30), 0);

                        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

                        for (int numDust = 0; numDust < 75; numDust++)
                        {
                            int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, 0f, -2f, 0, default, 2f);
                            Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-8f, 9f);
                            Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-8f, 9f);
                            Main.dust[dustGore].scale = Main.rand.NextFloat(2f, 3f);
                            Main.dust[dustGore].noGravity = true;
                        }

                        for (int numExplosion = 0; numExplosion < 25; numExplosion++)
                        {
                            int DustGore = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2,
                            ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.8f, 1.2f));

                            Main.dust[DustGore].velocity *= Main.rand.NextFloat(-5f, 6f);
                            Main.dust[DustGore].noGravity = true;

                            if (Main.rand.NextBool())
                            {
                                Main.dust[DustGore].scale = 0.5f;
                                Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }

                        Projectile.Kill();
                    }

                    if (Projectile.frame > 3)
                    {
                        SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);

                        Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
                        ShootSpeed.Normalize();
                        ShootSpeed *= 20;

                        int ProjType = ProjectileID.Bullet;

                        float Speed = 20f;

                        float knockBack = ItemGlobal.ActiveItem(player).knockBack;

                        player.PickAmmo(ItemGlobal.ActiveItem(player), out ProjType, out Speed, out Projectile.damage, out knockBack, out _);

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 45f;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y - playerCenterOffset, 
                        ShootSpeed.X + Main.rand.Next(-3, 4), ShootSpeed.Y + Main.rand.Next(-3, 4), ProjType, Projectile.damage, knockBack, Projectile.owner);

                        if (ExtraUseTime < ItemGlobal.ActiveItem(player).useTime - 1)
                        {
                            ExtraUseTime++;
                        }

                        Projectile.frame = 0;
                    }

                    Projectile.localAI[0] = 0;
                }

                //kill this holdout projectile if the player has no more arrows
                if (!player.HasAmmo(ItemGlobal.ActiveItem(player)))
                {
                    Projectile.Kill();
                }
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}