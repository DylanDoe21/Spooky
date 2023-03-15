using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;
using Spooky.Content.Events;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class EggEventShield : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Egg Barrier");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 2; 
            Projectile.alpha = 255;
        }

        public override void PostDraw(Color lightColor)
        {
            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            var center = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            float intensity = fade;
            DrawData drawData = new DrawData(ModContent.Request<Texture2D>("Spooky/ShaderAssets/Liquid").Value, center,
            new Rectangle(0, 0, 500, 420), Color.Purple, 0, new Vector2(250f, 250f), Projectile.scale * (1f + intensity * 0.05f), SpriteEffects.None, 0);
            GameShaders.Misc["ForceField"].UseColor(new Vector3(1f + intensity * 0.5f));
            GameShaders.Misc["ForceField"].Apply(drawData);
            drawData.Draw(Main.spriteBatch);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin();

            return;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Projectile.timeLeft = 2;

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 1)
            {
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.Center);

                //event start message
                string text = "The Egg Incusrion has begun!";

                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText(text, 171, 64, 255);
                }
                else
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), 171, 64, 255);
                }

                //set egg event to true, net update on multiplayer
                EggEventWorld.EggEventActive = true;

                if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
            }

            //spawn dust particles that get sucked towards this projectile, which is basically the egg
            if (Projectile.ai[0] % 20 == 0)
            {
                int MaxDusts = Main.rand.Next(10, 15);
                float distance = 50f;

                for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                {
                    Vector2 dustPos = (Vector2.One * new Vector2((float)Projectile.width / 3f, (float)Projectile.height / 3f) * distance).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + Projectile.Center;
                    Vector2 velocity = dustPos - Projectile.Center;

                    if (Main.rand.Next(2) == 0)
                    {
                        int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, DustID.DemonTorch, velocity.X * 2f, velocity.Y * 2f, 100, default, 1f);
                        Main.dust[dustEffect].scale = 5f;
                        Main.dust[dustEffect].noGravity = true;
                        Main.dust[dustEffect].noLight = false;
                        Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * (-50f);
                        Main.dust[dustEffect].fadeIn = 1.2f;
                    }
                }
            }

            //always kill this projectile if the egg event is not active
            if (!EggEventWorld.EggEventActive)
            {
                Projectile.Kill();
            }
        }
    }
}