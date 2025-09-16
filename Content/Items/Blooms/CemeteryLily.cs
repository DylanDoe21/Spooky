using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Blooms
{
	public class CemeteryLily : ModItem
	{
        public override void SetStaticDefaults() 
        {
			Item.ResearchUnlockCount = 2;

			ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] 
            {
				new Color(201, 34, 32),
                new Color(228, 188, 68),
                new Color(87, 105, 54)
			};
		}

		public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 42;
            Item.consumable = true;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.rare = ModContent.RarityType<BloomPreHMRarity>();
			Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.maxStack = 9999;
			Item.scale = 0.5f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<BloomBuffsPlayer>().CanConsumeFruit("CemeteryLily");
        }

		public override bool? UseItem(Player player)
		{
			player.GetModPlayer<BloomBuffsPlayer>().AddBuffToList("CemeteryLily", 10800);

            //dont increase the revive amount if the player already has revives to prevent abusing the mechanic
            if (player.GetModPlayer<BloomBuffsPlayer>().CemeteryLilyRevives <= 0)
            {
			    player.GetModPlayer<BloomBuffsPlayer>().CemeteryLilyRevives = 2;
            }

			return true;
		}
    }
}
