using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Minibiomes.Ocean
{
	public class SkeletonGar : ModNPC
	{
        private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 8;
			Main.npcCatchable[NPC.type] = true;
			NPCID.Sets.CountsAsCritter[Type] = true;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 200;
			NPC.damage = 0;
			NPC.defense = 10;
			NPC.width = 132;
			NPC.height = 40;
			NPC.npcSlots = 0.5f;
			NPC.knockBackResist = 0.35f;
			NPC.noGravity = true;
            NPC.dontTakeDamageFromHostiles = false;
			NPC.value = Item.buyPrice(0, 0, 0, 25);
			NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.DD2_SkeletonHurt;
            NPC.aiStyle = 16;
			AIType = NPCID.Goldfish;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			//draw aura
			Vector2 drawOrigin = new(NPCTexture.Width() * 0.5f, NPC.height * 0.5f);

            var effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            //draw aura
            for (int i = 0; i < 360; i += 30)
            {
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lime);

                Vector2 circular = new Vector2(2.5f, 0).RotatedBy(MathHelper.ToRadians(i));

                spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            return false;
		}

		public override void FindFrame(int frameHeight)
		{
            NPC.frameCounter++;
			if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

		public override void AI()
		{
			NPC.spriteDirection = -NPC.direction;

            NPC.rotation = NPC.velocity.Y * (NPC.direction == -1 ? 0.05f : -0.05f);

			int BigDunk = NPC.FindFirstNPC(ModContent.NPCType<Dunkleosteus>());
			if (BigDunk >= 0)
            {
                bool DunkLineOfSight = Collision.CanHitLine(Main.npc[BigDunk].position, Main.npc[BigDunk].width, Main.npc[BigDunk].height, NPC.position, NPC.width, NPC.height);
                if (NPC.Distance(Main.npc[BigDunk].Center) <= 150f && DunkLineOfSight && NPC.wet)
                {
                    NPC.localAI[0] = 60;
                }
            }

            if (NPC.localAI[0] > 0)
            {
                NPC.localAI[0]--;

                Vector2 vel = NPC.DirectionFrom(Main.npc[BigDunk].Center);
                vel.Normalize();
                vel *= 6f;
                NPC.velocity = vel;
                if (Main.npc[BigDunk].position.X > NPC.position.X)
                {
                    NPC.spriteDirection = -1;
                    NPC.direction = -1;
                    NPC.netUpdate = true;
                }
                else if (Main.npc[BigDunk].position.X < NPC.position.X)
                {
                    NPC.spriteDirection = 1;
                    NPC.direction = 1;
                    NPC.netUpdate = true;
                }
            }
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SkeletonGarGore" + numGores).Type);
                    }
                }
            }
        }
	}
}