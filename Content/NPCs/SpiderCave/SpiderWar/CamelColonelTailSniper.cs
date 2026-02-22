using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Core;
using Spooky.Content.NPCs.SpiderCave.Projectiles;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
    public class CamelColonelTailSniper : ModNPC
    {
        float SaveRotation;

		bool SpawnedFirefly = false;

        Vector2 SavePlayerPosition;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> ChainTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SendExtraAI(BinaryWriter writer)
		{
            //vector2
            writer.WriteVector2(SavePlayerPosition);

			//bools
			writer.Write(SpawnedFirefly);

			//floats
			writer.Write(SaveRotation);
			writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
			writer.Write(NPC.localAI[2]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
            //vector2
            SavePlayerPosition = reader.ReadVector2();

			//bools
			SpawnedFirefly = reader.ReadBoolean();

			//floats
			SaveRotation = reader.ReadSingle();
			NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
			NPC.localAI[2] = reader.ReadSingle();
		}

        public override void SetDefaults()
        {
            NPC.lifeMax = 7000;
            NPC.damage = 70;
            NPC.defense = 18;
            NPC.width = 30;
            NPC.height = 30;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.noGravity = true;
			NPC.hide = true;
            NPC.HitSound = SoundID.NPCHit12 with { Pitch = 1f };
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPC Parent = Main.npc[(int)NPC.ai[3]];
            Player player = Main.player[Parent.target];

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            ChainTexture ??= ModContent.Request<Texture2D>(Texture + "Chain");

            bool AnotherMinibossPresent = SpiderWarWorld.EventActiveNPCCount() > 1;
            int Repeats = AnotherMinibossPresent ? 3 : 4;
            if (Parent.ai[0] == 1 && Parent.localAI[0] > 42 && Parent.localAI[0] <= 80 && Parent.localAI[1] < Repeats)
            {     
                Vector2 DrawToCenter = SavePlayerPosition;

                Rectangle? chainSourceRectangle = null;
                float chainHeightAdjustment = 0f;

                Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
                Vector2 chainDrawPosition = NPC.Center;
                Vector2 VectorToPlayer = DrawToCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
                Vector2 unitVectorToPlayer = VectorToPlayer.SafeNormalize(Vector2.Zero);
                float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

                if (chainSegmentLength == 0)
                {
                    chainSegmentLength = 10;
                }

                float chainRotation = unitVectorToPlayer.ToRotation() + MathHelper.PiOver2;
                int chainCount = 0;
                float chainLengthRemainingToDraw = VectorToPlayer.Length() + chainSegmentLength / 2f;

                while (chainLengthRemainingToDraw > 0f)
                {
                    Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, Color.Lime * 0.25f, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                    chainDrawPosition += unitVectorToPlayer * chainSegmentLength;
                    chainCount++;
                    chainLengthRemainingToDraw -= chainSegmentLength;
                }
            }

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
        }

		public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCProjectiles.Add(index);
		}

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return false;
		}

        public override bool CheckActive()
        {
            return false;
        }

		public override bool PreAI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];
            Player player = Main.player[Parent.target];

            NPC.alpha = Parent.alpha;

            //kill segment if the head doesnt exist
			if (!Parent.active || Parent.type != ModContent.NPCType<CamelColonel>())
            {
                NPC.active = false;
            }

			if (!SpawnedFirefly)
			{
				int NewNPC = NPC.NewNPC(NPC.GetSource_ReleaseEntity(), (int)NPC.Center.X, (int)NPC.Center.Y - 1000, ModContent.NPCType<SpotlightFirefly>(), ai3: NPC.ai[3]);
				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
				}

				SpawnedFirefly = true;
			}

			Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			NPC SegmentParent = Main.npc[(int)NPC.ai[1]];

			Vector2 SegmentCenter = SegmentParent.Center + SegmentParent.velocity - NPC.Center;

			//how far each segment should be from each other
			if (SegmentCenter != Vector2.Zero)
			{
				NPC.Center = SegmentParent.Center - SegmentCenter.SafeNormalize(Vector2.Zero) * 2;
			}

            bool AnotherMinibossPresent = SpiderWarWorld.EventActiveNPCCount() > 1;

            switch ((int)Parent.ai[0])
			{
                //green sniper bolts
                case 1:
				{
                    if (Parent.localAI[0] == 42)
                    {
                        float Multiplier = AnotherMinibossPresent ? 0f : 7f;

                        SavePlayerPosition = player.Center + player.velocity * Multiplier;

                        SaveRotation = NPC.rotation;

                        NPCGlobalHelper.ShootHostileProjectile(NPC, SavePlayerPosition, Vector2.Zero, ModContent.ProjectileType<CamelSniperReticle>(), NPC.damage, 1f);
                    }

                    if (Parent.localAI[0] == 60)
                    {
                        SoundEngine.PlaySound(SoundID.Item167, NPC.Center);
                        SoundEngine.PlaySound(SoundID.Item63 with { Pitch = -0.5f }, NPC.Center);

                        Vector2 ShootSpeed = SavePlayerPosition - NPC.Center;
						ShootSpeed.Normalize();
						ShootSpeed *= 7f;

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 30f;

                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center + muzzleOffset, ShootSpeed, ModContent.ProjectileType<CamelSniperBolt>(), NPC.damage, 1f);
                    }

                    if (Parent.localAI[0] >= 42 && Parent.localAI[0] <= 80)
                    {
                        NPC.rotation = SaveRotation;
                    }

                    break;
                }

                //purple venom bolts
                case 2:
				{
                    int Frequency = AnotherMinibossPresent ? 18 : 10;

                    if (Parent.localAI[0] >= 30 && Parent.localAI[0] <= 150 && Parent.localAI[0] % Frequency == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item98, NPC.Center);

                        SaveRotation = NPC.rotation;

                        Vector2 ShootSpeed = player.Center - NPC.Center;
						ShootSpeed.Normalize();
						ShootSpeed *= 12f;

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 30f;

                        Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(45));

                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center + muzzleOffset, newVelocity, ModContent.ProjectileType<CamelVenomBolt>(), NPC.damage, 1f);
                    }

                    break;
                }
            }

			return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CamelColonelTailSniperGore").Type);

                NPC Parent = Main.npc[(int)NPC.ai[3]];

                Parent.ai[0] = 4;
                Parent.localAI[0] = 0;
                Parent.localAI[1] = 0;

                foreach (NPC npc in Main.ActiveNPCs)
			    {
                    if (npc.type == ModContent.NPCType<CamelColonelTail>() && npc.ai[3] == NPC.ai[3])
                    {
                        Gore.NewGore(npc.GetSource_Death(), npc.Center, npc.velocity, ModContent.Find<ModGore>("Spooky/CamelColonelTailGore" + (npc.ai[2] + 1)).Type);

                        npc.life = 0;
                        npc.active = false;
                    }
                }
            }
        }
    }
}
