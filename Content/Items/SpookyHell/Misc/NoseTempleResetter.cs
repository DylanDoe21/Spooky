using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.Tiles.NoseTemple;

namespace Spooky.Content.Items.SpookyHell.Misc
{
    public class NoseTempleResetter : ModItem
    {
        public override void SetDefaults()
        {                
            Item.width = 50;
            Item.height = 46;
            Item.noUseGraphic = true;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.DD2_EtherianPortalOpen;
            Item.rare = ItemRarityID.Blue;
        }

		public override bool CanUseItem(Player player)
		{
			return !AnyPlayersInBiome();
		}

		public override bool? UseItem(Player player)
        {
            Flags.downedMocoIdol1 = false;
            Flags.downedMocoIdol2 = false;
            Flags.downedMocoIdol3 = false;
            Flags.downedMocoIdol4 = false;
            Flags.downedMocoIdol5 = false;
            Flags.downedMocoIdol6 = false;

            NetMessage.SendData(MessageID.WorldData);

            return true;
        }

        //check if any player is in the range to activate the range
        public bool AnyPlayersInBiome()
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];

                int playerInBiomeCount = 0;

                if (player.active && !player.dead && player.InModBiome(ModContent.GetInstance<NoseTempleBiome>()))
                {
                    playerInBiomeCount++;
                }

                if (playerInBiomeCount >= 1)
                {
                    return true;
                }
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("SpookyMod:DemoniteBars", 10)
            .AddIngredient(ItemID.Wire, 15)
			.AddIngredient(ModContent.ItemType<SnotGlob>(), 5)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}