/*
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;

namespace Spooky.Content.Items.SpookyBiome
{
    public class HauntedSac : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Haunted Sac");
            Tooltip.SetDefault("Summons a haunted sac to float above you"
            + "\nEvery thirty seconds, the sac will summon a powerful ghost that chases enemies"
            + "\nEnemies killed by the ghost will drop ectoplasm"
            + "\nPicking up ectoplasm will give you temporary summoner stat buffs");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 34;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;  
			Item.value = Item.buyPrice(gold: 15);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().HauntedSac = true;
            
            bool NotSpawned = player.ownedProjectileCounts[ModContent.ProjectileType<HauntedSacProj>()] <= 0;
			if (NotSpawned && player.whoAmI == Main.myPlayer)
			{
				//leave the source as null for right now
				Projectile.NewProjectile(null, player.position.X + (float)(player.width / 2), player.position.Y + (float)(player.height / 2), 0f, 0f, 
				ModContent.ProjectileType<HauntedSacProj>(), 0, 0f, player.whoAmI, 0f, 0f);
			}
        }
    }
}
*/