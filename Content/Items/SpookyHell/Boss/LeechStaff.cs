using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.SpookyHell;
 
namespace Spooky.Content.Items.SpookyHell.Boss
{
    public class LeechStaff : ModItem
    {
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Staff of the Swarm");
			Tooltip.SetDefault("Casts leeches that will swarm nearby enemies"
			+ "\nEach leech costs 10 life to summon, leeches cannot be summoned when you are below 25% life"
			+ "\nLeeches will deal more damage the lower your health is, and they do not take up minion slots"
			+ "\nIf you are below half health, leeches will sometimes heal you on enemy hits");
			Item.staff[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
        {
            Item.damage = 55; 
			Item.mana = 10;                        
            Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;  
			Item.autoReuse = true;                  
            Item.width = 58;
            Item.height = 48;
            Item.useTime = 25;
            Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2); 
            Item.UseSound = SoundID.DD2_BetsysWrathShot;
            Item.shoot = ModContent.ProjectileType<Leech>();  
            Item.shootSpeed = 8f;
        }

		public override bool CanUseItem(Player player)
		{
			return player.statLife >= (player.statLifeMax2 / 4);
		}
	}
}