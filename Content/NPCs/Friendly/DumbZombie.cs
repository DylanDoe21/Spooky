using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI;

namespace Spooky.Content.NPCs.Friendly
{
    public class DumbZombie : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 200;
            NPC.defense = 5;
            NPC.width = 40;
			NPC.height = 44;
            NPC.friendly = true;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 7;
            TownNPCStayingHomeless = true;
        }

        public override bool CanChat() 
        {
			return true;
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.DumbZombie"),
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement()
			});
		}

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "";
		}

        public override string GetChat()
		{
			return Language.GetTextValue("Mods.Spooky.Dialogue.DumbZombie.Dialogue" + Main.rand.Next(1, 4));
		}

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;

            NPC.velocity.X *= 0;

            if (Main.rand.NextBool(1500))
            {
                EmoteBubble.NewBubble(EmoteID.EmoteConfused, new WorldUIAnchor(NPC), 200);
            }
        }
    }
}