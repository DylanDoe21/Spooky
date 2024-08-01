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
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
    public class ExplodingAppendix : ModNPC
    {
        int ScaleTimerLimit = 8;
        float ScaleAmount = 0.1f;
        float SaveRotation;
        
        bool Shake = false;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 500;
            NPC.damage = 50;
            NPC.defense = 50;
            NPC.width = 52;
            NPC.height = 62;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.Item177;
			NPC.DeathSound = SoundID.NPCDeath12;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type, ModContent.GetInstance<Biomes.SpookyHellEventBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ExplodingAppendix"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/ExplodingAppendixGlow");

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            spriteBatch.Draw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 7)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            if (NPC.ai[0] == 0)
            {
                NPC.rotation += 0.02f * (float)NPC.direction + (NPC.velocity.X / 40);

                Vector2 GoTo = player.Center;

                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 1, 3);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                
                if (NPC.Distance(player.Center) <= 150f)
                {
                    SaveRotation = NPC.rotation;

                    NPC.ai[0]++;
                }
            }
            else
            {
                NPC.velocity *= 0.8f;

                if (Shake)
                {
                    NPC.rotation += 0.1f;
                    if (NPC.rotation > SaveRotation + 0.2f)
                    {
                        Shake = false;
                    }
                }
                else
                {
                    NPC.rotation -= 0.1f;
                    if (NPC.rotation < SaveRotation - 0.2f)
                    {
                        Shake = true;
                    }
                }

                NPC.ai[1]++;
                if (NPC.ai[1] < ScaleTimerLimit)
                {
                    NPC.scale -= ScaleAmount;
                }
                if (NPC.ai[1] >= ScaleTimerLimit)
                {
                    NPC.scale += ScaleAmount;
                }

                if (NPC.ai[1] > ScaleTimerLimit * 2)
                {
                    NPC.ai[1] = 0;
                    NPC.scale = 1.2f;
                }

                NPC.ai[2]++;
                if (NPC.ai[2] == 30 || NPC.ai[2] == 60 || NPC.ai[2] == 90)
                {
                    ScaleAmount += 0.1f;
                    ScaleTimerLimit -= 2;
                }

                //explode
                if (NPC.ai[2] >= 120)
                {
                    SoundEngine.PlaySound(SoundID.Item70, NPC.Center);
                    SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot, NPC.Center);

                    if (player.Distance(NPC.Center) <= 250f)
                    {
                        SpookyPlayer.ScreenShakeAmount = 4f;
                    }

                    //lingering ichor cloud
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<AppendixLingerCloud>(), NPC.damage / 4, 0);

                    //spawn splatter
                    int NumProjectiles = Main.rand.Next(15, 25);
                    for (int i = 0; i < NumProjectiles; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center.X, NPC.Center.Y, Main.rand.Next(-4, 5), Main.rand.Next(-4, -1), ModContent.ProjectileType<YellowSplatter>(), 0, 0);
                    }

                    player.ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0, false);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostOrroboroCondition(), ModContent.ItemType<ArteryPiece>(), 3, 1, 3));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                //spawn splatter
                for (int i = 0; i < 6; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center.X, NPC.Center.Y, Main.rand.Next(-8, 9), Main.rand.Next(-8, -3), ModContent.ProjectileType<YellowSplatter>(), 0, 0);
                }

                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ExplodingAppendixGore" + numGores).Type);
                    }
                }
            }
        }
    }
}