using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class MiteVacuumProj : ModProjectile
	{
        int playerCenterOffset = 14;

        private static Asset<Texture2D> ChainTexture;

		public override void SetDefaults()
		{
            Projectile.width = 36;
            Projectile.height = 90;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}

        public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

			ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SpiderCave/MiteVacuumProjChain");

            bool flip = false;
            if (player.direction == -1)
            {
                flip = true;
            }

            Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
            Vector2 myCenter = Projectile.Center - new Vector2(5 * (flip ? -1 : 1), -5).RotatedBy(Projectile.rotation);
            Vector2 p0 = player.MountedCenter - new Vector2(14 * (flip ? -1 : 1), 3).RotatedBy(player.fullRotation);
            Vector2 p1 = player.MountedCenter - new Vector2(14 * (flip ? -1 : 1), 3).RotatedBy(player.fullRotation);
            Vector2 p2 = myCenter - new Vector2(12 * (flip ? -1 : 1), 10).RotatedBy(Projectile.rotation);
            Vector2 p3 = myCenter;

            int segments = 10;

            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)segments;
                Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
                t = (i + 1) / (float)segments;
                Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
                Vector2 toNext = (drawPosNext - drawPos2);
                float rotation = toNext.ToRotation();
                float distance = toNext.Length();

                lightColor = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

                Main.spriteBatch.Draw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(lightColor), rotation, drawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
            }

			return true;
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

            if (!player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
			{
				Projectile.Kill();
			}

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y + playerCenterOffset);
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
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 7 - Projectile.height / 2);

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

                Projectile.localAI[0]++;
                if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime)
                {
                    SoundEngine.PlaySound(SoundID.Item34 with { Pitch = -0.75f }, Projectile.Center);

                    if (Projectile.owner == Main.myPlayer)
                    {
                        if (Main.rand.NextBool(15))
                        {
                            Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y);
                            ShootSpeed.Normalize();
                            ShootSpeed *= 12f;

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 42f;

                            int newMite = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center + muzzleOffset, ShootSpeed, 
                            ModContent.ProjectileType<MiteProjectile>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                            Main.projectile[newMite].DamageType = DamageClass.Ranged;
                            Main.projectile[newMite].ai[0] = Main.rand.Next(5, 7);
                            Main.projectile[newMite].ai[2] = 3;
                            Main.projectile[newMite].penetrate = 3;
                            Main.projectile[newMite].localNPCHitCooldown = 30;
                            Main.projectile[newMite].usesLocalNPCImmunity = true;
                        }
                        else
                        {   
                            Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y);
                            ShootSpeed.Normalize();
                            ShootSpeed *= Main.rand.NextFloat(15f, 25f);

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 42f;

                            Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(35));

                            int newSpore = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center + muzzleOffset, newVelocity, 
                            ModContent.ProjectileType<MiteVacuumSpore>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            Main.projectile[newSpore].DamageType = DamageClass.Ranged;
                            Main.projectile[newSpore].ai[0] = Main.rand.Next(0, 2);
                        }
                    }

                    Projectile.localAI[0] = 0;
                }
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}