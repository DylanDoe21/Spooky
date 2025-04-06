using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    public class BoroTail : OrroTail
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/Orroboro/OrroTail";

        private static Asset<Texture2D> NPCTexture;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			if (!NPC.AnyNPCs(ModContent.NPCType<OrroHead>()))
			{
				for (int i = 0; i < 360; i += 30)
				{
                    Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Red);

					Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));
					spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color * 0.2f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.2f, SpriteEffects.None, 0);
				}
			}

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}
	}

    public class BoroTailP1 : OrroTail
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/Orroboro/OrroTail";

		private static Asset<Texture2D> NPCTexture;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}

		public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

			//kill segment if the head doesnt exist
			if (!Parent.active || (Parent.type != ModContent.NPCType<OrroHeadP1>() && Parent.type != ModContent.NPCType<OrroHead>() && Parent.type != ModContent.NPCType<BoroHead>()))
            {
				NPC.active = false;
			}

            //go invulnerable and shake during phase 2 transition
            if (NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()))
            {
                if (Main.npc[(int)NPC.ai[1]].ai[2] > 0)
                {
                    NPC.immortal = true;
                    NPC.dontTakeDamage = true;
                    NPC.netUpdate = true;
                    NPC.velocity *= 0f;

                    NPC.ai[2]++;

                    NPC.Center = new Vector2(NPC.Center.X, NPC.Center.Y);
                    NPC.Center += Main.rand.NextVector2Square(-2, 2);
                }
            }

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 destinationOffset = SegmentParent.Center + SegmentParent.velocity - NPC.Center;

			if (SegmentParent.rotation != NPC.rotation)
			{
				float angle = MathHelper.WrapAngle(SegmentParent.rotation - NPC.rotation);
				destinationOffset = destinationOffset.RotatedBy(angle * 0.1f);
			}

			NPC.rotation = destinationOffset.ToRotation() + 1.57f;

			//how far each segment should be from each other
			if (destinationOffset != Vector2.Zero)
			{
				NPC.Center = SegmentParent.Center - destinationOffset.SafeNormalize(Vector2.Zero) * 30f;
			}

			return false;
        }
    }
}