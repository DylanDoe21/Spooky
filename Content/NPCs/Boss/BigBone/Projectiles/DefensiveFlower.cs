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
	public class DefensiveFlower : ModNPC
	{
		public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 1450;
            NPC.damage = 0;
            NPC.defense = 35;
            NPC.width = 30;
            NPC.height = 30;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
			NPC.alpha = 255;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			//draw flower chain connected to big bone
			for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<BigBone>()) 
				{
					Vector2 ParentCenter = Main.npc[k].Center;

					Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/BigFlowerChain");

					Rectangle? chainSourceRectangle = null;
					float chainHeightAdjustment = 0f;

					Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
					Vector2 chainDrawPosition = NPC.Center;
					Vector2 vectorFromProjectileToPlayerArms = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
					Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
					float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;

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

						var chainTextureToDraw = chainTexture;

						Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

						chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
						chainCount++;
						chainLengthRemainingToDraw -= chainSegmentLength;
					}
				}
			}

			//draw glowy effect
			Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/DefensiveFlowerGlow").Value;

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            Color color = Color.Lerp(Color.DarkOrange, Color.Transparent, fade);

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), null, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale + fade / 5, SpriteEffects.None, 0);

			//draw flower on top so it doesnt look weird
			Texture2D flowerTex = ModContent.Request<Texture2D>(Texture).Value;

            Main.EntitySpriteDraw(flowerTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), null, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.3f + fade / 5, SpriteEffects.None, 0);

            return true;
        }

		public override void AI()
		{
			Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

			for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<BigBone>()) 
				{
					Main.npc[k].immortal = true;
					Main.npc[k].dontTakeDamage = true;
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
					Main.dust[dustGore].color = Color.OrangeRed;
					Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-2f, 2f);
					Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-2f, 2f);
					Main.dust[dustGore].noGravity = true;
				}
			}
		}
	}
}