using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.Slingshots
{
    public class TarFlingerBlob : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileGlobal.IsSlingshotAmmoProj[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 180;
			Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPosOld = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPosOld, rectangle, Color.Gray * 0.75f, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void AI()       
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.05f;
        }

		public override void OnKill(int timeLeft)
		{
            Player player = Main.player[Projectile.owner];

			SoundEngine.PlaySound(SoundID.Item126, Projectile.Center);

            foreach (NPC target in Main.ActiveNPCs)
            {
                if (target.Distance(Projectile.Center) <= 120f && target.CanBeChasedBy(this) && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type])
				{
                    player.ApplyDamageToNPC(target, Projectile.damage, 0, 0, false, null, true);
                    target.AddBuff(ModContent.BuffType<TarFlingerSlow>(), 180);
                }
            }

            float maxAmount = 15;
            int currentAmount = 0;
            while (currentAmount <= maxAmount)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(1f, 10f), Main.rand.NextFloat(1f, 10f));
                Vector2 Bounds = new Vector2(Main.rand.NextFloat(1f, 6f), Main.rand.NextFloat(1f, 6f));
                float intensity = Main.rand.NextFloat(1f, 10f);

                Vector2 vector12 = Vector2.UnitX * 0f;
                vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                int num104 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Asphalt, 0f, 0f, 100, default, 3f);
                Main.dust[num104].noGravity = true;
                Main.dust[num104].position = Projectile.Center + vector12;
                Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
                currentAmount++;
            }
		}
    }
}