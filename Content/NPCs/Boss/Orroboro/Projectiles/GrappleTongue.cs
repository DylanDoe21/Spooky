using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.Orroboro.Projectiles
{
    public class GrappleTongue : ModNPC
	{
        private static Asset<Texture2D> ChainTexture;
        private static Asset<Texture2D> ProjTexture;

		public static readonly SoundStyle LickSound = new("Spooky/Content/Sounds/Orroboro/BoroLick", SoundType.Sound) { PitchVariance = 0.6f };

		public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 18;
            NPC.height = 18;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            //only draw if the parent is active
            if (Parent.active && (Parent.type == ModContent.NPCType<OrroHeadP1>() || Parent.type == ModContent.NPCType<OrroHead>() || Parent.type == ModContent.NPCType<BoroHead>()))
            {
                ProjTexture ??= ModContent.Request<Texture2D>(Texture);
                ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Orroboro/Projectiles/BoroTongueSegment");
                
                Vector2 ParentCenter = Parent.Center;

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
                Vector2 chainDrawPosition = new Vector2(NPC.Center.X, NPC.Center.Y );
                Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
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

                    Main.spriteBatch.Draw(chainCount == 0 ? ProjTexture.Value : ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorToParent * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }

            return false;
        }

        public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[3]];

			float RotateX = Parent.Center.X - NPC.Center.X;
			float RotateY = Parent.Center.Y - NPC.Center.Y;
			NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			//die if the parent npc is dead
			if (!Parent.active || (Parent.type != ModContent.NPCType<OrroHeadP1>() && Parent.type != ModContent.NPCType<OrroHead>() && Parent.type != ModContent.NPCType<BoroHead>()))
            {
                NPC.active = false;
            }
			if (NPC.Center.Y > Main.player[Parent.target].Center.Y + 10)
			{
				if (NPCGlobalHelper.IsColliding(NPC) && NPC.localAI[0] == 0)
				{
					NPC.velocity = Vector2.Zero;
                    NPC.localAI[0]++;
				}
			}

			if (NPC.localAI[0] > 0 && NPC.Distance(Parent.Center) < 30)
			{
				NPC.active = false;
			}

            if (NPC.ai[0] == 0)
            {
				SoundEngine.PlaySound(LickSound, NPC.Center);

                Vector2 ChargeSpeed = new Vector2(NPC.ai[1], NPC.ai[2]) - NPC.Center;
                ChargeSpeed.Normalize();
                ChargeSpeed *= 45;
                NPC.velocity = ChargeSpeed;

				NPC.ai[0]++;
            }
		}
	}
}
     
          






