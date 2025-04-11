using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.Chat;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;

using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.NPCs.Boss.Orroboro;

namespace Spooky.Content.NPCs.EggEvent
{
    public class OrroboroEgg : ModNPC
    {
        float ShieldScale = 0.5f;
        float ShieldAlpha = 1f;

        float addedStretch = 0f;
		float stretchRecoil = 0f;

        bool OrroboroDoesNotExist;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> AuraTexture1;
        private static Asset<Texture2D> AuraTexture2;

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
            writer.Write(OrroboroDoesNotExist);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            OrroboroDoesNotExist = reader.ReadBoolean();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 250;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 128;
            NPC.height = 122;
            NPC.npcSlots = 0f;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            AuraTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/OrroboroEggGlow1");
            AuraTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/OrroboroEggGlow2");

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

            spriteBatch.Draw(NPCTexture.Value, DrawPos, NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, SpriteEffects.None, 0f);

            if (EggEventWorld.EggEventActive)
            {
                float fade1 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;
                float fade2 = (float)Math.Sin((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

                spriteBatch.Draw(AuraTexture1.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, NPC.GetAlpha(Color.Red) * fade1, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
                spriteBatch.Draw(AuraTexture2.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, NPC.GetAlpha(Color.Red) * fade2, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;
            float fade2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 150f)) / 2f + 0.5f;

            if (!Flags.downedEggEvent || EggEventWorld.EggEventActive) 
            {
                Color shieldColor = Color.Indigo;

                if (EggEventWorld.EggEventActive)
                {
                    shieldColor = Color.Lerp(Color.Indigo, Color.Red, fade);

                    if (NPC.ai[3] == 0 && ShieldScale < 0.8f)
                    {
                        ShieldScale += 0.0025f;
                    }
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                var center = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 12);
                DrawData drawData = new DrawData(ModContent.Request<Texture2D>("Spooky/ShaderAssets/EggShieldNoise").Value, center,
                new Rectangle(0, 0, 500, 420), shieldColor * ShieldAlpha, 0, new Vector2(250f, 250f), NPC.scale * (ShieldScale + (NPC.ai[3] > 0 ? fade2 * 0.25f : fade * 0.05f)), SpriteEffects.None, 0);

                GameShaders.Misc["ForceField"].UseColor(new Vector3(1f + fade * 0.5f));
                GameShaders.Misc["ForceField"].Apply(drawData);
                drawData.Draw(Main.spriteBatch);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }

		public override void FindFrame(int frameHeight)
		{
			if (NPC.ai[0] == 0)
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
				NPC.frame.Y = (int)NPC.ai[1] * frameHeight;
			}
		}

        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.LocalPlayer;

            Spooky.OrroboroSpawnX = (int)NPC.Center.X;
            Spooky.OrroboroSpawnY = (int)NPC.Center.Y;

            OrroboroDoesNotExist = !NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) && !NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) && !NPC.AnyNPCs(ModContent.NPCType<BoroHead>());

            if (EggEventWorld.EggEventActive || !Flags.downedEggEvent) 
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

            //egg incursion beginning
            if (NPC.ai[2] >= 1)
            {
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, NPC.Center);

                //event start message
                string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.EggEventBegin");
                
                if (Main.netMode == NetmodeID.Server)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                }
                else if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(text, 171, 64, 255);
                }

                Screenshake.ShakeScreenWithIntensity(NPC.Center, 8f, 450f);

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)SpookyMessageType.EggIncursionStart);
                    packet.Send();
                }
                else
                {
                    EggEventWorld.EventTimeLeftUI = 21600;
                    EggEventWorld.EggEventActive = true;
                }

                NPC.ai[2] = 0;
                
                NPC.netUpdate = true;
            }

            //orroboro spawn animation
            if (NPC.ai[0] >= 1)
            {
                NPC.ai[0]++;

                //stretch the egg and increase the frame count using NPC.ai[1] 
                if (NPC.ai[1] < 5)
                {
                    if (NPC.ai[0] >= 45)
                    {
                        SoundEngine.PlaySound(EggCrackSound1, NPC.Center);

                        stretchRecoil = Main.rand.NextFloat(0.25f, 0.5f);

                        NPC.ai[1]++;
                        NPC.ai[0] = 1;
                    }
                }
                //spawn orroboro, reset ai variables
                else
                {
                    SoundEngine.PlaySound(EggCrackSound2, NPC.Center);

                    if (!NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()))
                    {
                        //spawn message
                        string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.OrroboroSpawn");

                        if (Main.netMode == NetmodeID.Server)
                        {
                            ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                        }
						else if (Main.netMode == NetmodeID.SinglePlayer)
						{
							Main.NewText(text, 171, 64, 255);
                        }

                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            ModPacket packet = Mod.GetPacket();
                            packet.Write((byte)SpookyMessageType.SpawnOrroboro);
                            packet.Send();
                        }
                        else
                        {
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OrroHeadP1>(), 0, -1);
                        }
					}

                    //spawn egg gores
                    for (int numGores = 1; numGores <= 7; numGores++)
                    {
                        if (Main.netMode != NetmodeID.Server) 
                        {
                            Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, new Vector2(Main.rand.Next(-8, 8), Main.rand.Next(-7, -3)), ModContent.Find<ModGore>("Spooky/OrroboroEggGore" + numGores).Type);
                        }
                    }

                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;

					NPC.netUpdate = true;
                }
            }

            //end egg incursion when the timer is past 6 minutes
            if (EggEventWorld.EggEventActive)
            {
                if (EggEventWorld.EventTimeLeft >= 21600)
                {
                    NPC.ai[3]++;
                    if (NPC.ai[3] == 120)
                    {
                        SoundEngine.PlaySound(EventEndSound, NPC.Center);
                    }

                    if (NPC.ai[3] >= 120)
                    {
                        ShieldScale += 0.25f;
                        ShieldAlpha -= 0.06f;
                    }

                    if (NPC.ai[3] >= 120 && ShieldAlpha <= 0f)
                    {
                        Screenshake.ShakeScreenWithIntensity(NPC.Center, 8f, 450f);

                        //kill all existing egg incursion enemies
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC Enemy = Main.npc[i];

                            int[] EventNPCs = new int[] { ModContent.NPCType<BiojetterEye>(), ModContent.NPCType<CoughLungs>(), ModContent.NPCType<CruxBat>(), ModContent.NPCType<EarWorm>(), ModContent.NPCType<ExplodingAppendix>(), 
                            ModContent.NPCType<GooSlug>(), ModContent.NPCType<HoppingHeart>(), ModContent.NPCType<HoverBrain>(), ModContent.NPCType<TongueBiter>(), ModContent.NPCType<FleshBolster>() };

                            if (Enemy.active && EventNPCs.Contains(Enemy.type))
                            {
                                Main.LocalPlayer.ApplyDamageToNPC(Enemy, Enemy.lifeMax * 2, 0, 0, false);
                            }
                        }

                        //event end message (different for after you have beaten the event already)
                        string text = Flags.downedEggEvent ? Language.GetTextValue("Mods.Spooky.EventsAndBosses.EggEventOverBeaten") : Language.GetTextValue("Mods.Spooky.EventsAndBosses.EggEventOver");
                        
                        if (Main.netMode == NetmodeID.Server)
                        {
                            ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                        }
                        else if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText(text, 171, 64, 255);
                        }

                        Flags.downedEggEvent = true;
                        EggEventWorld.EggEventActive = false;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.WorldData);
                        }

                        NPC.ai[3] = 0;

                        NPC.netUpdate = true;
                    }
                }
            }
            else
            {
                ShieldAlpha = 1f;
                ShieldScale = 0.5f;
            }
        }
    }
}