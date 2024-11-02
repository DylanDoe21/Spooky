using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.EggEvent
{
    public class GiantEar : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<GooChompers>();
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 50;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 30);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().GiantEar = true;
            
            bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<EarParasite>()] <= 0;
			if (NotSpawned && player.whoAmI == Main.myPlayer)
			{
				//leave the source as null for right now
				Projectile.NewProjectile(null, player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ModContent.ProjectileType<EarParasite>(), 0, 0f, player.whoAmI);
			}
        }
    }
}