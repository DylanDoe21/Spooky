using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;

namespace Spooky.Content.NPCs.Friendly
{
    public class GiantWeb : ModNPC  
    {
        public bool ConsumeItem = false;

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
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
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

                ConsumeItem = true;
                NPC.netUpdate = true;
            }
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.1f);
            NPC.velocity *= 0;

            if (ConsumeItem)
            {
                if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<OldHunterHat>()) && !Flags.OldHunterHat)
                {
                    Flags.OldHunterHat = true;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.WorldData);
                    }
                }
                if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<OldHunterSkull>()) && !Flags.OldHunterSkull)
                {
                    Flags.OldHunterSkull = true;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.WorldData);
                    }
                }
                if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<OldHunterTorso>()) && !Flags.OldHunterTorso)
                {
                    Flags.OldHunterTorso = true;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.WorldData);
                    }
                }
                if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<OldHunterLegs>()) && !Flags.OldHunterLegs) 
                {
                    Flags.OldHunterLegs = true;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.WorldData);
                    }
                }

                ConsumeItem = false;
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

                Flags.OldHunterAssembled = true;

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }

                NPC.netUpdate = true;
                NPC.active = false;
            }
        }
    }
}