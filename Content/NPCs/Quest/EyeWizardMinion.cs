using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Quest.Projectiles;

namespace Spooky.Content.NPCs.Quest
{
    public class EyeWizardMinion : ModNPC
    {
        public float SaveRotation;

        Vector2 GoToPosition;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> ChainTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(GoToPosition);

            //floats
            writer.Write(SaveRotation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
			GoToPosition = reader.ReadVector2();

            //floats
            SaveRotation = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 45;
            NPC.defense = 0;
            NPC.width = 16;
            NPC.height = 28;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.ai[1] >= 240)
            {
                ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

                Player player = Main.player[NPC.target];
                Vector2 TargetPosition = player.Center;

                Rectangle? chainSourceRectangle = null;

                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
                Vector2 chainDrawPosition = NPC.Center;
                Vector2 vectorFromNPCToPlayerArms = TargetPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorFromNPCToPlayerArms = vectorFromNPCToPlayerArms.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorFromNPCToPlayerArms.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = vectorFromNPCToPlayerArms.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                    float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f * (float)Math.Sin(chainLengthRemainingToDraw);

                    Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, Color.White, chainRotation, chainOrigin, NPC.scale * fade, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorFromNPCToPlayerArms * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }

            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawPosition = new Vector2(NPC.Center.X, NPC.Center.Y) - screenPos;

            Main.EntitySpriteDraw(NPCTexture.Value, drawPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0f);
            
            return false;
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[0]];
            
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            //kill this npc if the parent does not exist
			if (!Parent.active || Parent.type != ModContent.NPCType<EyeWizardBook>())
			{
                NPC.active = false;
			}

			float RotateX = player.Center.X - NPC.Center.X;
			float RotateY = player.Center.Y - NPC.Center.Y;
			NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
            
            NPC.ai[1]++;

            if (NPC.ai[1] == 2)
            {
                GoToPosition = new Vector2(Main.rand.Next(450, 600), 0).RotatedByRandom(360);
                NPC.netUpdate = true;
            }
 
            if (NPC.ai[1] > 2)
            {
                float vel = MathHelper.Clamp(NPC.Distance(player.Center + GoToPosition) / 12, 4, 7);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(player.Center + GoToPosition) * vel, 0.08f);
            }

            if (NPC.ai[1] == 300)
            {
                Vector2 ShootSpeed = player.Center - NPC.Center;
                ShootSpeed.Normalize();
                ShootSpeed *= 35f;

                NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<EyeMinionBeam>(), NPC.damage, 4.5f);

                NPC.ai[1] = 0;
            }
        }
    }
}