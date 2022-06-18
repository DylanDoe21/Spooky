using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.Orroboro.Projectiles
{
    public class OrroboroEyeSpawner : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spooky Eye");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 52;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.LimeGreen) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new Rectangle(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
			
            return true;
        }
        
        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] <= 85)
            {   
                Projectile.velocity *= 1.03f;

                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 10;
                }
            }

            if (Projectile.localAI[0] >= 120)
            {
                SoundEngine.PlaySound(SoundID.Item103, Projectile.position);

                //spawn orroboro with message
                int Orroboro = NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y + 35, ModContent.NPCType<OrroboroHead>());
                Main.NewText("Orro-Boro has awoken!", 171, 64, 255);

                //net update so the worm wont vanish on multiplayer
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: Orroboro);
                }

                for (int i = 0; i < 35; i++)
				{
                    int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 65, 0f, -2f, 0, default(Color), 1.5f);
                    Main.dust[newDust].noGravity = true;
                    Main.dust[newDust].scale = 2.5f;
                    Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;

                    if (Main.dust[newDust].position != Projectile.Center)
                    {
                        Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
                    }
                }

                Projectile.Kill();
            }
        }
    }
}