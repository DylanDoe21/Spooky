using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Friendly
{
    public class PartySkeletonDealer : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 200;
            NPC.defense = 5;
            NPC.width = 34;
			NPC.height = 46;
            NPC.friendly = true;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 7;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
        }

        public override bool CanChat() 
        {
			return true;
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PartySkeletonDealer"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {   
            NPC.frameCounter += 1;

            //walking  animation
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 9)
            {
                NPC.frame.Y = 1 * frameHeight;
            }

            //jumping/falling frame
            if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0 || NPC.velocity == Vector2.Zero)
            {
                NPC.frame.Y = 2 * frameHeight;
            }

            //still frame
            if (NPC.velocity == Vector2.Zero)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeletonDealer.ShopButton");
		}

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = Language.GetTextValue("LegacyInterface.28");
            }
		}

        public override string GetChat()
		{
			List<string> Dialogue = new List<string>
			{
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeletonDealer.Dialogue1"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeletonDealer.Dialogue2"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeletonDealer.Dialogue3"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeletonDealer.Dialogue4"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeletonDealer.Dialogue5"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeletonDealer.Dialogue6"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeletonDealer.Dialogue7"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeletonDealer.Dialogue8"),
			};

			return Main.rand.Next(Dialogue);
		}

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type)
                .Add<Items.Food.BlackLicorice>(Condition.Hardmode);

            npcShop.Register();
        }
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            NPC.GivenName = Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeletonDealer.Name");

            if (!Main.player[Main.myPlayer].InModBiome(ModContent.GetInstance<Biomes.RaveyardBiome>()))
            {
                NPC.alpha += 5;

                if (NPC.alpha >= 255)
                {
                    NPC.active = false;
                }
            }
            else
            {
                if (NPC.alpha >= 0)
                {
                    NPC.alpha -= 5;
                }
            }
        }
    }
}