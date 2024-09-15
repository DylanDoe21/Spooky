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
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Quest
{
	public class FrankenGoblinEye : ModNPC
	{
        public override string Texture => "Spooky/Content/NPCs/SpookyBiome/MonsterEye2";

        bool HasJumped = false;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 150;
            NPC.damage = 40;
			NPC.defense = 5;
			NPC.width = 42;
			NPC.height = 42;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 1f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 66;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			float stretch = NPC.velocity.Y * 0.025f;

			stretch = Math.Abs(stretch);

			//limit how much it can stretch
			if (stretch > 0.5f)
			{
				stretch = 0.5f;
			}

			//limit how much it can squish
			if (stretch < -0.5f)
			{
				stretch = -0.5f;
			}

			Vector2 scaleStretch = new Vector2(1f + stretch, 1f - stretch);
			
			if (NPC.velocity.Y <= 0)
			{
				scaleStretch = new Vector2(1f - stretch, 1f + stretch);
			}
			if (NPC.velocity.Y > 0)
			{
				scaleStretch = new Vector2(1f + stretch, 1f - stretch);
			}

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, scaleStretch, effects, 0);

			return false;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            //face towards the player
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            NPC.spriteDirection = NPC.direction;

            JumpToTarget(player, 500);
        }

        public void JumpToTarget(Player target, int JumpHeight)
        {
            NPC.ai[0]++;

            //set where the it should be jumping towards
            Vector2 JumpTo = new Vector2(target.Center.X + (target.Center.X < NPC.Center.X ? -150 : 150), NPC.Center.Y - JumpHeight);

            //set velocity and speed
            Vector2 velocity = JumpTo - NPC.Center;
            velocity.Normalize();

            float speed = MathHelper.Clamp(velocity.Length() / 36, 20, 35);

            NPC.velocity.X *= NPC.velocity.Y <= 0 ? 0.98f : 0.95f;

            //actual jumping
            NPC.ai[1]++;

            if (NPC.velocity == Vector2.Zero && !HasJumped)
            {
                if (NPC.ai[1] == 10)
                {
                    SoundEngine.PlaySound(SoundID.GlommerBounce, NPC.Center);
                    
                    velocity.Y -= 0.3f;
                    
                    HasJumped = true;
                }
            }
            
            if (NPC.ai[1] < 20 && HasJumped)
            {
                NPC.velocity = velocity * speed;
            }

            //loop ai
            if (NPC.ai[0] >= 100)
            {
                HasJumped = false;

                NPC.ai[0] = 0;
                NPC.ai[1] = 0;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 12; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, (NPC.velocity * 0.5f) + new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, -1)), ModContent.Find<ModGore>("Spooky/MonsterEyeChunk").Type);
                    }
                }
            }
        }
	}
}