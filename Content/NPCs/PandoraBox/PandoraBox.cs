using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Core;
using Spooky.Content.Achievements;
using Spooky.Content.Dusts;
using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpiderCave;

namespace Spooky.Content.NPCs.PandoraBox
{
    public class PandoraBox : ModNPC
    {
        bool SpawnedEnemies = false;
        bool HasDoneSpawnAnimation = false;
        bool EventEnemiesExist = true;
        bool PlayAnimation = false;
        bool EndingAnimation = false;
        bool Shake = false;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;
			NPCID.Sets.NoTownNPCHappiness[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(SpawnedEnemies);
            writer.Write(HasDoneSpawnAnimation);
            writer.Write(EventEnemiesExist);
            writer.Write(PlayAnimation);
            writer.Write(EndingAnimation);
            writer.Write(Shake);

            //floats
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            SpawnedEnemies = reader.ReadBoolean();
            HasDoneSpawnAnimation = reader.ReadBoolean();
            EventEnemiesExist = reader.ReadBoolean();
            PlayAnimation = reader.ReadBoolean();
            EndingAnimation = reader.ReadBoolean();
            Shake = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 250;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 46;
            NPC.height = 36;
            NPC.npcSlots = 0f;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (PandoraBoxWorld.PandoraEventActive)
            {
                NPCTexture ??= ModContent.Request<Texture2D>(Texture);

                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

                var effects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Vector2 drawPosition = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4);
                Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Cyan);

                for (int repeats = 0; repeats < 4; repeats++)
                {
                    Vector2 afterImagePosition = new Vector2(NPC.Center.X, NPC.Center.Y) + NPC.rotation.ToRotationVector2() - screenPos + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * repeats;
                    Main.spriteBatch.Draw(NPCTexture.Value, afterImagePosition, NPC.frame, color * fade, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, effects, 0f);
                }

                Main.spriteBatch.Draw(NPCTexture.Value, drawPosition, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.2f, effects, 0f);
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
                    }
                }
                else
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
            }
        }

        public override bool CheckActive()
		{
			return false;
		}

        public void SwitchToNextWave()
        {
            if (HasDoneSpawnAnimation && !EventEnemiesExist)
            {
                if (PandoraBoxWorld.Wave < 4)
                {
                    NPC.ai[0] = 180;
                    
                    SpawnedEnemies = false;
                    HasDoneSpawnAnimation = false;

					PandoraBoxWorld.Wave++;

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}
				}
                else
                {
                    EndingAnimation = true;
                }
                
                NPC.netUpdate = true;
            }
        }

        public void SpawnEnemy(int Type)
        {
            int NPCToSpawn = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                switch (Type)
                {
                    //bobbert
                    case 0:
                    {
                        NPCToSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Bobbert>());
                        Main.npc[NPCToSpawn].velocity.X = Main.rand.Next(-10, 11);
                        Main.npc[NPCToSpawn].velocity.Y = Main.rand.Next(-10, -5);
                        break;
                    }

                    //stitch
                    case 1:
                    {
                        NPCToSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Stitch>());
                        Main.npc[NPCToSpawn].velocity.X = Main.rand.Next(-10, 11);
                        Main.npc[NPCToSpawn].velocity.Y = Main.rand.Next(-10, -5);
                        break;
                    }

                    //sheldon
                    case 2:
                    {
                        NPCToSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Sheldon>());
                        Main.npc[NPCToSpawn].velocity.X = Main.rand.Next(-10, 11);
                        Main.npc[NPCToSpawn].velocity.Y = Main.rand.Next(-10, -5);
                        break;
                    }

                    //chester
                    case 3:
                    {
                        NPCToSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Chester>());
                        Main.npc[NPCToSpawn].velocity.Y = -8;
                        break;
                    }
                }
            }

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.SyncNPC, number: NPCToSpawn);
            }

			HasDoneSpawnAnimation = true;

            NPC.netUpdate = true;
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

                        NPC.ai[3]++;
                        if (NPC.ai[3] > 30)
                        {
                            SoundEngine.PlaySound(SoundID.AbigailUpgrade, NPC.Center);
                            SoundEngine.PlaySound(SoundID.NPCDeath33, NPC.Center);

                            for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                            {
                                SpawnEnemy(0);
                            }

                            SpawnEnemy(1);

                            NPC.ai[3] = 0;
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

                        if (NPC.localAI[0] == 10)
                        {
                            SoundEngine.PlaySound(SoundID.AbigailUpgrade, NPC.Center);
                            SoundEngine.PlaySound(SoundID.NPCDeath33, NPC.Center);

                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                SpawnEnemy(0);
                            }

                            SpawnEnemy(1);
                            SpawnEnemy(2);

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

                        if (NPC.localAI[0] == 10)
                        {
                            SoundEngine.PlaySound(SoundID.AbigailUpgrade, NPC.Center);
                            SoundEngine.PlaySound(SoundID.NPCDeath33, NPC.Center);

                            for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                            {
                                SpawnEnemy(1);
                            }

                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                SpawnEnemy(2);
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

                        if (NPC.localAI[0] == 10)
                        {
                            SoundEngine.PlaySound(SoundID.AbigailUpgrade, NPC.Center);
                            SoundEngine.PlaySound(SoundID.NPCDeath33, NPC.Center);

                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                SpawnEnemy(0);
                                SpawnEnemy(1);
                                SpawnEnemy(2);
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

                        if (NPC.localAI[0] == 10)
                        {
                            SoundEngine.PlaySound(SoundID.AbigailUpgrade, NPC.Center);
                            SoundEngine.PlaySound(SoundID.NPCDeath33, NPC.Center);

                            SpawnEnemy(3);

                            SpawnedEnemies = true;
                        }
                    }

                    break;
                }
            }

            NPC.netUpdate = true;
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

        public override void AI()
        {
            Player player = Main.player[NPC.target];

            if (NPC.ai[2] > 0)
            {
                SoundEngine.PlaySound(SoundID.Unlock, NPC.Center);

                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PandoraLockGore" + numGores).Type);
                    }
                }

                PandoraBoxWorld.PandoraEventActive = true;

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }

                NPC.ai[0] = 180;
                NPC.ai[2] = 0;

                NPC.netUpdate = true;
            }

            if (PlayAnimation)
            {
                NPC.localAI[0]++;
                if (NPC.localAI[0] >= 20)
                {
                    PlayAnimation = false;
                    NPC.localAI[0] = 0;
                    NPC.netUpdate = true;
                }
            }

            if (PandoraBoxWorld.PandoraEventActive)
            {
                //bool to check if any enemies during the event exist
                EventEnemiesExist = NPC.AnyNPCs(ModContent.NPCType<Bobbert>()) || NPC.AnyNPCs(ModContent.NPCType<Stitch>()) || NPC.AnyNPCs(ModContent.NPCType<Sheldon>()) || NPC.AnyNPCs(ModContent.NPCType<Chester>());

                //cooldown before switching to the next wave
                if (NPC.ai[0] > 0)
                {
                    NPC.ai[0]--;

                    PlayAnimation = false;

                    if (NPC.ai[0] <= 60)
                    {
                        if (Shake)
                        {
                            NPC.rotation += 0.1f;
                            if (NPC.rotation > 0.2f)
                            {
                                Shake = false;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.rotation -= 0.1f;
                            if (NPC.rotation < -0.2f)
                            {
                                Shake = true;
                                NPC.netUpdate = true;
                            }
                        }
                    }
                }

                if (NPC.ai[0] <= 0 && !EndingAnimation)
                {
                    NPC.rotation = 0;
                    SwitchToNextWave();
                    SpawnEnemies();
                }
            }
            else
            {
                SpawnedEnemies = false;
                HasDoneSpawnAnimation = false;
                PlayAnimation = false;
            }

            //when the event is completed, drop the item loot
            if (EndingAnimation)
            {
                NPC.ai[1]++;

                if (NPC.ai[1] < 90)
                {
                    if (Shake)
                    {
                        NPC.rotation += 0.1f;
                        if (NPC.rotation > 0.2f)
                        {
                            Shake = false;
                        }
                    }
                    else
                    {
                        NPC.rotation -= 0.1f;
                        if (NPC.rotation < -0.2f)
                        {
                            Shake = true;
                        }
                    }

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
                    NPC.rotation = 0;

                    PlayAnimation = true;

                    if (NPC.ai[1] >= 115)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_DarkMageAttack, NPC.Center);
                        
                        //drop one of the pandora accessories
                        int[] Accessories = new int[] { ModContent.ItemType<PandoraChalice>(), ModContent.ItemType<PandoraCross>(), 
                        ModContent.ItemType<PandoraCuffs>(), ModContent.ItemType<PandoraRosary>() };

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int newItem = Item.NewItem(NPC.GetSource_DropAsItem(), NPC.Hitbox, Main.rand.Next(Accessories));
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                            }

                            int oldHunterSoul = Item.NewItem(NPC.GetSource_DropAsItem(), NPC.Hitbox, ModContent.ItemType<OldHunterSoul>());
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, oldHunterSoul, 1f);
                            }

                            //chance to drop the funny bean
                            if (Main.rand.NextBool(20))
                            {
                                int FunnyBean = Item.NewItem(NPC.GetSource_DropAsItem(), NPC.Hitbox, ModContent.ItemType<PandoraBean>());
                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, FunnyBean, 1f);
                                }
                            }
                        }

                        Flags.downedPandoraBox = true;
                        PandoraBoxWorld.PandoraEventActive = false;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.WorldData);
                        }

                        ModContent.GetInstance<EventAchievementPandoraBox>().PandoraBoxCondition.Complete();

                        NPC.ai[1] = 0;

                        EndingAnimation = false;

                        NPC.netUpdate = true;
                    }
                }
            }
        }

        //this exists entirely because boss checklist doesnt let you manually add non-vanity items to the "collectibles" list
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PandoraChalice>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PandoraCross>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PandoraCuffs>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PandoraRosary>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PandoraBean>()));
        }
    }
}