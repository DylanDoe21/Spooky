using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.Chat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Core;
using Spooky.Effects;
using Spooky.Content.Biomes;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.EggEvent.Projectiles;
using Spooky.Content.Items.SpiderCave.Misc;
using Terraria.Audio;

namespace Spooky.Content.NPCs.EggEvent
{
    //TODO:
    //make sure that the egg event being downed is actually set and saved properly in multiplayer
    //implement SendExtraAI/ReceiveExtraAI for all custom variables in this egg npc
    //add a new projectile visual that goes from killed enemies and flies towards the egg before dying (like the lunar pillar enemies)
    public class OrroboroEgg : ModNPC
    {
        float ShieldScale = 0.5f;

        float addedStretch = 0f;
		float stretchRecoil = 0f;

        bool SpawnedEnemies = false;
        bool EventEnemiesExist = true;

        bool OrroboroDoesNotExist;

        public static readonly SoundStyle EventEndSound = new("Spooky/Content/Sounds/EggEvent/EggEventEnd", SoundType.Sound) { Volume = 2f };
        public static readonly SoundStyle EggDecaySound = new("Spooky/Content/Sounds/Orroboro/EggDecay", SoundType.Sound);
        public static readonly SoundStyle EggCrackSound1 = new("Spooky/Content/Sounds/Orroboro/EggCrack1", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle EggCrackSound2 = new("Spooky/Content/Sounds/Orroboro/EggCrack2", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(SpawnedEnemies);
            writer.Write(EventEnemiesExist);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            SpawnedEnemies = reader.ReadBoolean();
            EventEnemiesExist = reader.ReadBoolean();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 250;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 128;
            NPC.height = 122;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			Texture2D Tex = ModContent.Request<Texture2D>(Texture).Value;

			float stretch = 0f;

			stretch = Math.Abs(stretch) - addedStretch;
			
			//limit how much it can stretch
			if (stretch > 0.2f)
			{
				stretch = 0.2f;
			}

			//limit how much it can squish
			if (stretch < -0.2f)
			{
				stretch = -0.2f;
			}

			Vector2 scaleStretch = new Vector2(1f - stretch, 1f + stretch);

            Vector2 DrawPos = NPC.Center + new Vector2(0, NPC.height / 2 + NPC.gfxOffY + 4) - Main.screenPosition;

            spriteBatch.Draw(Tex, DrawPos, NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, SpriteEffects.None, 0f);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

            if (!Flags.downedEggEvent || EggEventWorld.EggEventActive) 
            {
                Color shieldColor = Color.Indigo;

                if (EggEventWorld.EggEventActive)
                {
                    shieldColor = Color.Lerp(Color.Indigo, Color.Red, fade);

                    if (ShieldScale < 0.8f)
                    {
                        ShieldScale += 0.0025f;
                    }
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                var center = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 12);
                DrawData drawData = new DrawData(ModContent.Request<Texture2D>("Spooky/ShaderAssets/EggShieldNoise").Value, center,
                new Rectangle(0, 0, 500, 420), shieldColor, 0, new Vector2(250f, 250f), NPC.scale * (ShieldScale + fade * 0.05f), SpriteEffects.None, 0);

                GameShaders.Misc["ForceField"].UseColor(new Vector3(1f + fade * 0.5f));
                GameShaders.Misc["ForceField"].Apply(drawData);
                drawData.Draw(Main.spriteBatch);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[1] == 0)
            {
                if (OrroboroDoesNotExist)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
                else
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
            }
            else
            {
                NPC.frame.Y = (int)NPC.ai[2] * frameHeight;
            }
        }

        public override bool NeedSaving()
        {
            return true;
        }

        public void SwitchToNextWave()
        {
            if (EggEventWorld.Wave < 9)
            {
                CombatText.NewText(NPC.getRect(), Color.Magenta, Language.GetTextValue("Mods.Spooky.EventsAndBosses.EggEventWaveComplete"), true);
                
                NPC.ai[0] = 420;
                EggEventWorld.Wave++;
                SpawnedEnemies = false;

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }

                NPC.netUpdate = true;
            }
            else
            {
                SoundEngine.PlaySound(EventEndSound, NPC.Center);

                NPC.ai[0] = 0;
                EggEventWorld.Wave = 0;
                EggEventWorld.EggEventActive = false;

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }

                if (!Flags.downedEggEvent)
                {
                    //event end message
                    string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.EggEventOver");
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Main.NewText(text, 171, 64, 255);
                    }
                    else
                    {
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                    }

                    NPC.SetEventFlagCleared(ref Flags.downedEggEvent, -1);
                }

                NPC.netUpdate = true;
            }
        }

        //spawn an enemy based on the type inputted
        public void SpawnEnemy(int Type)
        {
            //0 = Glutinous
            //1 = Vigilante
            //2 = Ventricle
            //3 = Crux
            //4 = Vesicator

            int Spawner = Projectile.NewProjectile(NPC.GetSource_FromAI(), (int)(NPC.Center.X + Main.rand.Next(-900, 900)), (int)(NPC.Center.Y + Main.rand.Next(100, 150)), 
            Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, -5f), ModContent.ProjectileType<GiantBiomass>(), 0, 0, 0, 0, 0, Type);
            Main.projectile[Spawner].rotation += Main.rand.NextFloat(0f, 360f);
        }

        //spawn enemies for each wave, along with additional enemies when the event is rematched
        public void SpawnEnemies()
        {
            if (!SpawnedEnemies)
            {
                switch (EggEventWorld.Wave)
                {
                    //wave enemies: Glutinous x3, Vigilante x1
                    //post defeat: Vigilante x1, Ventricle x1
                    case 0:
                    {
                        for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                        {
                            SpawnEnemy(0);
                        }
                        
                        SpawnEnemy(1);

                        if (Flags.downedEggEvent)
                        {
                            SpawnEnemy(1);
                            SpawnEnemy(2);
                        }

                        break;
                    }
                    
                    //wave enemies: Glutinous x4, Vigilante x2
                    //post defeat: Ventricle x2
                    case 1:
                    {
                        for (int numEnemy = 1; numEnemy <= 4; numEnemy++)
                        {
                            SpawnEnemy(0);
                        }

                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            SpawnEnemy(1);
                        }

                        if (Flags.downedEggEvent)
                        {
                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                SpawnEnemy(2);
                            }
                        }

                        break;
                    }

                    //wave enemies: Glutinous x3, Vigilante x2, Ventricle x1
                    //post defeat: Vigilante x1, Crux x1
                    case 2:
                    {
                        for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                        {
                            SpawnEnemy(0);
                        }

                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            SpawnEnemy(1);
                        }

                        SpawnEnemy(2);

                        if (Flags.downedEggEvent)
                        {
                            SpawnEnemy(1);
                            SpawnEnemy(3);
                        }

                        break;
                    }

                    //wave enemies: Glutinous x2, Vigilante x2, Ventricle x2
                    //post defeat: Crux x2
                    case 3:
                    {
                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            SpawnEnemy(0);
                            SpawnEnemy(1);
                            SpawnEnemy(2);
                        }

                        if (Flags.downedEggEvent)
                        {
                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                SpawnEnemy(3);
                            }
                        }

                        break;
                    }

                    //wave enemies: Vigilante x3, Crux x2
                    //post defeat: Glutinous x2, Crux x1
                    case 4:
                    {
                        for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                        {
                            SpawnEnemy(1);
                        }

                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            SpawnEnemy(3);
                        }

                        if (Flags.downedEggEvent)
                        {
                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                SpawnEnemy(0);
                            }

                            SpawnEnemy(3);
                        }

                        break;
                    }

                    //wave enemies: Glutinous x5, Vigilante x2, Crux x2
                    //post defeat: Vesicator x1
                    case 5:
                    {
                        for (int numEnemy = 1; numEnemy <= 5; numEnemy++)
                        {
                            SpawnEnemy(0);
                        }

                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            SpawnEnemy(1);
                        }

                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            SpawnEnemy(3);
                        }

                        if (Flags.downedEggEvent)
                        {
                            SpawnEnemy(4);
                        }

                        break;
                    }

                    //wave enemies: Glutinous x8, Vesicator x1
                    //post defeat: Ventricle x2
                    case 6:
                    {
                        for (int numEnemy = 1; numEnemy <= 8; numEnemy++)
                        {
                            SpawnEnemy(0);
                        }

                        SpawnEnemy(4);

                        if (Flags.downedEggEvent)
                        {
                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                SpawnEnemy(2);
                            }
                        }

                        break;
                    }

                    //wave enemies: Glutinous x4, Vigilante x3, Ventricle x2, Crux x2
                    //post defeat: Vigilante x2, Ventricle x2
                    case 7:
                    {
                        for (int numEnemy = 1; numEnemy <= 4; numEnemy++)
                        {
                            SpawnEnemy(0);
                        }

                        for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                        {   
                            SpawnEnemy(1);
                        }

                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            SpawnEnemy(2);
                            SpawnEnemy(3);
                        }

                        if (Flags.downedEggEvent)
                        {
                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                SpawnEnemy(1);
                                SpawnEnemy(2);
                            }
                        }

                        break;
                    }

                    //wave enemies: Glutinous x6, Vigilante x6, Crux x2, Vesicator x1
                    //post defeat: Ventricle x2
                    case 8:
                    {
                        for (int numEnemy = 1; numEnemy <= 6; numEnemy++)
                        {
                            SpawnEnemy(0);
                        }

                        for (int numEnemy = 1; numEnemy <= 6; numEnemy++)
                        {   
                            SpawnEnemy(1);
                        }

                        for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                        {
                            SpawnEnemy(3);
                        }

                        SpawnEnemy(4);

                        if (Flags.downedEggEvent)
                        {
                            for (int numEnemy = 1; numEnemy <= 2; numEnemy++)
                            {
                                SpawnEnemy(2);
                            }
                        }

                        break;
                    }

                    //wave enemies: Glutinous x6, Vigilante x5, Ventricle x3, Crux x1, Vesicator x1
                    //post defeat: Glutinous x2, Vigilante x2, Ventricle x1, Crux x1
                    case 9:
                    {
                        for (int numEnemy = 1; numEnemy <= 6; numEnemy++)
                        {
                            SpawnEnemy(0);
                        }

                        for (int numEnemy = 1; numEnemy <= 5; numEnemy++)
                        {   
                            SpawnEnemy(1);
                        }

                        for (int numEnemy = 1; numEnemy <= 3; numEnemy++)
                        {   
                            SpawnEnemy(2);
                        }

                        SpawnEnemy(3);
                        SpawnEnemy(4);

                        if (Flags.downedEggEvent)
                        {
                            for (int numEnemy = 1; numEnemy <= 6; numEnemy++)
                            {
                                SpawnEnemy(0);
                                SpawnEnemy(1);
                            }

                            SpawnEnemy(2);
                            SpawnEnemy(3);
                        }

                        break;
                    }
                }

                SpawnedEnemies = true;
            }
        }

        public override void AI()
        {
            Player player = Main.LocalPlayer;

            Spooky.OrroboroSpawnX = (int)NPC.Center.X;
            Spooky.OrroboroSpawnY = (int)NPC.Center.Y;

            OrroboroDoesNotExist = !NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) && !NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) && !NPC.AnyNPCs(ModContent.NPCType<BoroHead>());

            if (!Flags.downedEggEvent || EggEventWorld.EggEventActive) 
            {
                Lighting.AddLight(NPC.Center, Color.Indigo.ToVector3());
            }

            //stretch stuff
            if (stretchRecoil > 0)
			{
				stretchRecoil -= 0.03f;
			}
			else
			{
				stretchRecoil = 0;
			}

			addedStretch = -stretchRecoil;

            //right click functionality
            if (NPC.Hitbox.Intersects(new Rectangle((int)Main.MouseWorld.X - 1, (int)Main.MouseWorld.Y - 1, 1, 1)) && NPC.Distance(player.Center) <= 200f && OrroboroDoesNotExist && NPC.ai[1] == 0)
            {
                if (Main.mouseRight && Main.mouseRightRelease && !EggEventWorld.EggEventActive)
                {
                    Main.mouseRightRelease = false;

                    //summon orroboro if the egg incursion has been completed
                    if ((player.HasItem(ModContent.ItemType<Concoction>()) && Flags.downedEggEvent))
                    {
                        SoundEngine.PlaySound(EggDecaySound, NPC.Center);

                        NPC.ai[1] = 1;
                    }
                    //if the player hasnt completed the egg incursion or has a strange cyst, start the egg incursion
                    else if ((player.HasItem(ModContent.ItemType<Concoction>()) && !Flags.downedEggEvent) || player.ConsumeItem(ModContent.ItemType<StrangeCyst>()))
                    {
                        SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.Center);

                        SpookyPlayer.ScreenShakeAmount = 8;

                        NPC.ai[0] = 420;
                        EggEventWorld.EggEventActive = true;
                    }
                }
            }

            //orroboro spawn animation
            if (NPC.ai[1] > 0)
            {
                NPC.ai[1]++;

                //stretch the egg and increase the frame count using NPC.ai[2] 
                if (NPC.ai[2] < 5)
                {
                    if (NPC.ai[1] >= 45)
                    {
                        SoundEngine.PlaySound(EggCrackSound1, NPC.Center);

                        stretchRecoil = Main.rand.NextFloat(0.25f, 0.5f);

                        NPC.ai[2]++;
                        NPC.ai[1] = 1;
                    }
                }
                //spawn orroboro, reset ai variables
                else
                {
                    SoundEngine.PlaySound(EggCrackSound2, NPC.Center);

                    //spawn message
                    string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.OrroboroSpawn");

                    if (Main.netMode != NetmodeID.SinglePlayer) 
                    {
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));

                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)SpookyMessageType.SpawnOrroboro);
                        packet.Send();
                    }
                    else 
                    {
                        Main.NewText(text, 171, 64, 255);

                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OrroHeadP1>(), 0, -1);
                    }

                    //spawn egg gores
                    for (int numGores = 1; numGores <= 7; numGores++)
                    {
                        if (Main.netMode != NetmodeID.Server) 
                        {
                            Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.Next(-8, 8), Main.rand.Next(-7, -3)), ModContent.Find<ModGore>("Spooky/OrroboroEggGore" + numGores).Type);
                        }
                    }

                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                }
            }

            //egg event handling
            if (EggEventWorld.EggEventActive)
            {
                //bool to check if any enemies during the event exist
                EventEnemiesExist = NPC.AnyNPCs(ModContent.NPCType<Glutinous>()) || NPC.AnyNPCs(ModContent.NPCType<Vigilante>()) || 
                NPC.AnyNPCs(ModContent.NPCType<Ventricle>()) || NPC.AnyNPCs(ModContent.NPCType<Crux>()) || NPC.AnyNPCs(ModContent.NPCType<Vesicator>());

                //set the vignette effect around the egg
                VignettePlayer vignettePlayer = player.GetModPlayer<VignettePlayer>();
                vignettePlayer.SetVignette(0f, 1700f, 1f, Color.Black, new Vector2(NPC.Center.X, NPC.Center.Y - 85));

                //push players towards the egg if they get too far so they cannot leave the area during the event
                for (int i = 0; i <= Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead)
                    {
                        float distance = Main.player[i].Distance(new Vector2(NPC.Center.X, NPC.Center.Y - 85));
                        if (distance > 1600 && Main.player[i].InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
                        {
                            Vector2 movement = new Vector2(NPC.Center.X, NPC.Center.Y - 85) - Main.player[i].Center;
                            float difference = movement.Length() - 600;
                            movement.Normalize();
                            movement *= difference < 25f ? difference : 25f;
                            Main.player[i].position += movement;
                        }
                    }
                }

                //cooldown before switching to the next wave
                NPC.ai[0]--;

                //spawn enemies before checking for the wave switch so the enemies spawn in first
                //prevents the wave from being switched automatically before enemies are spawned
                if (NPC.ai[0] == 170)
                {
                    SpawnEnemies();

                    NPC.netUpdate = true;
                }

                if ((NPC.ai[0] <= 0 && EggEventWorld.Wave < 10) || EggEventWorld.Wave >= 10)
                {
                    if (SpawnedEnemies && !EventEnemiesExist)
                    {
                        SwitchToNextWave();

                        NPC.netUpdate = true;
                    }
                }
            }
            else
            {
                NPC.ai[0] = 0;
                ShieldScale = 0.5f;
                SpawnedEnemies = false;
            }
        }
    }
}