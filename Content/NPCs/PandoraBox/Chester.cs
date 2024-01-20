using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.NPCs.PandoraBox.Projectiles;

namespace Spooky.Content.NPCs.PandoraBox
{
    public class Chester : ModNPC
    {
        public bool SpawnedWeapons = false;

        public static readonly SoundStyle BurpSound = new("Spooky/Content/Sounds/ChesterBurp", SoundType.Sound) { PitchVariance = 0.6f };

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 4200;
            NPC.damage = 60;
            NPC.defense = 10;
            NPC.width = 60;
            NPC.height = 84;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit54;
            NPC.DeathSound = SoundID.NPCDeath52;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type, ModContent.GetInstance<Biomes.PandoraBoxBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Chester"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.PandoraBoxBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			for (int k = 0; k < Main.maxNPCs; k++)
			{
				if (Main.npc[k].active && Main.npc[k].type == ModContent.NPCType<PandoraBox>() && NPC.Distance(Main.npc[k].Center) <= 750f) 
				{
					Vector2 ParentCenter = Main.npc[k].Center;

					Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/PandoraBox/ChainBig");

					Rectangle? chainSourceRectangle = null;
					float chainHeightAdjustment = 0f;

					Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
					Vector2 chainDrawPosition = NPC.Center;
					Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
					Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
					float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;

					if (chainSegmentLength == 0)
					{
						chainSegmentLength = 10;
					}

					float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
					int chainCount = 0;
					float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;
		
					while (chainLengthRemainingToDraw > 0f)
					{
						Color chainDrawColor = Color.Cyan * 0.5f;

						var chainTextureToDraw = chainTexture;

						Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

						chainDrawPosition += unitVectorToParent * chainSegmentLength;
						chainCount++;
						chainLengthRemainingToDraw -= chainSegmentLength;
					}
				}
			}

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            float num108 = 4;
            float num107 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6.28318548f)) / 2f + 0.5f;
            float num106 = 0f;

            SpriteEffects spriteEffects3 = (NPC.spriteDirection == 1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 vector33 = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity;
			Color color29 = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.LightBlue);

            for (int num103 = 0; num103 < 4; num103++)
            {
				Color color28 = color29;
                color28 = NPC.GetAlpha(color28);
                color28 *= 1f - num107;
                Vector2 vector29 = new Vector2(NPC.Center.X, NPC.Center.Y) + ((float)num103 / (float)num108 * 6.28318548f + NPC.rotation + num106).ToRotationVector2() * (4f * num107 + 2f) - screenPos + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * (float)num103;
                Main.spriteBatch.Draw(tex, vector29, NPC.frame, color28, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, spriteEffects3, 0f);
            }

            Main.spriteBatch.Draw(tex, vector33, NPC.frame, color29, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, spriteEffects3, 0f);

            return true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool CheckActive()
        {
            return !PandoraBoxWorld.PandoraEventActive;
        }
        
        public override void AI()
		{
			Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.rotation = NPC.velocity.X * 0.04f;

            if (!SpawnedWeapons)
            {
                NPC.ai[1] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ChesterShield>(), ai1: NPC.whoAmI);
                NPC.ai[2] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<ChesterClub>(), ai2: NPC.whoAmI);
                
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[1]);
                    NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[2]);
                }

                SpawnedWeapons = true;
                NPC.netUpdate = true;
            }

            switch ((int)NPC.ai[0])
            {
                //fly above player
                case 0:
                {
                    NPC.localAI[0]++;

                    Vector2 GoTo = new Vector2(player.Center.X , player.Center.Y - 200);

                    float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 3, 7);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                    if (NPC.localAI[0] >= 240)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //stay still, try to bonk player with club
                case 1:
                {
                    NPC.localAI[0]++;

                    NPC.velocity *= 0.5f;

                    if (NPC.localAI[0] >= 120)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                    }

                    break;
                }

                //fly above player, but put the shield between itself and the player
                case 2:
                {
                    NPC.localAI[0]++;

                    Vector2 GoTo = new Vector2(player.Center.X , player.Center.Y - 200);

                    float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 3, 7);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                    if (NPC.localAI[0] >= 240)
                    {
                        NPC.localAI[0] = 0;

                        if (NPC.AnyNPCs(ModContent.NPCType<Bobbert>()) || NPC.AnyNPCs(ModContent.NPCType<Stitch>()) || NPC.AnyNPCs(ModContent.NPCType<Sheldon>()))
                        {
                            NPC.ai[0] = 0;
                        }
                        else
                        {
                            NPC.ai[0]++;
                        }
                    }

                    break;
                }

                //summon some enemies
                case 3:
                {
                    NPC.localAI[0]++;

                    NPC.velocity *= 0.2f;

                    if (NPC.localAI[0] == 85)
                    {
                        SoundEngine.PlaySound(BurpSound, NPC.Center);

                        for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-5f, 5f), 
                            Main.rand.NextFloat(-5f, -3f), ModContent.ProjectileType<PandoraEnemySpawnChester>(), 0, 0, 0);
                        }
                    }

                    if (NPC.localAI[0] >= 140)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;
                    }

                    break;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 30; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.2f);
                    Main.dust[dustGore].color = Color.Cyan;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
                    Main.dust[dustGore].noGravity = true;
                }
            }
        }
    }
}