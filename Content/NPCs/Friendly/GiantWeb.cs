using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Items.BossSummon;

namespace Spooky.Content.NPCs.Friendly
{
    public class GiantWeb : ModNPC  
    {
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
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.behindTiles = true;
            NPC.aiStyle = -1;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Flags.OldHunterHat)
            {
                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebHat").Value;
                Texture2D outlineTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebHatOutline").Value;

                Main.EntitySpriteDraw(outlineTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4),
                NPC.frame, Color.White * 0.25f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }
            if (Flags.OldHunterSkull)
            {
                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebSkull").Value;
                Texture2D outlineTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebSkullOutline").Value;

                Main.EntitySpriteDraw(outlineTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4),
                NPC.frame, Color.White * 0.25f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }
            if (Flags.OldHunterTorso)
            {
                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebTorso").Value;
                Texture2D outlineTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebTorsoOutline").Value;

                Main.EntitySpriteDraw(outlineTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4),
                NPC.frame, Color.White * 0.25f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }
            if (Flags.OldHunterLegs)
            {
                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebLegs").Value;
                Texture2D outlineTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebLegsOutline").Value;

                Main.EntitySpriteDraw(outlineTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4),
                NPC.frame, Color.White * 0.25f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);

                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }
        }

        public override bool NeedSaving()
        {
            return true;
        }

        /*
        public override bool CanChat()
        {
            bool HasSkeletonPiece = Main.LocalPlayer.HasItem(ModContent.ItemType<OldHunterHat>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<OldHunterSkull>()) ||
            Main.LocalPlayer.HasItem(ModContent.ItemType<OldHunterTorso>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<OldHunterLegs>());

            return HasSkeletonPiece;
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("Mods.Spooky.Dialogue.GiantWeb.Button");
		}

        public override string GetChat()
		{
			return Language.GetTextValue("Mods.Spooky.Dialogue.GiantWeb.Dialogue");
		}

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
		{
            if (firstButton)
            {
                Main.npcChatText = "";

                SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack, NPC.Center);

                SpookyPlayer.ScreenShakeAmount = 5;

                NPC.ai[0] = 1;
                NPC.netUpdate = true;
            }
        }
        */

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.1f);
            NPC.velocity *= 0;

            bool PlayerHasSkeletonPiece = Main.player[Main.myPlayer].HasItem(ModContent.ItemType<OldHunterHat>()) || Main.player[Main.myPlayer].HasItem(ModContent.ItemType<OldHunterSkull>()) ||
            Main.player[Main.myPlayer].HasItem(ModContent.ItemType<OldHunterTorso>()) || Main.player[Main.myPlayer].HasItem(ModContent.ItemType<OldHunterLegs>());

            if (NPC.Hitbox.Intersects(new Rectangle((int)Main.MouseWorld.X - 2, (int)Main.MouseWorld.Y - 2, 5, 5)) && 
            NPC.Hitbox.Intersects(Main.player[Main.myPlayer].Hitbox) && PlayerHasSkeletonPiece && Main.myPlayer == Main.player[Main.myPlayer].whoAmI)
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

                if (Main.player[Main.myPlayer].ConsumeItem(ModContent.ItemType<OldHunterHat>()) && !Flags.OldHunterHat)
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
                if (Main.player[Main.myPlayer].ConsumeItem(ModContent.ItemType<OldHunterSkull>()) && !Flags.OldHunterSkull)
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
                if (Main.player[Main.myPlayer].ConsumeItem(ModContent.ItemType<OldHunterTorso>()) && !Flags.OldHunterTorso)
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
                if (Main.player[Main.myPlayer].ConsumeItem(ModContent.ItemType<OldHunterLegs>()) && !Flags.OldHunterLegs) 
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