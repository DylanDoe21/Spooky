using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.SpookFishron.Projectiles
{
    public class SpooklizzardStaffSpin : ModProjectile
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/SpookFishron/Projectiles/SpooklizzardStaff";

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 vector = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + new Vector2(23, 23) + new Vector2(0, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Cyan) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length) * 0.65f;
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, effects, 0);
            }
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            return false;
        }

		public override bool? CanDamage()
		{
			return Projectile.ai[0] > 0;
		}

		public override void AI()
        {
            Projectile.rotation += 0.5f * Projectile.direction;

            if (Projectile.ai[0] == 0)
            {
                NPC Parent = Main.npc[(int)Projectile.ai[1]];

                if (!Parent.active || Parent.type != ModContent.NPCType<SpookFishron>())
                {
                    Projectile.Kill();
                }

                if (Parent.active && Parent.type == ModContent.NPCType<SpookFishron>() && Parent.ai[0] != 6)
                {
                    Projectile.Kill();
                }

                Projectile.timeLeft = 5;

                Vector2 pos = new Vector2(100, 0).RotatedBy(Parent.rotation + MathHelper.PiOver2);

                Vector2 desiredVelocity = Projectile.DirectionTo(pos + Parent.Center) * 55;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);

                Rectangle collision = new Rectangle((int)pos.X + (int)Parent.Center.X - 50, (int)pos.Y + (int)Parent.Center.Y - 50, 100, 100);

                if (Projectile.Hitbox.Intersects(collision))
                {
                    Parent.localAI[0] = 236; //set parent ai to immediately start attacking

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Parent.Center, Vector2.Zero,
                    ModContent.ProjectileType<SpooklizzardStaff>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[1]);

                    Projectile.Kill();
                }
            }
		}
    }
}