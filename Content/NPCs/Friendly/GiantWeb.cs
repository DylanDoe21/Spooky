using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;

namespace Spooky.Content.NPCs.Friendly
{
    public class GiantWeb : ModNPC  
    {
        bool HasSkeletonPiece = Main.LocalPlayer.HasItem(ModContent.ItemType<OldHunterHat>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<OldHunterSkull>()) ||
        Main.LocalPlayer.HasItem(ModContent.ItemType<OldHunterTorso>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<OldHunterLegs>());

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
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
                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }
            if (Flags.OldHunterSkull)
            {
                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebSkull").Value;
                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }
            if (Flags.OldHunterTorso)
            {
                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebTorso").Value;
                Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            }
            if (Flags.OldHunterLegs)
            {
                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/GiantWebLegs").Value;
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
			button = "Insert Skeleton Piece";
		}

        public override string GetChat()
		{
			return "You are holding a piece of the skeleton, it looks like it can fit in the molds. Insert it?";
		}

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
		{
            if (firstButton)
            {
                Main.npcChatText = "";

                if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<OldHunterHat>()))
                {
                    Flags.OldHunterHat = true;
                }
                if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<OldHunterSkull>()))
                {
                    Flags.OldHunterSkull = true;
                }
                if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<OldHunterTorso>()))
                {
                    Flags.OldHunterTorso = true;
                }
                if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<OldHunterLegs>()))
                {
                    Flags.OldHunterLegs = true;
                }
            }
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, Color.White.ToVector3() * 0.1f);
            NPC.velocity *= 0;
        }
    }
}