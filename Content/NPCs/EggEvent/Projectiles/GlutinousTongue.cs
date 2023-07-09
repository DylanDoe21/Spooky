using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{ 
    public class GlutinousTongue : ModNPC
    {
        public override void SetStaticDefaults()
		{
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 850;
            NPC.damage = 40;
            NPC.defense = 20;
            NPC.width = 54;
			NPC.height = 114;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[2]];

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<Glutinous>())
            {
                Vector2 ParentCenter = Parent.Center;

                Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilArm");

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
                Vector2 chainDrawPosition = new Vector2(NPC.Center.X, NPC.Center.Y - 10);
                Vector2 vectorFromProjectileToPlayerArms = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    var chainTextureToDraw = chainTexture;

                    Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }

            return true;
        }

		public override void AI()
        {
            NPC.direction = NPC.spriteDirection = NPC.velocity.X > 0f ? -1 : 1;
            NPC.rotation = NPC.velocity.ToRotation();
        }
    }
}