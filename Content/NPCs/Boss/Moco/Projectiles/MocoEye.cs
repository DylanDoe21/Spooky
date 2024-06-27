using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.Moco.Projectiles
{
	public class MocoEye : ModProjectile
	{
        Vector2 ChainStartPosition = Vector2.Zero;

        private static Asset<Texture2D> ChainTexture;
        private static Asset<Texture2D> ProjTexture;

        public static readonly SoundStyle EyePopSound = new("Spooky/Content/Sounds/Moco/MocoEyePop", SoundType.Sound) { PitchVariance = 0.75f };

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
			Projectile.friendly = false;
            Projectile.hostile = true;                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;             					
            Projectile.timeLeft = 400;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            NPC Parent = Main.npc[(int)Projectile.ai[1]];

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<Moco>() && ChainStartPosition != Vector2.Zero)
            {
                ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Moco/Projectiles/MocoEyeChain");
                
                Vector2 ChainDrawOrigin = ChainStartPosition;

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
                Vector2 chainDrawPosition = Projectile.Center;
                Vector2 vectorToParent = ChainDrawOrigin.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorToParent * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }

            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Vector2 RealDrawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Gray) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(ProjTexture.Value, RealDrawPos, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0);

            return false;
        }

        public override void AI()
		{
            NPC Parent = Main.npc[(int)Projectile.ai[1]];

            Player player = Main.player[Parent.target];

            //die if the parent npc is dead
            if (!Parent.active || Parent.type != ModContent.NPCType<Moco>())
            {
                Projectile.active = false;
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 1)
            {
                ChainStartPosition = Projectile.Center;
            }

            if (Projectile.ai[0] > 15)
            {
                Vector2 ChargeSpeed = ChainStartPosition - Projectile.Center;
                ChargeSpeed.Normalize();
                ChargeSpeed *= 45;
                Projectile.velocity = ChargeSpeed;

                if (Projectile.Hitbox.Intersects(Parent.Hitbox))
                {
                    SoundEngine.PlaySound(EyePopSound, Projectile.Center);

                    Parent.localAI[0] = 249;
 
                    Projectile.active = false;
                }
            }
		}
	}
}