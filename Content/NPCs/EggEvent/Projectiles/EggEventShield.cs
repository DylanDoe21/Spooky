using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Events;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class EggEventShield : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 2; 
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            var center = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            float intensity = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;
            DrawData drawData = new DrawData(ModContent.Request<Texture2D>("Spooky/ShaderAssets/Noise").Value, center,
            new Rectangle(0, 0, 500, 420), Color.BlueViolet, 0, new Vector2(250f, 250f), Projectile.scale * (1f + intensity * 0.05f), SpriteEffects.None, 0);

            GameShaders.Misc["ForceField"].UseColor(new Vector3(1f + intensity * 0.5f));
            GameShaders.Misc["ForceField"].Apply(drawData);
            drawData.Draw(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            //spawn dust particles that get sucked towards this projectile, which is technically the egg
            if (Projectile.ai[0] % 20 == 0)
            {
                int MaxDusts = Main.rand.Next(10, 15);
                float distance = 50f;

                for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                {
                    Vector2 dustPos = (Vector2.One * new Vector2((float)Projectile.width / 3f, (float)Projectile.height / 3f) * distance).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + Projectile.Center;
                    Vector2 velocity = dustPos - Projectile.Center;

                    if (Main.rand.NextBool(2))
                    {
                        int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, DustID.DemonTorch, velocity.X * 2f, velocity.Y * 2f, 100, default, 1f);
                        Main.dust[dustEffect].scale = 5f;
                        Main.dust[dustEffect].noGravity = true;
                        Main.dust[dustEffect].noLight = false;
                        Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * (-50f);
                        Main.dust[dustEffect].fadeIn = 1.2f;
                    }
                }

                Projectile.ai[0] = 2;
            }

            //always kill this projectile if the egg event is not active
            if (EggEventWorld.EggEventActive)
            {
                Projectile.timeLeft = 5;
            }
            else
            {
                Projectile.Kill();
            }
        }
    }
}