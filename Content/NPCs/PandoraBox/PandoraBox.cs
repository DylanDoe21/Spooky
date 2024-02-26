using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.NPCs.PandoraBox.Projectiles;

namespace Spooky.Content.NPCs.PandoraBox
{
    public class PandoraBox : ModNPC
    {
        bool SpawnedEnemies = false;
        bool EventEnemiesExist = true;
        bool PlayAnimation = false;
        bool EndingAnimation = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
			NPCID.Sets.NoTownNPCHappiness[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(SpawnedEnemies);
            writer.Write(EventEnemiesExist);
            writer.Write(PlayAnimation);
            writer.Write(EndingAnimation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            SpawnedEnemies = reader.ReadBoolean();
            EventEnemiesExist = reader.ReadBoolean();
            PlayAnimation = reader.ReadBoolean();
            EndingAnimation = reader.ReadBoolean();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 250;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 46;
            NPC.height = 36;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
        }

        public override bool NeedSaving()
        {
            return true;
        }

        public override bool CanChat()
        {
            //dont allow interactions with the box during the event
            return !PandoraBoxWorld.PandoraEventActive;
        }

        public override string GetChat()
		{
            return Language.GetTextValue("Mods.Spooky.Dialogue.PandoraBox.Dialogue");
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("Mods.Spooky.Dialogue.PandoraBox.Button");
		}

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
		{
            if (firstButton)
            {
                Main.npcChatText = "";
                SoundEngine.PlaySound(SoundID.Unlock, NPC.Center);
                PandoraBoxWorld.PandoraEventActive = true;
                NPC.ai[0] = 180;

                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PandoraLockGore" + numGores).Type);
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (PandoraBoxWorld.PandoraEventActive)
            {
                Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

                var effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Vector2 drawPosition = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4);
                Color newColor = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Cyan);

                for (int repeats = 0; repeats < 4; repeats++)
                {
                    Color color = newColor;
                    color = NPC.GetAlpha(color);
                    color *= 1f - fade;
                    Vector2 afterImagePosition = new Vector2(NPC.Center.X, NPC.Center.Y) + NPC.rotation.ToRotationVector2() - screenPos + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * repeats;
                    Main.spriteBatch.Draw(texture, afterImagePosition, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, effects, 0f);
                }

                Main.spriteBatch.Draw(texture, drawPosition, NPC.frame, newColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, effects, 0f);
            }

            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/PandoraBox/PandoraBoxGlow").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            if (!PandoraBoxWorld.PandoraEventActive)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
            else
            {
                if (PlayAnimation)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 7)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = 1 * frameHeight;
                        PlayAnimation = false;
                    }
                }
                else
                {
                    NPC.frameCounter = 0;
                    NPC.frame.Y = 1 * frameHeight;
                }
            }
        }

        public void SwitchToNextWave()
        {
            if (PandoraBoxWorld.SpawnedEnemySpawners && !EventEnemiesExist)
            {
                if (PandoraBoxWorld.Wave < 4)
                {
                    NPC.ai[0] = 180;
                    PandoraBoxWorld.Wave++;
                    SpawnedEnemies = false;
                    PandoraBoxWorld.SpawnedEnemySpawners = false;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.WorldData);
                    }

                    NPC.netUpdate = true;
                }
                else
                {
                    EndingAnimation = true;

                    NPC.netUpdate = true;
                }
            }
        }

        public void SpawnEnemies()
        {
            switch (PandoraBoxWorld.Wave)
            {
                //wave enemies: Bobbert x3, Stitch x1
                case 0:
                {
                    if (!SpawnedEnemies)
                    {
                        PlayAnimation = true;

                        if (NPC.frame.Y == 3 * 36)
                        {
                            for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<PandoraEnemySpawn>(), 0, 0, 0, 0);
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<PandoraEnemySpawn>(), 0, 0, 0, 1);

                            SpawnedEnemies = true;
                        }
                    }

                    break;
                }

                //wave enemies: Bobbert x2, Stitch x1, Sheldon x1
                case 1:
                {
                    if (!SpawnedEnemies)
                    {
                        PlayAnimation = true;

                        if (NPC.frame.Y == 3 * 36)
                        {
                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<PandoraEnemySpawn>(), 0, 0, 0, 0);
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<PandoraEnemySpawn>(), 0, 0, 0, 1);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<PandoraEnemySpawn>(), 0, 0, 0, 2);

                            SpawnedEnemies = true;
                        }
                    }

                    break;
                }

                //wave enemies: Stitch x3, Sheldon x2
                case 2:
                {
                    if (!SpawnedEnemies)
                    {
                        PlayAnimation = true;

                        if (NPC.frame.Y == 3 * 36)
                        {
                            for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<PandoraEnemySpawn>(), 0, 0, 0, 1);
                            }

                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<PandoraEnemySpawn>(), 0, 0, 0, 2);
                            }

                            SpawnedEnemies = true;
                        }
                    }

                    break;
                }

                //wave enemies: Bobbert x2, Stitch x2, Sheldon x2
                case 3:
                {
                    if (!SpawnedEnemies)
                    {
                        PlayAnimation = true;

                        if (NPC.frame.Y == 3 * 36)
                        {
                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<PandoraEnemySpawn>(), 0, 0, 0, 0);

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<PandoraEnemySpawn>(), 0, 0, 0, 1);

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.NextFloat(-8f, 8f), 
                                Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<PandoraEnemySpawn>(), 0, 0, 0, 2);
                            }

                            SpawnedEnemies = true;
                        }
                    }

                    break;
                }

                //wave enemies: Chester x1
                case 4:
                {
                    if (!SpawnedEnemies)
                    {
                        PlayAnimation = true;

                        if (NPC.frame.Y == 3 * 36)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 
                            Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<PandoraEnemySpawn>(), 0, 0, 0, 3);

                            SpawnedEnemies = true;
                        }
                    }

                    break;
                }
            }

            NPC.netUpdate = true;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            if (PandoraBoxWorld.PandoraEventActive)
            {
                if (NPC.AnyNPCs(ModContent.NPCType<Bobbert>()) || NPC.AnyNPCs(ModContent.NPCType<Stitch>()) || 
                NPC.AnyNPCs(ModContent.NPCType<Sheldon>()) || NPC.AnyNPCs(ModContent.NPCType<Chester>()))
                {
                    EventEnemiesExist = true;
                }
                else
                {
                    EventEnemiesExist = false;
                }

                //cooldown before switching to the next wave
                if (NPC.ai[0] > 0)
                {
                    NPC.ai[0]--;
                }
                
                SwitchToNextWave();

                if (NPC.ai[0] <= 0)
                {
                    SpawnEnemies();
                }

                //when the event is completed, spawn projectile that drops the items
                if (EndingAnimation)
                {
                    NPC.ai[1]++;

                    if (NPC.ai[1] < 90)
                    {
                        int MaxDusts = Main.rand.Next(5, 15);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 2f, (float)NPC.height / 2f) * Main.rand.NextFloat(1.5f, 2f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.1f);
                            Main.dust[dustEffect].color = Color.Cyan;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-5f, -2f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }
                    }

                    if (NPC.ai[1] >= 90)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_DarkMageAttack, NPC.Center);

                        NPC.SetEventFlagCleared(ref Flags.downedPandoraBox, -1);

                        PandoraBoxWorld.Wave = 0;
                        PandoraBoxWorld.SpawnedEnemySpawners = false;
                        PandoraBoxWorld.PandoraEventActive = false;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.WorldData);
                        }

                        SpawnedEnemies = false;
                        EventEnemiesExist = true;
                        EndingAnimation = false;

                        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center.X, NPC.Center.Y, 0, -1, ModContent.ProjectileType<PandoraLootSpawner>(), 0, 0, NPC.target);

                        NPC.ai[1] = 0;
                    }
                }
            }
            else
            {
                SpawnedEnemies = false;
                EventEnemiesExist = true;
            }
        }
    }
}