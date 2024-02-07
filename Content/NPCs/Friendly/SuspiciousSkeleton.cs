using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Cemetery.Contraband;
using Spooky.Content.Items.Pets;

namespace Spooky.Content.NPCs.Friendly
{
    public class SuspiciousSkeleton : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) 
            {
				Velocity = 1f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
            NPC.defense = 5;
            NPC.width = 34;
			NPC.height = 46;
            NPC.friendly = true;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 7;
            TownNPCStayingHomeless = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.RaveyardBiome>().Type };
        }

        public override bool CanChat() 
        {
			return true;
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SuspiciousSkeleton"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.RaveyardBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {   
            //walking animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
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
			button = Language.GetTextValue("Mods.Spooky.Dialogue.SuspiciousSkeleton.ShopButton");
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
			return Language.GetTextValue("Mods.Spooky.Dialogue.SuspiciousSkeleton.Dialogue" + Main.rand.Next(1, 9));
		}

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<SkeletonBouncer>()))
            {
                player.GetModPlayer<SpookyPlayer>().RaveyardGuardsHostile = true;
            }
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<SkeletonBouncer>()) && !projectile.friendly)
            {
                Main.player[projectile.owner].GetModPlayer<SpookyPlayer>().RaveyardGuardsHostile = true;
            }
        }

        public override void AddShops()
        {
            Condition RotGourdDowned = new Condition("Mods.Spooky.Conditions.RotGourdDowned", () => Flags.downedRotGourd);
            Condition SpookySpiritDowned = new Condition("Mods.Spooky.Conditions.SpookySpiritDowned", () => Flags.downedSpookySpirit);
            Condition MocoDowned = new Condition("Mods.Spooky.Conditions.MocoDowned", () => Flags.downedMoco);
            Condition DaffodilDowned = new Condition("Mods.Spooky.Conditions.DaffodilDowned", () => Flags.downedDaffodil);
            Condition OrroboroDowned = new Condition("Mods.Spooky.Conditions.OrroboroDowned", () => Flags.downedOrroboro);
            Condition BigBoneDowned = new Condition("Mods.Spooky.Conditions.BigBoneDowned", () => Flags.downedBigBone);

            var npcShop = new NPCShop(Type)
            //analog horror
            .Add<GeminiEntertainmentGame>(RotGourdDowned)
            .Add<MandelaCatalogueTV>(SpookySpiritDowned)
            .Add<CarnisFlavorEnhancer>(MocoDowned)
            .Add<BackroomsCorpse>(DaffodilDowned)
            .Add<Local58Telescope>(OrroboroDowned)
            .Add<MonumentMythosPyramid>(BigBoneDowned)
            //creepypasta
            .Add<PolybiusArcadeGame>(RotGourdDowned)
            .Add<SmileDogPicture>(SpookySpiritDowned)
            .Add<RedMistClarinet>(MocoDowned)
            .Add<SlendermanPage>(DaffodilDowned)
            .Add<RedGodzillaCartridge>(OrroboroDowned)
            .Add<HerobrineAltar>(BigBoneDowned)
            //random stuff
            .Add(new Item(ModContent.ItemType<PetscopTool1>()) { shopCustomPrice = 100000 })
            .Add(new Item(ModContent.ItemType<PetscopTool2>()) { shopCustomPrice = 100000 })
            .Add(new Item(ModContent.ItemType<PetscopTool3>()) { shopCustomPrice = 100000 })
            .Add<SirenHead>()
            .Add<PartySkeletonSkull>();

            npcShop.Register();
        }
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

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
    }
}