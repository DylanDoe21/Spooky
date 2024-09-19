using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Vinyl;

namespace Spooky.Content.NPCs.Friendly
{
	public class Musicman : ModNPC
	{
        public override void SetStaticDefaults()
		{
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 106;
			NPC.height = 84;
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
            NPC.aiStyle = 7;
			TownNPCStayingHomeless = true;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.RaveyardBiome>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Musicman"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.RaveyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 7)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("LegacyInterface.28");
		}

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = Language.GetTextValue("LegacyInterface.28");
				SoundEngine.PlaySound(SoundID.Item166, NPC.Center);
            }
		}

        public override string GetChat()
		{
            return "...";
		}

		public override void AddShops()
        {
            var npcShop = new NPCShop(Type)
            .Add<VinylMysterious>()
            .Add<VinylLazy>()
            .Add<VinylSleepy>()
            .Add<VinylSneaky>()
            .Add<VinylAlley>()
            .Add<RaveyardDisc1>()
            .Add<RaveyardDisc2>()
            .Add<RaveyardDisc3>()
            .Add<BananalizardHead>()
            .Add<DylanDoeHead>()
            .Add<HatHead>()
            .Add<KrakenHead>()
            .Add<PelusaHead>()
            .Add<SeasaltHead>()
            .Add<TacoHead>()
            .Add<WaasephiHead>();

            npcShop.Register();
        }

		public override void AI()
		{
			NPC.velocity.X *= 0;

			NPC.localAI[0]++;

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

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
    }
}