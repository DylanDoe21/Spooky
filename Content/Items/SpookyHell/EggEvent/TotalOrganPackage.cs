using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.EggEvent
{
    public class TotalOrganPackage : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 66;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 50);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
			player.GetModPlayer<SpookyPlayer>().GooChompers = true;
			player.GetModPlayer<SpookyPlayer>().VeinChain = true;
			player.GetModPlayer<SpookyPlayer>().PeptoStomach = true;
			player.GetModPlayer<SpookyPlayer>().StonedKidney = true;
			player.GetModPlayer<SpookyPlayer>().SmokerLung = true;
			player.GetModPlayer<SpookyPlayer>().GiantEar = true;

            //10% crit from goo chompers
            player.GetCritChance(DamageClass.Generic) += 10;

			bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<EarParasite>()] <= 0;
			if (NotSpawned && player.whoAmI == Main.myPlayer)
			{
				//leave the source as null for right now
				Projectile.NewProjectile(null, player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, ModContent.ProjectileType<EarParasite>(), 0, 0f, player.whoAmI);
			}
		}

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GooChompers>())
            .AddIngredient(ModContent.ItemType<VeinChain>())
            .AddIngredient(ModContent.ItemType<PeptoStomach>())
            .AddIngredient(ModContent.ItemType<StonedKidney>())
            .AddIngredient(ModContent.ItemType<SmokerLung>())
            .AddIngredient(ModContent.ItemType<GiantEar>())
            .AddIngredient(ModContent.ItemType<ArteryPiece>(), 50)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
        }
    }
}