using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Cemetery
{
	public class PolybiusSwirl : ModProjectile
    {
        int MissingCharges = 0;

        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
			Projectile.width = 138;
            Projectile.height = 138;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Cemetery/PolybiusSwirlBack");

            Color color = new Color(lightColor.R, lightColor.G, lightColor.B, -Projectile.alpha);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + drawOrigin + new Vector2(-69, Projectile.gfxOffY - 69);

            Main.EntitySpriteDraw(ProjTexture.Value, vector, null, color, -Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return true;
        }

        public override bool? CanDamage()
        {
			return MissingCharges < 5;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            MissingCharges++;
            //Projectile.alpha += 51;
            Projectile.ai[0] = 0;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 2;

            Projectile.rotation -= 0.05f;

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.position = new Vector2(Main.MouseWorld.X - (Projectile.width / 2), Main.MouseWorld.Y - (Projectile.height / 2));
            }

            if (!player.GetModPlayer<SpookyPlayer>().PolybiusArcadeGame || Main.gamePaused)
            {
                Projectile.Kill();
            }

            player.GetCritChance(DamageClass.Generic) += 2 * MissingCharges;

            Projectile.alpha = 51 * MissingCharges;

            if (Projectile.alpha > 0)
            {
                Projectile.ai[0]++;

                if (Projectile.ai[0] >= 120 && MissingCharges > 0)
                {
                    MissingCharges--;
                    Projectile.ai[0] = 0;
                }
            }
        }
    }
}