using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Spooky.Content.NPCs.Boss.Orroboro.Projectiles
{
    public class BoroTongue : ModNPC
	{
        private static Asset<Texture2D> ChainTexture;
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 15000;
            NPC.damage = 65;
            NPC.defense = 30;
            NPC.width = 65;
            NPC.height = 65;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.behindTiles = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<BoroHead>())
            {
                ProjTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Orroboro/Projectiles/BoroTongue");
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

            if (NPC.ai[0] <= 25)
            {
                NPC.direction = NPC.spriteDirection = NPC.velocity.X > 0f ? 1 : -1;
                NPC.rotation = NPC.velocity.ToRotation();

                if (NPC.spriteDirection == -1)
                {
                    NPC.rotation += MathHelper.Pi;
                }
            }

            //die if the parent npc is dead
            if (!Parent.active || Parent.type != ModContent.NPCType<BoroHead>())
            {
                NPC.active = false;
            }

            NPC.ai[0]++;

            if (NPC.ai[0] == 1)
            {
                Vector2 ChargeSpeed = new Vector2(NPC.ai[1], NPC.ai[2]) - NPC.Center;
                ChargeSpeed.Normalize();
                ChargeSpeed *= 40;
                NPC.velocity = ChargeSpeed;
            }

            if (NPC.ai[0] == 25)
            {
                NPC.localAI[0] = NPC.rotation;
                NPC.localAI[1] = NPC.direction;
            }

            if (NPC.ai[0] > 25)
            {
                NPC.rotation = NPC.localAI[0];
                NPC.direction = (int)NPC.localAI[1];

                Vector2 ChargeSpeed = Parent.Center - NPC.Center;
                ChargeSpeed.Normalize();
                ChargeSpeed *= 60;
                NPC.velocity = ChargeSpeed;

                if (NPC.Distance(Parent.Center) <= 30f)
                {
                    NPC.active = false;
                }
            }
		}
	}
}
     
          






