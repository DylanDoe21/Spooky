using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class SmilingDog : ModProjectile
    {
        bool isAttacking = false;

        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
			Projectile.height = 20;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override bool? CanDamage()
		{
            return isAttacking;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override void PostDraw(Color lightColor)
        {
            if (Projectile.ai[1] >= 10800)
            {
                GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Cemetery/SmilingDogGlow");

                Vector2 drawOrigin = new(GlowTexture.Width() * 0.5f, Projectile.height * 0.5f);

                var spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
                {
                    float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                    Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(Color.Red) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                    Rectangle rectangle = new(0, (GlowTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, GlowTexture.Width(), GlowTexture.Height() / Main.projFrames[Projectile.type]);
                    Main.EntitySpriteDraw(GlowTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, spriteEffects, 0);
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.dead)
            {
                player.GetModPlayer<SpookyPlayer>().SmileDogPicture = false;
            }

            if (player.GetModPlayer<SpookyPlayer>().SmileDogPicture)
            {
                Projectile.timeLeft = 2;
            }

            //timer for when it should start attacking you
            //dont increase at all if the player is wearing the full combined accessory
            if (!player.GetModPlayer<SpookyPlayer>().CreepyPasta)
            {
                Projectile.ai[1]++;
            }

            if (Projectile.ai[1] < 10800)
            {
                Projectile.friendly = true;
                Projectile.hostile = false;

                //target an enemy
                for (int i = 0; i < 200; i++)
                {
                    //first, if the player gets too far away while an enemy is being targetted then have the minion stop attacking and return to the player
                    if (Vector2.Distance(player.Center, Projectile.Center) >= 550f)
                    {
                        isAttacking = false;
                        IdleAI(player);

                        break;
                    }

                    NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                    if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 400f)
                    {
                        AttackingAI(Target);

                        break;
                    }
                    else
                    {
                        isAttacking = false;
                    }

                    NPC NPC = Main.npc[i];
                    if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 400f)
                    {
                        AttackingAI(NPC);

                        break;
                    }
                    else
                    {
                        isAttacking = false;
                    }
                }

                if (!isAttacking)
                {
                    IdleAI(player);
                }
            }
            else
            {
                Projectile.friendly = false;
                Projectile.hostile = true;

                AttackPlayerAI(player);

                if (Projectile.ai[1] >= 11400)
                {
                    Projectile.ai[1] = 0;
                }
            }
        }

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            Vector2 center2 = Projectile.Center;
            Vector2 vector48 = target.Center - center2;
            float targetDistance = vector48.Length();

            if (Projectile.velocity.Y == 0 && (HoleBelow() || (targetDistance <= 50f && Projectile.position.X == Projectile.oldPosition.X)))
            {
                Projectile.velocity.Y = -9f;
            }

            //jump if the target is too high
            if (Projectile.velocity.Y == 0 && target.Center.Y < Projectile.Center.Y - 50)
            {
                Projectile.velocity.Y = Main.rand.Next(-12, -6);
            }

            Projectile.velocity.Y += 0.35f;

            if (Projectile.velocity.Y > 15f)
            {
                Projectile.velocity.Y = 15f;
            }

            if (target.position.X - Projectile.position.X > 0f)
            {
                Projectile.velocity.X += 0.22f;
                if (Projectile.velocity.X > 6f)
                {
                    Projectile.velocity.X = 6f;
                }
            }
            else
            {
                Projectile.velocity.X -= 0.22f;
                if (Projectile.velocity.X < -6f)
                {
                    Projectile.velocity.X = -6f;
                }
            }

            //falling frame
            if (Projectile.velocity.Y > 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
            {
                Projectile.frame = 2;
            }
            //moving animation
            else if (Projectile.velocity.X != 0)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 2)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= 5)
                {
                    Projectile.frame = 1;
                }
            }

            if (Projectile.velocity.X > 0.8f)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.velocity.X < -0.8f)
            {
                Projectile.spriteDirection = 1;
            }
        }

        public void IdleAI(Player player)
        {
            Vector2 center2 = Projectile.Center;
            Vector2 vector48 = player.Center - center2;
            float playerDistance = vector48.Length();

            if (Projectile.velocity.Y == 0 && ((HoleBelow() && playerDistance > 100f) || (playerDistance > 100f && Projectile.position.X == Projectile.oldPosition.X)))
            {
                Projectile.velocity.Y = -10f;
            }

            Projectile.velocity.Y += 0.35f;

            if (Projectile.velocity.Y > 15f)
            {
                Projectile.velocity.Y = 15f;
            }

            if (playerDistance > 900f)
            {
                Projectile.position = player.Center - (Projectile.Size / 2);
            }

            //go to the player if too far away
            if (playerDistance > 90f)
            {
                if (player.position.X - Projectile.position.X > 0f)
                {
                    Projectile.velocity.X += 0.12f;
                    if (Projectile.velocity.X > 6f)
                    {
                        Projectile.velocity.X = 6f;
                    }
                }
                else
                {
                    Projectile.velocity.X -= 0.12f;
                    if (Projectile.velocity.X < -6f)
                    {
                        Projectile.velocity.X = -6f;
                    }
                }
            }

            //slow down when nearby the player
            if (playerDistance < 80f)
            {
                if (Projectile.velocity.X != 0f)
                {
                    if (Projectile.velocity.X > 0.8f)
                    {
                        Projectile.velocity.X -= 0.5f;
                    }
                    else if (Projectile.velocity.X < -0.8f)
                    {
                        Projectile.velocity.X += 0.5f;
                    }
                    else if (Projectile.velocity.X < 0.8f && Projectile.velocity.X > -0.8f)
                    {
                        Projectile.velocity.X = 0f;
                    }
                }
            }

            //set frames when idle
            if (Projectile.position.X == Projectile.oldPosition.X && Projectile.position.Y == Projectile.oldPosition.Y && Projectile.velocity.X == 0)
            {
                Projectile.frame = 0;
            }
            //falling frame
            else if (Projectile.velocity.Y > 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
            {
                Projectile.frame = 2;
            }
            //moving animation
            else if (Projectile.velocity.X != 0)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 2)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= 5)
                {
                    Projectile.frame = 1;
                }
            }

            if (Projectile.velocity.X > 0.8f)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.velocity.X < -0.8f)
            {
                Projectile.spriteDirection = 1;
            }
        }

        public void AttackPlayerAI(Player player)
        {
            isAttacking = true;

            Vector2 center2 = Projectile.Center;
            Vector2 vector48 = player.Center - center2;
            float playerDistance = vector48.Length();

            if (Projectile.velocity.Y == 0 && ((HoleBelow() && playerDistance > 5f) || (playerDistance > 5f && Projectile.position.X == Projectile.oldPosition.X)))
            {
                Projectile.velocity.Y = -10f;
            }

            Projectile.velocity.Y += 0.4f;

            if (Projectile.velocity.Y > 15f)
            {
                Projectile.velocity.Y = 15f;
            }

            if (player.position.X - Projectile.position.X > 0f)
            {
                Projectile.velocity.X += 0.2f;
                if (Projectile.velocity.X > 6f)
                {
                    Projectile.velocity.X = 6f;
                }
            }
            else
            {
                Projectile.velocity.X -= 0.2f;
                if (Projectile.velocity.X < -6f)
                {
                    Projectile.velocity.X = -6f;
                }
            }

            //falling frame
            if (Projectile.velocity.Y > 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
            {
                Projectile.frame = 2;
            }
            //moving animation
            else if (Projectile.velocity.X != 0)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 2)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= 5)
                {
                    Projectile.frame = 1;
                }
            }

            if (Projectile.velocity.X > 0.8f)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.velocity.X < -0.8f)
            {
                Projectile.spriteDirection = 1;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 center2 = Projectile.Center;
            Vector2 vector48 = player.Center - center2;
            float playerDistance = vector48.Length();
            fallThrough = playerDistance > 200f;

            return true;
        }

        private bool HoleBelow()
        {
            int tileWidth = 4;
            int tileX = (int)(Projectile.Center.X / 16f) - tileWidth;
            if (Projectile.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((Projectile.position.Y + Projectile.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile && (Main.tile[x - 1, y].HasTile || Main.tile[x + 1, y].HasTile))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}