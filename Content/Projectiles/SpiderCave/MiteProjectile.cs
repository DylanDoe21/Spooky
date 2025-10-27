using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class MiteProjectile : ModProjectile
    {
        public int WaveTimer = 0;
        public bool IsStickingToTarget = false;

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> AuraTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
			Projectile.penetrate = 2;
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
                        RealColor = Color.White;
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
                    case 4:
                    {
                        RealColor = Color.Blue;
                        break;
                    }
                    case 5:
                    {
                        RealColor = Color.Green;
                        break;
                    }
                    case 6:
                    {
                        RealColor = Color.Turquoise;
                        break;
                    }
                    case 7:
                    {
                        RealColor = Color.Red;
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

        public override bool? CanDamage()
		{
			return (Projectile.ai[2] == 0 && Projectile.ai[1] > 60) || Projectile.ai[2] > 0;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!IsStickingToTarget && Projectile.ai[2] == 1)
            {
                Projectile.ai[1] = target.whoAmI;
                Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
                IsStickingToTarget = true;
                Projectile.netUpdate = true;
            }

            if (Projectile.ai[2] == 3)
            {
                int WhoAmI = target.whoAmI;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC NPC = Main.npc[i];
                    if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(target.Center, NPC.Center) <= 400f)
                    {
                        //dont allow the bullet to bounce to the same enemy it already hit
                        if (i == WhoAmI)
                        {
                            continue;
                        }

                        SoundEngine.PlaySound(SoundID.Item56, NPC.Center);

                        Vector2 ChargeDirection = NPC.Center - target.Center;
                        ChargeDirection.Normalize();
                        ChargeDirection *= 35f;
                        Projectile.velocity = ChargeDirection;

                        break;
                    }
                }
            }
        }

        public override void AI()
        {
            Projectile.frame = (int)Projectile.ai[0];

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;
        
            //different behaviors
            switch ((int)Projectile.ai[2])
            {
                //home after a second
                case 0:
                {
                    Projectile.ai[1]++;
                    if (Projectile.ai[1] > 60)
                    {
                        int foundTarget = HomeOnTarget();
                        if (foundTarget != -1)
                        {
                            NPC target = Main.npc[foundTarget];
                            Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 8;
                            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                            Projectile.tileCollide = false;
                        }
                    }

                    break;
                }
                //stick to enemies
                case 1:
                {
                    if (IsStickingToTarget) 
                    {
                        Projectile.ignoreWater = true;
                        Projectile.tileCollide = false;

                        int npcTarget = (int)Projectile.ai[1];
                        if (npcTarget < 0 || npcTarget >= 200) 
                        {
                            Projectile.Kill();
                        }
                        else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage) 
                        {
                            Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                            Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
                        }
                        else 
                        {
                            Projectile.Kill();
                        }
                    }
                    else
                    {
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
                    }

                    break;
                }
                //bounce around and home in on targets horizontally
                case 2:
                {
                    Projectile.tileCollide = true;

                    Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;

                    Projectile.ai[1]++;
                    if (Projectile.ai[1] > 60)
                    {
                        int foundTarget = HomeOnTarget();
                        if (foundTarget != -1)
                        {
                            NPC target = Main.npc[foundTarget];
                            Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 8;
                            Projectile.velocity.X = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20).X;
                        }
                    }

                    break;
                }
                //move in a wave pattern
                case 3:
                {
                    Projectile.tileCollide = true;

                    float WaveIntensity = 5f;
                    float Wave = 8f;

                    WaveTimer++;
                    if (Projectile.ai[1] == 0)
                    {
                        if (WaveTimer > Wave * 0.5f)
                        {
                            WaveTimer = 0;
                            Projectile.ai[1] = 1;
                        }
                        else
                        {
                            Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
                            Projectile.velocity = perturbedSpeed;
                        }
                    }
                    else
                    {
                        if (WaveTimer <= Wave)
                        {
                            Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(WaveIntensity));
                            Projectile.velocity = perturbedSpeed;
                        }
                        else
                        {
                            Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
                            Projectile.velocity = perturbedSpeed;
                        }
                        if (WaveTimer >= Wave * 2)
                        {
                            WaveTimer = 0;
                        }
                    }

                    break;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.ai[2] == 2 || Projectile.ai[2] == 3)
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                    Projectile.velocity.X = -oldVelocity.X * 0.8f;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                    Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
                }
            }

			return false;
		}

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 250;

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
    }
}