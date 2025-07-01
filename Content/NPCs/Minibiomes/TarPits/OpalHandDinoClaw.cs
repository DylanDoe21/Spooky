using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Minibiomes.TarPits
{
	public class OpalHandDinoClaw : ModNPC
	{
		private static Asset<Texture2D> ChainTexture;
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 150;
            NPC.damage = 55;
            NPC.defense = 20;
            NPC.width = 18;
            NPC.height = 18;
            NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontCountMe = true;
			NPC.hide = true;
            NPC.HitSound = SoundID.NPCHit2;
            NPC.DeathSound = SoundID.NPCHit2;
            NPC.aiStyle = -1;
        }

		public override void DrawBehind(int index)
		{
			if (NPC.ai[1] == 0)
			{
				Main.instance.DrawCacheNPCProjectiles.Add(index);
			}
			else
			{
				Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
			}
		}

        public void DrawArms(bool SpawnGore)
		{
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/TarPits/OpalHandDinoSegment");

            //only draw if the parent is active
            if (Parent.active && Parent.type == ModContent.NPCType<OpalHandDino>() && !SpawnGore)
            {
				var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

				Vector2 ParentCenter = Parent.Center + new Vector2(Parent.direction == -1 ? -3 : 3, 10);

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
                Vector2 chainDrawPosition = new Vector2(NPC.Center.X, NPC.Center.Y);
                Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 8;
                }

                float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, effects, 0f);

                    chainDrawPosition += unitVectorToParent * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }

            if (SpawnGore)
            {
                Vector2 ParentCenter = Parent.Center + new Vector2(Parent.direction == -1 ? -3 : 3, 10);

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
                Vector2 chainDrawPosition = new Vector2(NPC.Center.X, NPC.Center.Y);
                Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 8;
                }

                float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), chainDrawPosition, Vector2.Zero, ModContent.Find<ModGore>("Spooky/OpalHandDinoSegmentGore").Type);
                    }

                    chainDrawPosition += unitVectorToParent * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawArms(false);

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
        }

        public override bool CheckDead()
        {
			NPC.ai[0] = 1;
			NPC.life = 1;
            return false;
        }

		public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[3]];
            Vector2 ParentCenter = Parent.Center + new Vector2(Parent.direction == -1 ? -3 : 3, 15);

            NPC.spriteDirection = NPC.direction = NPC.Center.X < Parent.Center.X ? -1 : 1;

            float RotateX = ParentCenter.X - NPC.Center.X;
            float RotateY = ParentCenter.Y - NPC.Center.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			//die if the parent npc is dead
			if (!Parent.active || Parent.type != ModContent.NPCType<OpalHandDino>())
			{
				if (Main.netMode != NetmodeID.Server)
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.Find<ModGore>("Spooky/OpalHandDinoClawGore").Type);
				}

				DrawArms(true);

				NPC.active = false;
			}
			else
			{
				if (NPC.ai[1] == 1)
				{
					Parent.localAI[0] = 470;
				}
			}

			if (NPCGlobalHelper.IsColliding(NPC))
			{
				NPC.velocity = Vector2.Zero;
			}

			//cause collision inbetween the hands and main parent so the arm blocks players from passing through
			Vector2 unit = new Vector2(1, 0).RotatedBy(NPC.rotation - MathHelper.PiOver2);
			float point = 0f;
			
            foreach (Player player in Main.ActivePlayers)
            {
                if (Collision.CheckAABBvLineCollision(player.Hitbox.TopLeft(), player.Hitbox.Size(), ParentCenter, ParentCenter + unit * ParentCenter.Distance(NPC.Center), 3, ref point))
                {
                    //get the point on the line where the player is and then push them away from that point as a workaround for collision
                    Vector2 PointOnLine = ParentCenter + unit * ParentCenter.Distance(player.Center);

                    Vector2 Pushback = player.Center - PointOnLine;
                    Pushback.Normalize();
                    Pushback *= 3;

                    player.velocity = Pushback;
                }
            }

			if (NPC.Distance(ParentCenter) > 600f)
			{
				NPC.ai[0] = 1;
			}

            //retract
            if (NPC.ai[0] == 1)
            {
                Vector2 ChargeSpeed = ParentCenter - NPC.Center;
                ChargeSpeed.Normalize();
                ChargeSpeed *= 20;
                NPC.velocity = ChargeSpeed;

                if (Parent.Distance(NPC.Center) <= 30f)
                {
					Parent.localAI[1]++;

                    if (NPC.ai[1] == 0)
                    {
                        Parent.localAI[2] = 1;
                    }
                    else
                    {
                        Parent.localAI[3] = 1;
                    }

                    NPC.active = false;
                }
            }
		}
	}
}