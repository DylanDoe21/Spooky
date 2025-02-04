using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
	public class HealingFlower : ModNPC
	{
		int HealingAmount = 0;

        private static Asset<Texture2D> ChainTexture;
        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 1300;
            NPC.damage = 0;
            NPC.defense = 35;
            NPC.width = 30;
            NPC.height = 30;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.hide = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			//draw flower chain connected to big bone
			if (Parent.active && Parent.type == ModContent.NPCType<BigBone>()) 
			{
                ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/BigFlowerChain");

                Vector2 ParentCenter = Parent.Center;

				Rectangle? chainSourceRectangle = null;
				float chainHeightAdjustment = 0f;

				Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
				Vector2 chainDrawPosition = NPC.Center;
				Vector2 vectorFromProjectileToPlayerArms = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
				Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
				float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

				if (chainSegmentLength == 0)
				{
					chainSegmentLength = 10;
				}

				float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
				int chainCount = 0;
				float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;
	
				while (chainLengthRemainingToDraw > 0f)
				{
					Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

					Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

					chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
					chainCount++;
					chainLengthRemainingToDraw -= chainSegmentLength;
				}
			}

            //draw glowy effect
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/HealingFlowerGlow");

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            Color color = Color.Lerp(Color.Red, Color.Transparent, fade);

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), null, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale + fade / 5, SpriteEffects.None, 0);

            //draw flower on top so it doesnt look weird
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), null, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.3f + fade / 5, SpriteEffects.None, 0);

            return false;
        }

		public override void DrawBehind(int index)
        {
            Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
        }

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			if (Parent.active && Parent.type == ModContent.NPCType<BigBone>()) 
			{
				NPC.ai[1]++;
				if (NPC.ai[1] >= 120 && Parent.life < Parent.lifeMax && HealingAmount > 0)
				{
					Parent.life += HealingAmount;
					Parent.HealEffect(HealingAmount, true);
					NPC.ai[1] = 0;
				}

				NPC.ai[2]++;
				if (NPC.ai[2] >= 360)
				{
					HealingAmount = Main.masterMode ? 45 : Main.expertMode ? 35 : 25;
				}
				if (NPC.ai[2] >= 720)
				{
					HealingAmount = Main.masterMode ? 55 : Main.expertMode ? 45 : 35;
				}
				if (NPC.ai[2] >= 1080)
				{
					HealingAmount = Main.masterMode ? 65 : Main.expertMode ? 55 : 45;
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
				for (int numDusts = 0; numDusts < 35; numDusts++)
				{                                                                                  
					int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.25f);
					Main.dust[dustGore].color = Color.Red;
					Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-2f, 2f);
					Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-2f, 2f);
					Main.dust[dustGore].noGravity = true;
				}
			}
		}
	}
}