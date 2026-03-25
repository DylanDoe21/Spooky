using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class SporeTomeMushroom : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> AuraTexture;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}
		
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            AuraTexture ??= ModContent.Request<Texture2D>(Texture + "Aura");

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int i = 0; i < 360; i += 90)
            {
                Color RealColor = Color.White;

                switch ((int)Projectile.ai[0])
                {
                    case 0:
                    {
                        RealColor = Color.SaddleBrown;
                        break;
                    }
                    case 1:
                    {
                        RealColor = Color.Red;
                        break;
                    }
                    case 2:
                    {
                        RealColor = Color.Gold;
                        break;
                    }
                    case 3:
                    {
                        RealColor = Color.Indigo;
                        break;
                    }
                }

                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(RealColor);

                Vector2 circular = new Vector2(Main.rand.NextFloat(2f, 2f), Main.rand.NextFloat(2f, 2f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(AuraTexture.Value, vector + circular, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.ai[0] == 0 || Projectile.ai[0] == 1)
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
            }

			return false;
		}

        public override void AI()
		{
            Projectile.frame = (int)Projectile.ai[0];

            if (Projectile.timeLeft < 60 && Projectile.ai[0] != 3)
            {
                Projectile.alpha += 5;
            }
        
            //different behaviors
            switch ((int)Projectile.ai[0])
            {
                //travel with gravity
                case 0:
                {
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    Projectile.rotation += 0f * (float)Projectile.direction;

                    break;
                }
                //spin and slow down
                case 1:
                {
                    Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.06f * (float)Projectile.direction;
                    Projectile.velocity *= 0.975f;

                    break;
                }
                //homing behavior
                case 2:
                {
                    Projectile.tileCollide = false;

                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			        Projectile.rotation += 0f * (float)Projectile.direction;

                    int foundTarget = FindTarget();
                    if (foundTarget != -1)
                    {
                        NPC target = Main.npc[foundTarget];
                        Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 25;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                        Projectile.tileCollide = false;
                    }

                    if (IsColliding())
                    {
                        Projectile.ai[1] = 1;
                    }
                    if (Projectile.ai[1] > 0)
                    {
                        Projectile.velocity *= 0.95f;

                        Projectile.alpha += 5;
                        if (Projectile.alpha >= 255)
                        {
                            Projectile.Kill();
                        }
                    }

                    break;
                }
                //float and explode on contact
                case 3:
                {
                    Projectile.rotation = Projectile.velocity.X * 0.01f;

                    Projectile.ai[1]++;
                    if (Projectile.ai[1] < 45)
                    {
                        Projectile.velocity *= 0.96f;
                    }
                    else
                    {
                        float update = Main.GlobalTimeWrappedHourly * 0.08f * 16;

                        Projectile.velocity.X += (float)Math.Sin(update) * 0.01f;
                        
                        if (Projectile.velocity.Y < 4f)
                        {
                            Projectile.velocity.Y = Projectile.velocity.Y + Main.rand.NextFloat(0f, 0.01f);
                        }
                    }

                    break;
                }
            }
        }

        private int FindTarget()
        {
            const float homingMaximumRangeInPixels = 350;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }

        public bool IsColliding()
        {
            int minTilePosX = (int)(Projectile.position.X / 16) - 1;
            int maxTilePosX = (int)((Projectile.position.X + Projectile.width) / 16) + 2;
            int minTilePosY = (int)(Projectile.position.Y / 16) - 1;
            int maxTilePosY = (int)((Projectile.position.Y + Projectile.height) / 16) + 2;
            if (minTilePosX < 0)
            {
                minTilePosX = 0;
            }
            if (maxTilePosX > Main.maxTilesX)
            {
                maxTilePosX = Main.maxTilesX;
            }
            if (minTilePosY < 0)
            {
                minTilePosY = 0;
            }
            if (maxTilePosY > Main.maxTilesY)
            {
                maxTilePosY = Main.maxTilesY;
            }

            for (int i = minTilePosX; i < maxTilePosX; ++i)
            {
                for (int j = minTilePosY; j < maxTilePosY; ++j)
                {
                    if (Main.tile[i, j] != null && WorldGen.SolidTile(i, j))
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);

                        if (Projectile.position.X + Projectile.width > vector2.X && Projectile.position.X < vector2.X + 16.0 && 
                        (Projectile.position.Y + Projectile.height > (double)vector2.Y && Projectile.position.Y < vector2.Y + 16.0))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void OnKill(int timeLeft)
		{
            if (Projectile.ai[0] == 3)
            {
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

                for (int i = 0; i < 7; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 velocity = new Vector2(0, Main.rand.Next(5, 10)).RotatedByRandom(MathHelper.ToRadians(360));

                        int newProj = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, velocity, 
                        ModContent.ProjectileType<SporeCloud>(), Projectile.damage / 3, Projectile.knockBack, ai0: 1);
                        Main.projectile[newProj].DamageType = DamageClass.Magic;
                        Main.projectile[newProj].alpha = 125;
                    }
                }
            }
        }
    }
}
     
          






