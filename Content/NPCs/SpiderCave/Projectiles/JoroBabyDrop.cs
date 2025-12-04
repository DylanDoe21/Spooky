using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.NPCs.SpiderCave.SpiderWar;

namespace Spooky.Content.NPCs.SpiderCave.Projectiles
{
    public class JoroBabyDrop : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }
		
        public override void SetDefaults()
        {
            Projectile.width = 74;
            Projectile.height = 44;
			Projectile.friendly = false;
            Projectile.hostile = true;                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;             					
            Projectile.timeLeft = 400;
		}
        
        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Vector2 RealDrawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(ProjTexture.Value, RealDrawPos, rectangle, lightColor, Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0);

            return false;
        }

        public override bool? CanDamage()
		{
			return false;
		}

        public override void AI()
        {
            float RotateDirection = Projectile.velocity.ToRotation();
            float RotateSpeed = 0.05f;

            Projectile.rotation = Projectile.rotation.AngleTowards(RotateDirection - (Projectile.ai[1] == 1 ? (float)Math.PI : 0), RotateSpeed);

            Projectile.frame = (int)Projectile.ai[1];

            Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;

            if (!IsColliding())
            {
                Projectile.ai[0]++;
            }
            if (Projectile.ai[0] > 5)
            {
                Projectile.tileCollide = true;

                if (IsColliding())
                {
                    Projectile.Kill();
                }
            }
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
                    if (Main.tile[i, j] != null && (Main.tile[i, j].HasTile && (Main.tileSolid[(int)Main.tile[i, j].TileType])))
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
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundMiss with { Pitch = -1.5f }, Projectile.Center);

            Screenshake.ShakeScreenWithIntensity(Projectile.Center, 18f, 600f);
            
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int NewNPC = NPC.NewNPC(Projectile.GetSource_FromAI(), (int)Projectile.Center.X, (int)Projectile.Center.Y, ModContent.NPCType<JoroBaby>());

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
                }
            }
		}
    }
}
     
          






