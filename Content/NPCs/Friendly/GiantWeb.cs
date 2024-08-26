using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;

namespace Spooky.Content.NPCs.Friendly
{
    public class GiantWeb : ModNPC  
    {
        private static Asset<Texture2D> HatTexture;
        private static Asset<Texture2D> HatOutlineTexture;
        private static Asset<Texture2D> SkullTexture;
        private static Asset<Texture2D> SkullOutlineTexture;
        private static Asset<Texture2D> TorsoTexture;
        private static Asset<Texture2D> TorsoOutlineTexture;
        private static Asset<Texture2D> LegsTexture;
        private static Asset<Texture2D> LegsOutlineTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 334;
			NPC.height = 278;
            NPC.npcSlots = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.behindTiles = true;
            NPC.dontCountMe = true;
            NPC.aiStyle = -1;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Flags.OldHunterHat)
            {
                HatTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebHat");
                HatOutlineTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebHatOutline");

                Main.EntitySpriteDraw(HatOutlineTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4),
                NPC.frame, Color.White * 0.25f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(HatTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }
            if (Flags.OldHunterSkull)
            {
                SkullTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebSkull");
                SkullOutlineTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebSkullOutline");

                Main.EntitySpriteDraw(SkullOutlineTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4),
                NPC.frame, Color.White * 0.25f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(SkullTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }
            if (Flags.OldHunterTorso)
            {
                TorsoTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebTorso");
                TorsoOutlineTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebTorsoOutline");

                Main.EntitySpriteDraw(TorsoOutlineTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4),
                NPC.frame, Color.White * 0.25f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(TorsoTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }
            if (Flags.OldHunterLegs)
            {
                LegsTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebLegs");
                LegsOutlineTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebLegsOutline");

                Main.EntitySpriteDraw(LegsOutlineTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4),
                NPC.frame, Color.White * 0.25f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(LegsTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }
        }

        public override bool NeedSaving()
        {
            return true;
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.1f);
            NPC.velocity *= 0;

            Player player = Main.player[Main.myPlayer];

            bool PlayerHasSkeletonPiece = player.HasItem(ModContent.ItemType<OldHunterHat>()) || player.HasItem(ModContent.ItemType<OldHunterSkull>()) || player.HasItem(ModContent.ItemType<OldHunterTorso>()) || player.HasItem(ModContent.ItemType<OldHunterLegs>());

            if (NPC.Hitbox.Intersects(new Rectangle((int)Main.MouseWorld.X - 2, (int)Main.MouseWorld.Y - 2, 5, 5)) && !Main.mapFullscreen && PlayerHasSkeletonPiece && NPC.Hitbox.Intersects(player.Hitbox) && Main.myPlayer == player.whoAmI)
            {
                NPC.GivenName = Language.GetTextValue("Mods.Spooky.NPCs.GiantWeb.DisplayNameAlt");

                if (Main.mouseRight && NPC.ai[0] == 0)
                {
                    NPC.ai[0] = 1;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                NPC.GivenName = Language.GetTextValue("Mods.Spooky.NPCs.GiantWeb.DisplayName");
            }

            if (NPC.ai[0] == 1)
            {
                SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack, NPC.Center);

                SpookyPlayer.ScreenShakeAmount = 5;

                if (player.ConsumeItem(ModContent.ItemType<OldHunterHat>()) && !Flags.OldHunterHat)
                {
                    if (Main.netMode != NetmodeID.SinglePlayer)
				    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)SpookyMessageType.OldHunterHat);
                        packet.Send();
                    }
                    else
                    {
                        Flags.OldHunterHat = true;
                    }
                }
                if (player.ConsumeItem(ModContent.ItemType<OldHunterSkull>()) && !Flags.OldHunterSkull)
                {
                    if (Main.netMode != NetmodeID.SinglePlayer)
				    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)SpookyMessageType.OldHunterSkull);
                        packet.Send();
                    }
                    else
                    {
                        Flags.OldHunterSkull = true;
                    }
                }
                if (player.ConsumeItem(ModContent.ItemType<OldHunterTorso>()) && !Flags.OldHunterTorso)
                {
                    if (Main.netMode != NetmodeID.SinglePlayer)
				    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)SpookyMessageType.OldHunterTorso);
                        packet.Send();
                    }
                    else
                    {
                        Flags.OldHunterTorso = true;
                    }
                }
                if (player.ConsumeItem(ModContent.ItemType<OldHunterLegs>()) && !Flags.OldHunterLegs) 
                {
                    if (Main.netMode != NetmodeID.SinglePlayer)
				    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)SpookyMessageType.OldHunterLegs);
                        packet.Send();
                    }
                    else
                    {
                        Flags.OldHunterLegs = true;
                    }
                }

                NPC.ai[0] = 0;

                NPC.netUpdate = true;
            }

            if (Flags.OldHunterHat && Flags.OldHunterSkull && Flags.OldHunterTorso && Flags.OldHunterLegs)
            {
                SoundEngine.PlaySound(SoundID.NPCHit11, NPC.Center);
                SoundEngine.PlaySound(SoundID.DD2_DefeatScene with { Pitch = SoundID.DD2_DefeatScene.Pitch - 0.8f }, NPC.Center);

                int Animation = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 25, ModContent.NPCType<GiantWebAnimationBase>());

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {   
                    NetMessage.SendData(MessageID.SyncNPC, number: Animation);
                }

                for (int numGores = 1; numGores <= 15; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), new Vector2(NPC.Center.X + Main.rand.Next(-NPC.width / 2, NPC.width / 2), NPC.Center.Y + Main.rand.Next(-NPC.height / 2, NPC.height / 2)), 
                        NPC.velocity, ModContent.Find<ModGore>("Spooky/GiantWebGore" + numGores).Type);
                    }
                }

                for (int numGores = 16; numGores <= 19; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/GiantWebGore" + numGores).Type);
                    }
                }

                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)SpookyMessageType.OldHunterAssembled);
                    packet.Send();
                }
                else
                {
                    Flags.OldHunterAssembled = true;
                }

                NPC.netUpdate = true;
                
                NPC.active = false;
            }
        }
    }
}