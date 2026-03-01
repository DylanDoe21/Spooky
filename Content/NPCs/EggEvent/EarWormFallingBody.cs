using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.EggEvent
{
    public class EarWormFallingBody : ModNPC
    {
        public override string Texture => "Spooky/Content/NPCs/EggEvent/EarWormSegment";

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 14;
            NPC.height = 14;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.behindTiles = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            Rectangle frame = new Rectangle(0, NPC.frame.Y, NPCTexture.Width(), NPCTexture.Height() / Main.npcFrameCount[NPC.type]);
            Vector2 origin = new Vector2(NPCTexture.Width() * 0.5f, NPCTexture.Height() * 0.5f);
            Main.spriteBatch.Draw(NPCTexture.Value, NPC.Center - Main.screenPosition, frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];
			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

            NPC.alpha = Parent.alpha;
        
            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.NPCType<EarWormFalling>())
            {
                NPC.active = false;
            }

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
				NPC.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * 8f;
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
}
