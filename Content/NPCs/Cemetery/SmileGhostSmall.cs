using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Cemetery
{
    public class SmileGhostSmall1 : ModNPC
    {
		public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

		private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 4;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			//ints
			writer.Write(MoveSpeedX);
			writer.Write(MoveSpeedY);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//ints
			MoveSpeedX = reader.ReadInt32();
			MoveSpeedY = reader.ReadInt32();
		}

		public override void SetDefaults()
        {
            NPC.lifeMax = 40;
            NPC.damage = 20;
            NPC.defense = 0;
            NPC.width = 28;
			NPC.height = 38;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.4f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit54 with { Pitch = 1.2f };
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.aiStyle = -1;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			//draw aura
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			//draw aura
			for (int i = 0; i < 360; i += 90)
            {
				Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.White, Color.Gold, i / 30));

				Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2f), 0).RotatedBy(MathHelper.ToRadians(i));

				spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.075f, effects, 0f);
			}

			return true;
		}

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 5)
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
			if (NPC.ai[1] == 0)
			{
				int SmallerGhost = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-45, -20), (int)NPC.Center.Y, ModContent.NPCType<SmileGhostSmall2>(), ai0: NPC.whoAmI);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: SmallerGhost);
				}

				NPC.ai[1]++;
			}

			return true;
		}

		public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[0]];

			Player player = Main.player[Parent.target];

            NPC.rotation = Parent.velocity.X * 0.05f;

			if (!Parent.active || (Parent.type != ModContent.NPCType<SmileGhost>() && Parent.type != ModContent.NPCType<SmileGhostSmall1>() && Parent.type != ModContent.NPCType<SmileGhostSmall2>()))
			{
				player.ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0);
			}

			Vector2 destinationOffset = Parent.Center + Parent.velocity - NPC.Center;

			//how far each segment should be from each other
			if (destinationOffset != Vector2.Zero)
			{
				NPC.Center = Parent.Center - destinationOffset.SafeNormalize(Vector2.Zero) * 40f;
			}

			for (int num = 0; num < Main.maxNPCs; num++)
			{
				NPC other = Main.npc[num];

				bool IsPushable = other.type == ModContent.NPCType<SmileGhost>() || other.type == ModContent.NPCType<SmileGhostSmall1>() ||
				other.type == ModContent.NPCType<SmileGhostSmall2>() || other.type == ModContent.NPCType<SmileGhostSmall3>();

				if (num != NPC.whoAmI && IsPushable && other.active && Math.Abs(NPC.position.X - other.position.X) + Math.Abs(NPC.position.Y - other.position.Y) < NPC.width)
				{
					const float pushAway = 0.2f;
					if (NPC.position.X < other.position.X)
					{
						NPC.velocity.X -= pushAway;
					}
					else
					{
						NPC.velocity.X += pushAway;
					}
					if (NPC.position.Y < other.position.Y)
					{
						NPC.velocity.Y -= pushAway;
					}
					else
					{
						NPC.velocity.Y += pushAway;
					}
				}
			}
		}

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.15f);
                    Main.dust[dustGore].color = Color.Gold;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-2f, 2f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-2f, 2f);
                    Main.dust[dustGore].noGravity = true;
                }
            }
        }
    }

    public class SmileGhostSmall2 : SmileGhostSmall1
    {
        private static Asset<Texture2D> NPCTexture;

		public override void SetDefaults()
		{
			NPC.lifeMax = 40;
			NPC.damage = 25;
			NPC.defense = 0;
			NPC.width = 18;
			NPC.height = 30;
			NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.4f;
			NPC.noGravity = true;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit54 with { Pitch = 1.2f };
            NPC.DeathSound = SoundID.NPCDeath6;
			NPC.aiStyle = -1;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			//draw aura
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			//draw aura
			for (int i = 0; i < 360; i += 90)
            {
				Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.White, Color.Gold, i / 30));

				Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2f), 0).RotatedBy(MathHelper.ToRadians(i));

				spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, color * 0.75f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.05f, effects, 0f);
			}

			return true;
		}

		public override bool PreAI()
		{
			if (NPC.ai[1] == 0)
			{
				int SmallerGhost = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-45, -20), (int)NPC.Center.Y, ModContent.NPCType<SmileGhostSmall3>(), ai0: NPC.whoAmI);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: SmallerGhost);
				}

				NPC.ai[1]++;
			}

			return true;
		}
	}

    public class SmileGhostSmall3 : SmileGhostSmall1
    {
        private static Asset<Texture2D> NPCTexture;

		public override void SetDefaults()
		{
			NPC.lifeMax = 40;
			NPC.damage = 25;
			NPC.defense = 0;
			NPC.width = 14;
			NPC.height = 20;
			NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.4f;
			NPC.noGravity = true;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit54 with { Pitch = 1.2f };
            NPC.DeathSound = SoundID.NPCDeath6;
			NPC.aiStyle = -1;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			//draw aura
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			//draw aura
			for (int i = 0; i < 360; i += 90)
            {
				Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.White, Color.Gold, i / 30));

				Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2f), 0).RotatedBy(MathHelper.ToRadians(i));

				spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, color * 0.75f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.05f, effects, 0f);
			}

			return true;
		}

		public override bool PreAI()
		{
			return true;
		}
	}
}