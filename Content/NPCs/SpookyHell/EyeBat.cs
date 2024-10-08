using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Items.Food;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class EyeBat : ModNPC
    {
        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(20f, -10f)
            };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 75;
            NPC.damage = 30;
            NPC.defense = 15;
            NPC.width = 92;
            NPC.height = 92;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 14;
            AIType = NPCID.Raven;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.EyeBat"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			if (NPC.localAI[0] >= 180)
			{
                Vector2 drawOrigin = new(NPCTexture.Width() * 0.5f, NPC.height * 0.5f);

                for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
                {
                    var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
                    Color color = NPC.GetAlpha(Color.Purple) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length));
                    spriteBatch.Draw(NPCTexture.Value, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
                }
			}
            
            return true;
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/EyeBatGlow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            //flying animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 3)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 7)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            //charging frame
            if (NPC.localAI[0] >= 240)
            {
                NPC.frame.Y = 7 * frameHeight;
            }
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];

            if (NPC.Distance(player.Center) <= 450f || NPC.localAI[0] >= 180)
            {
                NPC.localAI[0]++;
            }

            if (NPC.localAI[0] >= 240)
            {
                NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;
                NPC.rotation = 0;

                NPC.aiStyle = -1;
            }
            else
            {
                NPC.spriteDirection = NPC.direction;
                NPC.rotation = NPC.velocity.X * 0.04f;

                NPC.aiStyle = 14;
                AIType = NPCID.Raven;
            }
            
            if (NPC.localAI[0] >= 180 && NPC.localAI[0] < 240)
            {
                Vector2 GoTo = new Vector2(player.Center.X + (NPC.Center.X < player.Center.X ? -300 : 300), player.Center.Y);

                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 5, 10);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
            }

            if (NPC.localAI[0] == 240)
            {
                SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                Vector2 ChargeDirection = player.Center - NPC.Center;
                ChargeDirection.Normalize();
                        
                ChargeDirection.X *= 25;
                NPC.velocity.X = ChargeDirection.X;

                NPC.velocity.Y *= 0.2f;
            }

            if (NPC.localAI[0] >= 270)
            {
                NPC.velocity *= 0.9f;
            }

            if (NPC.localAI[0] >= 295)
            {
                NPC.localAI[0] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CreepyChunk>(), 3, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MonsterBloodVial>(), 100));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GoofyPretzel>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyeBatGore1").Type);
                }

                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyeBatGore2").Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EyeBatGore3").Type);
                    }
                }
            }
        }
    }
}