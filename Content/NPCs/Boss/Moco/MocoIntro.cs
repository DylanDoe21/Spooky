using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

using Spooky.Content.NPCs.Boss.Moco.Projectiles;

namespace Spooky.Content.NPCs.Boss.Moco
{
    public class MocoIntro : ModNPC
    {
        public override string Texture => "Spooky/Content/NPCs/SpookyHell/Mocling";

        float RandomGoToX;
        float RandomGoToY;

        Vector2 SaveNPCPosition;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;
        
        public override void SetStaticDefaults()
		{
            Main.npcFrameCount[NPC.type] = 9;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.width = 38;
            NPC.height = 36;
            NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
			NPC.dontTakeDamage = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 2)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/MoclingGlow");

			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
			Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);

            return false;
		}

		public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC Parent = Main.npc[(int)NPC.ai[3]];

            NPC.rotation = NPC.velocity.X * 0.01f;
            NPC.spriteDirection = NPC.Center.X >= Parent.Center.X ? -1 : 1;

            NPC.ai[0]++;

            if (NPC.ai[0] == 1)
            {
                RandomGoToX = Main.rand.NextFloat(-85f, 85f);
                RandomGoToY = Main.rand.NextFloat(-100f, -50f);
            }

            if (NPC.ai[0] > 1 && NPC.ai[0] <= 10)
            {
                if (Parent.active && Parent.type == ModContent.NPCType<MocoSpawner>())
                {
                    MoveToParent(Parent, RandomGoToX, RandomGoToY);
                }
            }
            else
            {
                NPC.velocity *= 0.92f;
            }

            if (NPC.ai[0] >= 100 && NPC.ai[1] < 5)
            {
                NPC.ai[1]++;
                NPC.ai[0] = 0;
            }

            if (NPC.ai[1] >= 3)
            {
                NPC.ai[2]++;

                if (Parent.active && Parent.type == ModContent.NPCType<MocoSpawner>())
                {
                    Parent.ai[1] = 1;

                    //spawn dusts from the moco shrine head that the mocling aborbs
                    if (Parent.ai[2] <= 200)
                    {
                        if (Parent.ai[2] % 20 == 0)
                        {
                            Vector2 ProjectilePosition = Parent.Center + new Vector2(0, 65).RotatedByRandom(360);

                            Vector2 Velocity = Parent.Center - ProjectilePosition;
                            Velocity.Normalize();
                            Velocity *= 5f;

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), ProjectilePosition, Velocity, ModContent.ProjectileType<MocoSpawnerPower>(), 0, 0, Main.myPlayer, NPC.whoAmI);
                        }
                    }
                }

                if (NPC.ai[2] == 300)
                {
                    SaveNPCPosition = NPC.Center;
                }

                if (NPC.ai[2] >= 300)
                {
                    NPC.spriteDirection = NPC.direction;
                }

                if (NPC.ai[2] > 300 && NPC.ai[2] < 360)
                {
                    NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                    NPC.Center += Main.rand.NextVector2Square(-5, 5);

                    //make the mocling scale up and down rapidly
                    NPC.ai[3]++;
                    if (NPC.ai[3] < 3)
                    {
                        NPC.scale -= 0.2f;
                    }
                    if (NPC.ai[3] >= 3)
                    {
                        NPC.scale += 0.2f;
                    }
                    
                    if (NPC.ai[3] > 6)
                    {
                        NPC.ai[3] = 0;
                        NPC.scale = 1f;
                    }
                }

                if (NPC.ai[2] >= 360)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, NPC.Center);

                    //spawn particles
                    for (int numParticles = 0; numParticles <= 12; numParticles++)
                    {
                        ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.TrueNightsEdge, new ParticleOrchestraSettings
                        {
                            PositionInWorld = NPC.Center + new Vector2(Main.rand.Next(-55, 56), Main.rand.Next(-55, 56))
                        });
                    }

                    int Moco = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 50, ModContent.NPCType<Moco>());
                    Main.npc[Moco].alpha = 255;

                    if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.SyncNPC, number: Moco);
					}

                    NPC.active = false;
                    NPC.netUpdate = true;
                }
            }
		}

        public void MoveToParent(NPC parent, float TargetPositionX, float TargetPositionY)
        {
            Vector2 GoTo = parent.Center + new Vector2(TargetPositionX, TargetPositionY);

            if (NPC.Distance(GoTo) >= 200f)
            { 
                GoTo -= NPC.DirectionTo(GoTo) * 100f;
            }

            Vector2 GoToVelocity = GoTo - NPC.Center;

            float lerpValue = Utils.GetLerpValue(100f, 600f, GoToVelocity.Length(), false);

            float velocityLength = GoToVelocity.Length();

            if (velocityLength > 18f)
            { 
                velocityLength = 18f;
            }

            NPC.velocity = Vector2.Lerp(GoToVelocity.SafeNormalize(Vector2.Zero) * velocityLength, GoToVelocity / 6f, lerpValue);
            NPC.netUpdate = true;
        }
    }
}