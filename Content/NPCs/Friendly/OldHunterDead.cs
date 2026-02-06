using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Friendly
{
	public class OldHunterDead : ModNPC
	{
        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> AuraTexture;

		public override void SetStaticDefaults()
		{
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 250;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 38;
			NPC.height = 54;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
            NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            AuraTexture ??= ModContent.Request<Texture2D>(Texture + "Aura");
            
            if (NPC.ai[1] > 0)
            {
                float scaleTime = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

                float time = Main.GlobalTimeWrappedHourly;

                time %= 4f;
                time /= 2f;

                if (time >= 1f)
                {
                    time = 2f - time;
                }

                time = time + 0.5f;

                for (float i = 0f; i < 1f; i += 0.1f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;

                    Main.EntitySpriteDraw(AuraTexture.Value, NPC.Center - screenPos + new Vector2(12, NPC.gfxOffY + 12).RotatedBy(radians) * time, 
                    NPC.frame, NPC.GetAlpha(Color.Cyan * 0.2f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale + scaleTime * 1.2f, SpriteEffects.None, 0);
                }
            }
            
            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}

		public override bool CheckActive()
        {
            return false;
        }

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

        public override bool CanChat()
        {
            return NPC.ai[1] <= 0;
        }

        public override string GetChat()
		{
            return Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.OldHunterCorpse");
        }

        public override void AI()
		{
			if (NPC.ai[1] == 1)
            {
                NPC.ai[0]++;
                if (NPC.ai[0] == 1)
                {
                    Flags.OldHunterRevived = true;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.WorldData);
                    }
                }
                if (NPC.ai[0] % 25 == 0)
                {
                    int Distance = Main.rand.Next(125, 251);

                    int Amount = Main.rand.Next(1, 3);
                    for (int numDusts = 0; numDusts < Amount; numDusts++)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 offset = new Vector2();
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * Distance);
                            offset.Y += (float)(Math.Cos(angle) * Distance);
                            Vector2 DustPos = NPC.Center + offset - new Vector2(4, 4);
                            Dust dust = Main.dust[Dust.NewDust(DustPos, 0, 0, DustID.UltraBrightTorch, 0, 0, 100, Color.White, 1f)];
                            dust.velocity = -((DustPos - NPC.Center) * Main.rand.NextFloat(0.01f, 0.12f));
                            dust.noGravity = true;
                            dust.scale = 2f;
                        }
                    }
                }

                if (NPC.ai[0] >= 240)
                {
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, NPC.Center);
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, NPC.Center);

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)SpookyMessageType.SpawnOldHunter);
                        packet.Send();
                    }
                    else
                    {
                        Flags.SpawnOldHunter = true;
                    }

                    float maxAmount = 25;
                    int currentAmount = 0;
                    while (currentAmount <= maxAmount)
                    {
                        Vector2 velocity = new Vector2(Main.rand.NextFloat(1f, 10f), Main.rand.NextFloat(1f, 10f));
                        Vector2 Bounds = new Vector2(Main.rand.NextFloat(1f, 6f), Main.rand.NextFloat(1f, 6f));
                        float intensity = Main.rand.NextFloat(1f, 10f);

                        Vector2 vector12 = Vector2.UnitX * 0f;
                        vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                        vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                        int num104 = Dust.NewDust(NPC.Center, 0, 0, DustID.UltraBrightTorch, 0f, 0f, 100, default, 3f);
                        Main.dust[num104].noGravity = true;
                        Main.dust[num104].position = NPC.Center + vector12;
                        Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
                        currentAmount++;
                    }

                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                
                    NPC.netUpdate = true;
                }
            }
        }
    }
}