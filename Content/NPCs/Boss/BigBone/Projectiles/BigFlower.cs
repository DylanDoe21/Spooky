using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
	public class BigFlower : ModNPC
	{
		private static Asset<Texture2D> ChainTexture;
        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle MagicCastSound = new("Spooky/Content/Sounds/BigBone/BigBoneMagic2", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 5200;
            NPC.damage = 0;
            NPC.defense = 25;
            NPC.width = 62;
            NPC.height = 60;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
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

            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/BigFlowerGlow");

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

			Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.DarkOrange);

			for (int numEffect = 0; numEffect < 5; numEffect++)
			{
				Color newColor = color;
				newColor = NPC.GetAlpha(newColor);
				newColor *= 1f - fade;
				Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4);
				Main.EntitySpriteDraw(GlowTexture.Value, vector, NPC.frame, newColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.05f + fade / 3, SpriteEffects.None, 0);
			}

            return true;
        }

		public override void AI()
		{
			NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];

			NPC Parent = Main.npc[(int)NPC.ai[0]];

			int Damage = Main.masterMode ? 90 / 3 : Main.expertMode ? 70 / 2 : 50;

			Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			if (Parent.active && Parent.type == ModContent.NPCType<BigBone>()) 
			{
				NPC.ai[1] = Parent.localAI[2];
			}

			if (NPC.ai[1] == 350)
			{
				SoundEngine.PlaySound(MagicCastSound, NPC.Center);
			}

			if (NPC.ai[1] >= 350 && NPC.ai[1] <= 370)
			{
				int MaxDusts = Main.rand.Next(10, 20);
				for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
				{
					Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * 1.5f).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
					Vector2 velocity = dustPos - NPC.Center;
					int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, DustID.OrangeTorch, velocity.X * 2f, velocity.Y * 2f, 100, default, 1.4f);
					Main.dust[dustEffect].noGravity = true;
					Main.dust[dustEffect].noLight = false;
					Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-5f, -2f);
					Main.dust[dustEffect].fadeIn = 1.3f;
				}
			}

			if (NPC.ai[1] == 370)
			{
				SoundEngine.PlaySound(SoundID.Item42, NPC.Center);

				Vector2 ShootSpeed = player.Center - NPC.Center;
				ShootSpeed.Normalize();
						
				ShootSpeed.X *= 15;
				ShootSpeed.Y *= 15;

				Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
				Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

				if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				{
					position += muzzleOffset;
				}

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), position.X, position.Y, ShootSpeed.X, 
					ShootSpeed.Y, ModContent.ProjectileType<MassiveFlameBallBolt>(), Damage, 1, Main.myPlayer, 0, 0);
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
		
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
			var parameters = new DropOneByOne.Parameters() 
			{
				ChanceNumerator = 1,
				ChanceDenominator = 1,
				MinimumStackPerChunkBase = 1,
				MaximumStackPerChunkBase = 1,
				MinimumItemDropsCount = 2,
				MaximumItemDropsCount = 3,
			};

			npcLoot.Add(new DropOneByOne(ItemID.Heart, parameters));
        }
	}
}