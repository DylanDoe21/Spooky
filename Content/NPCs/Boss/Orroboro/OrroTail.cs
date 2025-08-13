using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    public class OrroTail : ModNPC
    {
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };

            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 15000;
            NPC.damage = 55;
            NPC.defense = 30;
            NPC.width = 30;
            NPC.height = 30;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.aiStyle = -1;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}

		public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            NPC.alpha = Parent.alpha;

            //go invulnerable and shake during phase 2 transition
            if (Parent.type == ModContent.NPCType<OrroHeadP1>() && Parent.ai[0] == -2)
            {
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                NPC.netUpdate = true;
                NPC.velocity = Vector2.Zero;

                NPC.Center = new Vector2(NPC.Center.X, NPC.Center.Y);
                NPC.Center += Main.rand.NextVector2Square(-2, 2);
            }

            //kill segment if the head doesnt exist
			if (!Parent.active || (Parent.type != ModContent.NPCType<OrroHeadP1>() && Parent.type != ModContent.NPCType<OrroHead>() && Parent.type != ModContent.NPCType<BoroHead>()))
            {
                if (Parent.type != ModContent.NPCType<OrroHeadP1>())
                {
                    SpawnGores();
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
				NPC.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * 30f;
			}

			return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                SpawnGores();
            }
        }

        public void SpawnGores()
        {
            if (Main.netMode != NetmodeID.Server) 
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TailGore").Type);
            }
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