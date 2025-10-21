using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.SpiderCave.SporeEvent
{
    public class EyelashMitePurpleBody1 : ModNPC
    {
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 500;
            NPC.damage = 45;
            NPC.defense = 10;
            NPC.width = 30;
            NPC.height = 20;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit22 with { Pitch = -0.5f };
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Rectangle frame = new Rectangle(0, NPC.frame.Y, NPCTexture.Width(), NPCTexture.Height() / Main.npcFrameCount[NPC.type]);
            Vector2 origin = new Vector2(NPCTexture.Width() * 0.5f, NPCTexture.Height() * 0.5f);
            Main.spriteBatch.Draw(NPCTexture.Value, NPC.Center - Main.screenPosition, frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0);

            return false;
        }

        public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            NPC.spriteDirection = Parent.spriteDirection;

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.NPCType<EyelashMitePurpleHead>())
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyelashMitePurpleBodyGore1").Type);
                }

                NPC.active = false;
            }

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - NPC.Center;

			if (SegmentParent.rotation != NPC.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - NPC.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.2f);
			}

			NPC.rotation = SegmentCenter.ToRotation() + 1.57f;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
				NPC.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * 25f;
			}

			return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }
    }

    public class EyelashMitePurpleBody2 : EyelashMitePurpleBody1
    {
        private static Asset<Texture2D> NPCTexture;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Rectangle frame = new Rectangle(0, NPC.frame.Y, NPCTexture.Width(), NPCTexture.Height() / Main.npcFrameCount[NPC.type]);
            Vector2 origin = new Vector2(NPCTexture.Width() * 0.5f, NPCTexture.Height() * 0.5f);
            Main.spriteBatch.Draw(NPCTexture.Value, NPC.Center - Main.screenPosition, frame, drawColor, NPC.rotation, origin, NPC.scale, effects, 0);

            return false;
        }

        public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            NPC.spriteDirection = Parent.spriteDirection;

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.NPCType<EyelashMitePurpleHead>())
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyelashMitePurpleBodyGore2").Type);
                }

                NPC.active = false;
            }

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - NPC.Center;

			if (SegmentParent.rotation != NPC.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - NPC.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.2f);
			}

			NPC.rotation = SegmentCenter.ToRotation() + 1.57f;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
				NPC.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * 15f;
			}

			return false;
        }
    }
}
