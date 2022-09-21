using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class CandyBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bag O' Treats");
            Tooltip.SetDefault("Summons a magical bag of candy to float above you"
            + "\nThe candy bag will drop pieces of candy every 25 seconds"
            + "\nPicking up candies will provide different summoner benefits");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().TreatBag = true;
            
            bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<TreatBag>()] <= 0;
			if (NotSpawned && player.whoAmI == Main.myPlayer)
			{
				//leave the source as null for right now
				Projectile.NewProjectile(null, player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, 
				ModContent.ProjectileType<TreatBag>(), 0, 0f, player.whoAmI, 0f, 0f);
			}
        }
    }
}