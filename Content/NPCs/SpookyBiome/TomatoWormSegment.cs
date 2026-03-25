using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class TomatoWormBody1 : ModNPC
    {
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 2000;
            NPC.damage = 55;
            NPC.defense = 10;
            NPC.width = 42;
            NPC.height = 34;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.behindTiles = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 3)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.NPCType<TomatoWormHead>())
            {
                for (int numDusts = 0; numDusts <= 12; numDusts++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.DesertWater2, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-15f, -10f), 50, Color.White, 2.5f);
                    dust.velocity = Parent.velocity;
                    dust.noGravity = true;
                }

                NPC.active = false;
            }

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - NPC.Center;

			if (SegmentParent.rotation != NPC.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - NPC.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.1f);
			}

			NPC.rotation = SegmentCenter.ToRotation() + 1.57f;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
				NPC.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * (SegmentParent.type == ModContent.NPCType<TomatoWormHead>() ? 42f : 30f);
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

    public class TomatoWormBody2 : TomatoWormBody1
    {
    }

    public class TomatoWormTail : TomatoWormBody1
    {
        public override void SetDefaults()
        {
            NPC.lifeMax = 2000;
            NPC.damage = 55;
            NPC.defense = 10;
            NPC.width = 42;
            NPC.height = 34;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.behindTiles = true;
        }

        public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.NPCType<TomatoWormHead>())
            {
                for (int numDusts = 0; numDusts <= 12; numDusts++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.DesertWater2, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-15f, -10f), 50, Color.White, 2.5f);
                    dust.velocity = Parent.velocity;
                    dust.noGravity = true;
                }

                NPC.active = false;
            }

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - NPC.Center;

			if (SegmentParent.rotation != NPC.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - NPC.rotation);
				SegmentCenter = SegmentCenter.RotatedBy(angle * 0.1f);
			}

			NPC.rotation = SegmentCenter.ToRotation() + 1.57f;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
				NPC.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * 33f;
			}

			return false;
        }
    }
}
