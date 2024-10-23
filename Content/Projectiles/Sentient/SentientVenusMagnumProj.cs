using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Sentient
{
	public class SentientVenusMagnumProj : ModProjectile
	{
        int playerCenterOffset = 4;
        
        float ExtraUseTime = 0;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
            Projectile.width = 46;
            Projectile.height = 100;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
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
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 10 - Projectile.height / 2);

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
                //edit the projectiles usetime so it shoots the giant glob if at max shoot speed
                if (ExtraUseTime >= 8)
                {
                    Projectile.timeLeft = 20;
                }
                else
                {
                    Projectile.timeLeft = 2;
                }

                Projectile.localAI[0] += 0.25f;

                if (Projectile.localAI[0] >= 8 - ExtraUseTime)
                {
                    Projectile.frame++;

                    if (Projectile.frame > 3)
                    {
                        SoundEngine.PlaySound(SoundID.NPCHit21 with { Pitch = 1.25f }, Projectile.Center);

                        int ProjType = ProjectileID.Bullet;

                        float Speed = 15f;

                        float knockBack = ItemGlobal.ActiveItem(player).knockBack;

                        player.PickAmmo(ItemGlobal.ActiveItem(player), out ProjType, out Speed, out Projectile.damage, out knockBack, out _);

                        ProjType = Main.rand.NextBool(15) ? ModContent.ProjectileType<OozeTooth>() : ModContent.ProjectileType<OozeSmall>();
                        
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
                            ShootSpeed.Normalize();
                            ShootSpeed *= 15f;

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 50f;

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y - playerCenterOffset, 
                            ShootSpeed.X + Main.rand.Next(-1, 2), ShootSpeed.Y + Main.rand.Next(-1, 2), ProjType, Projectile.damage, knockBack, Projectile.owner);
                        }

                        if (ExtraUseTime < 8)
                        {
                            ExtraUseTime += 0.5f;
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
            else
            {
                //launch massive ooze if at max shooting speed
                if (Projectile.timeLeft >= 19)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit21 with { Pitch = 0.75f }, Projectile.Center);

                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
                        ShootSpeed.Normalize();
                        ShootSpeed *= 20;

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 50f;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y - playerCenterOffset, 
                        ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<OozeBig>(), Projectile.damage * 5, Projectile.knockBack, Projectile.owner);
                    }
                }
            }

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}