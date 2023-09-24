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
    public class PartySkeleton1 : ModNPC  
    {
        int shirtStyle = 0;
        int shirtLogoStyle = 0;
        int pantsStyle = 0;

        bool hasShirt = false;
        bool shirtLogo = false;
        bool hasPants = false;
        bool hasSunGlasses = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
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
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PartySkeleton1"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (hasPants)
            {
                Texture2D pantsTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Pants1").Value;

                switch (pantsStyle)
                {
                    case 0:
                    {
                        pantsTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Pants1").Value;
                        break;
                    }
                    case 1:
                    {
                        pantsTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Pants2").Value;
                        break;
                    }
                    case 2:
                    {
                        pantsTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Pants3").Value;
                        break;
                    }
                }

                Main.EntitySpriteDraw(pantsTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }

            if (hasShirt)
            {
                Texture2D shirtTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt1").Value;

                switch (shirtStyle)
                {
                    case 0:
                    {
                        shirtTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt1").Value;
                        break;
                    }
                    case 1:
                    {
                        shirtTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt2").Value;
                        break;
                    }
                    case 2:
                    {
                        shirtTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt3").Value;
                        break;
                    }
                    case 3:
                    {
                        shirtTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt4").Value;
                        break;
                    }
                    case 4:
                    {
                        shirtTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/Shirt5").Value;
                        break;
                    }
                }

                Main.EntitySpriteDraw(shirtTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }

            if (shirtLogo)
            {
                Texture2D shirtLogoTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/ShirtLogo1").Value;

                switch (shirtLogoStyle)
                {
                    case 0:
                    {
                        shirtLogoTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/ShirtLogo1").Value;
                        break;
                    }
                    case 1:
                    {
                        shirtLogoTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/ShirtLogo2").Value;
                        break;
                    }
                }

                Main.EntitySpriteDraw(shirtLogoTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }

            if (hasSunGlasses)
            {
                Texture2D sunglassTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Friendly/SkeletonClothes/SunGlasses").Value;

                Main.EntitySpriteDraw(sunglassTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
                NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
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
			button = "";
		}

        public override string GetChat()
		{
			List<string> Dialogue = new List<string>
			{
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue1"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue2"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue3"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue4"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue5"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue6"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue7"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue8"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue9"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue10"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue11"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue12"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue13"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue14"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue15"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue16"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue17"),
                Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue18"),
			};

			return Main.rand.Next(Dialogue);
		}
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] == 1)
            {
                //select a random name for the skeleton when it spawns
                string[] names = { "Boney", "Tony", "Jeff", "McRib", "Clemmence", "Hans Flabberghast", "Carlcium", "Ribert", "Nigel", 
                "Morton", "Jeremy", "Gustavo", "Notorious B.O.N.E", "Patrice", "Patrique", "Wanda", "Señor Hernandez", "John Jr", 
                "Fortunado", "Quandale", "Normal Skeleton", "Bongo", "Ongo", "Pete Griffith", "Tario" };
                NPC.GivenName = Main.rand.Next(names);

                //choose pants
                if (Main.rand.NextBool(3))
                {
                    hasPants = true;
                    pantsStyle = Main.rand.Next(0, 3);
                }

                //choose shirt
                if (Main.rand.NextBool(3))
                {
                    hasShirt = true;
                    shirtStyle = Main.rand.Next(0, 5);

                    if (Main.rand.NextBool())
                    {
                        shirtLogo = true;
                        shirtLogoStyle = Main.rand.Next(0, 2);
                    }
                }

                //choose if it will have sunglasses
                if (Main.rand.NextBool(10))
                {
                    hasSunGlasses = true;
                }
            }

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

    public class PartySkeleton2 : PartySkeleton1  
    {
    }

    public class PartySkeleton3 : PartySkeleton1  
    {
    }

    public class PartySkeleton4 : PartySkeleton1  
    {
    }

    public class PartySkeleton5 : PartySkeleton1  
    {
    }

    public class PartySkeleton6 : PartySkeleton1  
    {
    }
}